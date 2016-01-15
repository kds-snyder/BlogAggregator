using Microsoft.Azure.WebJobs;
using BlogAggregator.Core.Services;

namespace BlogAggregator.WebJob
{
    public class Functions
    {       
        private readonly IBlogService _blogService;       

        public Functions(IBlogService blogService)

        {
            _blogService = blogService;
       }

        [NoAutomaticTrigger]
        // Save new blog posts in Post table for approved blogs
        public void SaveNewBlogPosts()
        {            
            _blogService.ExtractAndSaveAllNewBlogPosts();
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        //public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        //{
        //    log.WriteLine(message);
        //}
    }
}
