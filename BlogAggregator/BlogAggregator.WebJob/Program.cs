using System;
using SimpleInjector;
using BlogAggregator.Core.Services;
using BlogAggregator.Core.BlogReader.WordPress;
using BlogAggregator.Data.Infrastructure;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Repository;
using BlogAggregator.Data.Repository;

namespace BlogAggregator.WebJob
{   
    class Program
    {
        static void Main()
        {
            try
            {
                // Configure SimpleInjector                     
                Container container = configureSimpleInjector();

                // Call the method to save new blog posts
                Console.WriteLine("Calling ExtractAndSaveAllNewBlogPosts");
                IBlogService blogService = container.GetInstance<BlogService>();
                blogService.ExtractAndSaveAllNewBlogPosts();
                Console.WriteLine("Completed ExtractAndSaveAllNewBlogPosts");
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
            finally
            {
                // Comment out when deploying
                //Console.ReadLine();
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

            container.Verify();

            return container;
        }
    }
}
