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

namespace BlogAggregator.WebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {

            // Configure SimpleInjector.
            var container = new Container();
            configureSimpleInjector(container);
            
            // Configure JobHost.
            var jobHostConfiguration = new JobHostConfiguration
            {
                JobActivator = new BlogAggregatorJobActivator(container)
            };
            var jobHost = new JobHost(jobHostConfiguration);
            
            // The following code ensures that the WebJob will be running continuously
            //jobHost.Call(UpdateBlogPosts);
        }

        // Configure Simple Injector dependencies
        private static void configureSimpleInjector(Container container)
        {
 
            container.Register<IDatabaseFactory, DatabaseFactory>(Lifestyle.Scoped);

            container.Register<IUnitOfWork, UnitOfWork>();

            container.Register<IBlogRepository, BlogRepository>();
            container.Register<IPostRepository, PostRepository>();

            container.Register<IBlogService, BlogService>();
            container.Register<IWordPressBlogReader, WordPressBlogReader>();
        }
    }
}
