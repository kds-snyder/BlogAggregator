using BlogAggregator.API.OAuth;
using BlogAggregator.Core.BlogReader.WordPress;
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Repository;
using BlogAggregator.Core.Services;
using BlogAggregator.Data.Infrastructure;
using BlogAggregator.Data.OAuth;
using BlogAggregator.Data.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;
using SimpleInjector.Integration.WebApi;
using System;
using System.Web.Http;

[assembly: OwinStartup(typeof(BlogAggregator.API.Startup))]
namespace BlogAggregator.API
{
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static GoogleOAuth2AuthenticationOptions googleAuthOptions { get; private set; }
        public static FacebookAuthenticationOptions facebookAuthOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            Container container = ConfigureSimpleInjector(app);

            HttpConfiguration config = new HttpConfiguration
            {
                DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container)
            };

            WebApiConfig.Register(config);

            ConfigureOAuth(app, container);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            app.UseWebApi(config);

        }

        public void ConfigureOAuth(IAppBuilder app, Container container)
        {
            Func<IAuthRepository> authRepositoryFactory = container.GetInstance<IAuthRepository>;

            //Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            // Set third-party login provider options
            googleAuthOptions = new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "620597621300-kl2s8olb1teeu5nrcmuitkvtn59i1ogu.apps.googleusercontent.com",
                ClientSecret = "AmTWc3UFoBBsD6HewS3VPtUL",
                Provider = new GoogleAuthProvider()
            };
            app.UseGoogleAuthentication(googleAuthOptions);

            //facebookAuthOptions = new FacebookAuthenticationOptions()
            //{
            //    AppId = "xxx",
            //    AppSecret = "xxx",
            //    Provider = new FacebookAuthProvider()
            //};
            //app.UseFacebookAuthentication(facebookAuthOptions);

            var oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new BlogAggregatorAuthorizationServerProvider(authRepositoryFactory),
                RefreshTokenProvider = new BlogAggregatorRefreshTokenProvider(authRepositoryFactory)
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }

        // Register containers for classes that use dependency injection
        private Container ConfigureSimpleInjector(IAppBuilder app)
        {
            var container = new Container();

            container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();

            container.Register<IUserStore<User, int>, UserStore>(Lifestyle.Scoped);
            container.Register<IAuthRepository, AuthRepository>(Lifestyle.Scoped);

            container.Register<IDatabaseFactory, DatabaseFactory>(Lifestyle.Scoped);

            container.Register<IUnitOfWork, UnitOfWork>();
          
            container.Register<IBlogRepository, BlogRepository>();
            container.Register<IPostRepository, PostRepository>();
            container.Register<IUserRepository, UserRepository>();
            container.Register<IExternalLoginRepository, ExternalLoginRepository>();

            container.Register<IBlogService, BlogService>();
            container.Register<IWordPressBlogReader, WordPressBlogReader>();

            app.Use(async (context, next) => {
                using (container.BeginExecutionContextScope())
                {
                    await next();
                }
            });

            container.Verify();

            return container;
        }
    }
}