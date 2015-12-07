using BlogAggregator.Core.Domain;
using BlogAggregator.Data.Repository;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogAggregator.API.OAuth
{
    public class BlogAggregatorAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private Func<IAuthRepository> _authRepositoryFactory;
        private IAuthRepository _authRepository
        {
            get
            {
                return _authRepositoryFactory.Invoke();
            }
        }

        public BlogAggregatorAuthorizationServerProvider(Func<IAuthRepository> authRepositoryFactory)
        {
            _authRepositoryFactory = authRepositoryFactory;
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await Task.Factory.StartNew(() =>
            {
                context.Validated();
            });
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            User user = await _authRepository.FindUser(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            context.Validated(identity);
        }
    }
}
