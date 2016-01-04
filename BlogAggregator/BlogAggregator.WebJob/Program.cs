using System;
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

                // Console.ReadLine must be commented out when deployed as it causes WebJob to fail with timeout error
                //Console.ReadLine();
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
                // Console.ReadLine must be commented out when deployed as it causes WebJob to fail with timeout error
                //Console.ReadLine();
                throw;
            }           
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

            //container.Register<ILogger, Logger>();

            container.Verify();

            return container;
        }
    }
}
