using AutoMapper;
using BlogAggregator.API.OAuth;
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
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

        // Get user corresponding to external login
        [HttpGet]
        [Route("GetExternalLoginUser")]
        public async Task<IHttpActionResult> GetExternalLoginUser(ExternalLoginUserData externalLogin)
        {
            User dbUser = await _authRepository.FindAsync(new UserLoginInfo(externalLogin.LoginProvider, 
                                                                            externalLogin.ProviderKey));

            return Ok(Mapper.Map<UserModel>(dbUser));
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ObtainLocalAccessToken")]
        public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
        {
            string errorInfo = "\nObtainLocalAccessToken " + DateTime.Now + " provider: " + 
                provider + " externalAccessToken: " + externalAccessToken;            
            var emailLog = new EmailLog();
            string emailLogSentTo = "kds_snyder@yahoo.com";
            string emailLogSubject = "ObtainLocalAccessToken Email Log";

            try
            {
                if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
                {
                    errorInfo = errorInfo + "\nProvider or external access token is not sent";
                    emailLog.SendEmail(emailLogSentTo, emailLogSubject, errorInfo);
                    return BadRequest("Provider or external access token is not sent" + errorInfo);
                }

                var verifiedAccessToken = await verifyExternalAccessToken(provider, externalAccessToken);
                //errorInfo = errorInfo + verifiedAccessToken.errorDetails;
                if (verifiedAccessToken == null)
                {
                    errorInfo = errorInfo + "\nInvalid Provider or External Access Token ";
                    emailLog.SendEmail(emailLogSentTo, emailLogSubject, errorInfo);
                    return BadRequest("Invalid Provider or External Access Token");
                }

                User user = await _authRepository.FindAsync(new UserLoginInfo(provider, verifiedAccessToken.user_id));

                bool hasRegistered = user != null;
                errorInfo = errorInfo + "\nUser ID: " + user.Id + " name: " + user.UserName;

                if (!hasRegistered)
                {
                    errorInfo = errorInfo + "\nExternal user is not registered";
                    emailLog.SendEmail(emailLogSentTo, emailLogSubject, errorInfo);
                    return BadRequest("External user is not registered");
                }

                //Generate access token response
                //var accessTokenResponse = generateLocalAccessTokenResponse(user.UserName);
                var accessTokenResponse = generateLocalAccessTokenResponse(user);
                errorInfo = errorInfo + "\naccessTokenResponse: " + accessTokenResponse;
                emailLog.SendEmail(emailLogSentTo, emailLogSubject, errorInfo);
                return Ok(accessTokenResponse);
            }
            catch (Exception e)
            {
                errorInfo = errorInfo + e.Message;
                emailLog.SendEmail(emailLogSentTo, emailLogSubject, errorInfo);
                throw e;
            }

        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserRegistration registrationModel)
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
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {           
            string errorInfo = "\nRegisterExternal " + DateTime.Now + " user name: " + model.UserName +
                " provider: " + model.Provider + " externalAccessToken: " + model.ExternalAccessToken ;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var verifiedAccessToken = await verifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
            //errorInfo = errorInfo + verifiedAccessToken.errorDetails;
            //if (verifiedAccessToken.user_id == "")
            if (verifiedAccessToken == null)           
            {
                return BadRequest("Invalid Provider or External Access Token" + errorInfo);
            }

            User user = await _authRepository.FindAsync(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                return BadRequest("External user is already registered" + errorInfo);
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
            //var accessTokenResponse = generateLocalAccessTokenResponse(model.UserName);
            var accessTokenResponse = generateLocalAccessTokenResponse(user);

            return Ok(accessTokenResponse);
        }


        //private JObject generateLocalAccessTokenResponse(string userName)
        private JObject generateLocalAccessTokenResponse(User user)
        {

            var tokenExpiration = TimeSpan.FromDays(1);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(
                                        new JProperty("userName", user.UserName),
                                        new JProperty("isAuthorized", user.Authorized),
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

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;
        }

        private async Task<ParsedExternalAccessToken> 
                verifyExternalAccessToken(string provider, string accessToken)
        {            
           
            ParsedExternalAccessToken parsedToken = null;
            //var parsedToken = new ExternalLoginModel.ParsedExternalAccessToken();
            //parsedToken.errorDetails = "";
            //parsedToken.user_id = "";

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
                    string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);               
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
                
                parsedToken = new ParsedExternalAccessToken();

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

                    if (!string.Equals(Startup.googleAuthOptions.ClientId, parsedToken.app_id, 
                                                            StringComparison.OrdinalIgnoreCase))
                    {
                        //parsedToken.errorDetails = parsedToken.errorDetails +
                        //    "Mismatch: parsed user_id: " + parsedToken.user_id + " parsed app_id: " + parsedToken.app_id;
                        return null;
                    }

                }
                //else
                //{
                //    parsedToken.errorDetails = parsedToken.errorDetails + "Not google";
                //}
            }
            //else
            //{
            //    parsedToken.errorDetails = parsedToken.errorDetails +
            //        "Unsuccessful status response";
            //}
            return parsedToken;
        }
       
    }
}
