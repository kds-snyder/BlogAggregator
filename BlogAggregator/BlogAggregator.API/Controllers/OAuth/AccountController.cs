using BlogAggregator.API.OAuth;
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Models;
using BlogAggregator.Core.Services;
using BlogAggregator.Data.OAuth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace BlogAggregator.API.Controllers.OAuth
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string faceBookProviderName = "Facebook";
        private const string googleProviderName = "Google";          

        private readonly IAuthRepository _authRepository;

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        public AccountController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            string redirectUri = string.Empty;

            if (error != null)
            {
                return BadRequest(Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            var redirectUriValidationResult = validateClientAndRedirectUri(this.Request, ref redirectUri);

            if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
            {
                return BadRequest(redirectUriValidationResult);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            User user = await _authRepository.FindAsync
                (new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            redirectUri = string.Format
                ("{0}#external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}",
                redirectUri,
                externalLogin.ExternalAccessToken,
                externalLogin.LoginProvider,
                hasRegistered.ToString(),
                externalLogin.UserName);

            return Redirect(redirectUri);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ObtainLocalAccessToken")]
        public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
        {
            string emailInfo = "ObtainLocalAccessToken " + DateTime.Now + " provider = " + 
                provider + " externalAccessToken = " + externalAccessToken;
            var emailLog = new EmailLog();
            string emailLogSentTo = "kds_snyder@yahoo.com";
            string emailLogSubject = "ObtainLocalAccessToken Email Log";

            try
            {
                if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
                {
                    emailInfo = emailInfo + "\nProvider or external access token is not sent";
                    emailLog.SendEmail(emailLogSentTo, emailLogSubject, emailInfo);
                    return BadRequest("Provider or external access token is not sent");
                }

                var verifiedAccessToken = await verifyExternalAccessToken(provider, externalAccessToken);
                if (verifiedAccessToken == null)
                {
                    emailInfo = emailInfo + "\nInvalid Provider or External Access Token ";
                    emailLog.SendEmail(emailLogSentTo, emailLogSubject, emailInfo);
                    return BadRequest("Invalid Provider or External Access Token");
                }

                User user = await _authRepository.FindAsync(new UserLoginInfo(provider, verifiedAccessToken.user_id));

                bool hasRegistered = user != null;
                emailInfo = emailInfo + "\nUser ID: " + user.Id + " name: " + user.UserName;

                if (!hasRegistered)
                {
                    emailInfo = emailInfo + "\nExternal user is not registered";
                    emailLog.SendEmail(emailLogSentTo, emailLogSubject, emailInfo);
                    return BadRequest("External user is not registered");
                }

                //generate access token response
                var accessTokenResponse = generateLocalAccessTokenResponse(user.UserName);
                emailInfo = emailInfo + "\naccessTokenResponse: " + accessTokenResponse;
                emailLog.SendEmail(emailLogSentTo, emailLogSubject, emailInfo);
                return Ok(accessTokenResponse);
            }
            catch (Exception e)
            {
                emailInfo = emailInfo + e.Message;
                emailLog.SendEmail(emailLogSentTo, emailLogSubject, emailInfo);
                throw e;
            }

        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegistrationModel registrationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await _authRepository.RegisterUser(registrationModel);

            IHttpActionResult errorResult = getErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }

        // POST api/Account/RegisterExternal
        [AllowAnonymous]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(ExternalLoginModel.RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var verifiedAccessToken = await verifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            User user = await _authRepository.FindAsync(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                return BadRequest("External user is already registered");
            }

            user = new User() { UserName = model.UserName };

            IdentityResult result = await _authRepository.CreateAsync(user);
            if (!result.Succeeded)
            {
                return getErrorResult(result);
            }

            var info = new ExternalLoginInfo()
            {
                DefaultUserName = model.UserName,
                Login = new UserLoginInfo(model.Provider, verifiedAccessToken.user_id)
            };

            result = await _authRepository.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return getErrorResult(result);
            }

            //generate access token response
            var accessTokenResponse = generateLocalAccessTokenResponse(model.UserName);

            return Ok(accessTokenResponse);
        }
       

        private JObject generateLocalAccessTokenResponse(string userName)
        {

            var tokenExpiration = TimeSpan.FromDays(1);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(
                                        new JProperty("userName", userName),
                                        new JProperty("access_token", accessToken),
                                        new JProperty("token_type", "bearer"),
                                        new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                        new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                        new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
        );

            return tokenResponse;
        }

        private IHttpActionResult getErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private string getQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }

        private string validateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        {

            Uri redirectUri;

            var redirectUriString = getQueryString(Request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }

            var clientId = getQueryString(Request, "client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return "client_Id is required";
            }

            var client = _authRepository.FindClient(clientId);

            if (client == null)
            {
                return string.Format("Client_id '{0}' is not registered in the system.", clientId);
            }

            if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
            {
                return string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
            }

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;
        }

        private async Task<ExternalLoginModel.ParsedExternalAccessToken> verifyExternalAccessToken(string provider, string accessToken)
        {
            ExternalLoginModel.ParsedExternalAccessToken parsedToken = null;

            var verifyTokenEndPoint = "";

            //if (provider == faceBookProviderName)
            //{
            //    //You can get it from here: https://developers.facebook.com/tools/accesstoken/
            //    //More about debug_tokn here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook

            //    var appToken = "xxxxx";
            //    verifyTokenEndPoint = 
            //          string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", 
            //                  accessToken, appToken);
            //}
            //else 
            if (provider == googleProviderName)
            {
                verifyTokenEndPoint = 
                    string.Format("https://www.googleapis.com/oauth2/v3/tokeninfo?access_token={0}", accessToken);
            }
            else
            {
                return null;
            }

            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                dynamic jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                parsedToken = new ExternalLoginModel.ParsedExternalAccessToken();

                //if (provider == "Facebook")
                //{
                //    parsedToken.user_id = jObj["data"]["user_id"];
                //    parsedToken.app_id = jObj["data"]["app_id"];

                //    if (!string.Equals(Startup.facebookAuthOptions.AppId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                //    {
                //        return null;
                //    }
                //}
                //else 
                if (provider == googleProviderName)
                {
                    parsedToken.user_id = jObj["user_id"];
                    parsedToken.app_id = jObj["audience"];

                    if (!string.Equals(Startup.googleAuthOptions.ClientId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }

                }
            }
            return parsedToken;
        }
       
    }
}
