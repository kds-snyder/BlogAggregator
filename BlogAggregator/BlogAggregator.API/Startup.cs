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
using NLog;
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
        public static GoogleOAuth2AuthenticationOptions GoogleAuthOptions { get; private set; }
        public static FacebookAuthenticationOptions FacebookAuthOptions { get; private set; }

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

            var oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new BlogAggregatorAuthorizationServerProvider(authRepositoryFactory),
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            // Set third-party login provider options
            GoogleAuthOptions = new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "77265186922-n2f9s9pfs6qi90pm9arl6vo1r0hjeatd.apps.googleusercontent.com",
                ClientSecret = "Xp6BVnYNweq9ceha-vnddFs-",
                Provider = new GoogleAuthProvider()
            };
            app.UseGoogleAuthentication(GoogleAuthOptions);

            //FacebookAuthOptions = new FacebookAuthenticationOptions()
            //{
            //    AppId = "xxx",
            //    AppSecret = "xxx",
            //    Provider = new FacebookAuthProvider()
            //};
            //app.UseFacebookAuthentication(FacebookAuthOptions);                     
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

            //container.RegisterConditional(
            //    typeof(ILogger),
            //    c => typeof(Logger).MakeGenericType(c.Consumer.ImplementationType),
            //                  Lifestyle.Transient,
            //                    c => true);

            app.Use(async (context, next) =>
            {
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