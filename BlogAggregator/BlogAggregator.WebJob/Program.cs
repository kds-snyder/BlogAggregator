using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using SimpleInjector;
using BlogAggregator.Core.Services;
using BlogAggregator.Core.BlogReader.WordPress;
using BlogAggregator.Data.Infrastructure;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Repository;
using BlogAggregator.Data.Repository;
using SimpleInjector.Extensions.ExecutionContextScoping;

namespace BlogAggregator.WebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            // Configure SimpleInjector                     
            Container container = configureSimpleInjector();

            // Configure JobHost.
            var jobHostConfiguration = new JobHostConfiguration
            {
                JobActivator = new BlogAggregatorJobActivator(container)
            };
            var jobHost = new JobHost(jobHostConfiguration);

            // Call the scheduled blog post updating method
            Console.WriteLine("Calling SaveNewBlogPosts");
            jobHost.Call(typeof(Functions).GetMethod("SaveNewBlogPosts"));
            Console.ReadLine();
        }

        // Configure Simple Injector dependencies
        private static Container configureSimpleInjector()
        {
            var container = new Container();

            container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();
            container.Register<IDatabaseFactory, DatabaseFactory>(Lifestyle.Scoped);

            //container.RegisterSingleton<IBlogRepository, BlogRepository>();
            //container.RegisterSingleton<IPostRepository, PostRepository>();
            //container.RegisterSingleton<IUnitOfWork, UnitOfWork>();

            container.Register<IBlogRepository, BlogRepository>(Lifestyle.Singleton);
            container.Register<IPostRepository, PostRepository>(Lifestyle.Singleton);
            container.Register<IUnitOfWork, UnitOfWork>(Lifestyle.Singleton);

            //container.Register<IBlogRepository, BlogRepository>(Lifestyle.Scoped);
            //container.Register<IPostRepository, PostRepository>(Lifestyle.Scoped);
            //container.Register<IUnitOfWork, UnitOfWork>(Lifestyle.Scoped);

            //container.Register<IBlogRepository, BlogRepository>();
            //container.Register<IPostRepository, PostRepository>();
            //container.Register<IUnitOfWork, UnitOfWork>();

            container.Verify();

            return container;
        }
    }
}
