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
            try
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
                Console.WriteLine("Completed SaveNewBlogPosts");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occurred: {0}\nSource: {1}\nStackTrace: {2}", 
                                    e.Message, e.Source, e.StackTrace);
                if (e.InnerException != null)
                {
                    Console.WriteLine("InnerException: {0}\n{1}",
                                e.InnerException, e.InnerException.StackTrace);
               }              
                throw;
            }
            // Console.ReadLine commented out as it causes WebJob to fail with timeout error although SaveNewBlogPosts is successful
            //Console.ReadLine();
        }

        // Configure Simple Injector dependencies
        private static Container configureSimpleInjector()
        {
            var container = new Container();

            container.Register<IDatabaseFactory, DatabaseFactory>(Lifestyle.Singleton);

            container.Register<IBlogRepository, BlogRepository>();
            container.Register<IPostRepository, PostRepository>();
            container.Register<IUnitOfWork, UnitOfWork>();

            container.Register<IBlogService, BlogService>();
            container.Register<IWordPressBlogReader, WordPressBlogReader>();

            container.Verify();

            return container;
        }
    }
}
