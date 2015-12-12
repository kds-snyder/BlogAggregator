using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using BlogAggregator.Core.Repository;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Services;

namespace BlogAggregator.WebJob
{
    public class BlogPostUpdater
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlogService _blogService; 
          
        public BlogPostUpdater(IBlogRepository blogRepository, IPostRepository postRepository,
                                                    IUnitOfWork unitOfWork, IBlogService blogService)
        {
            _blogRepository = blogRepository;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _blogService = blogService;
        }

        [NoAutomaticTrigger]
        public void UpdateBlogPosts()
        {
            Console.WriteLine("Update blog posts");
            Console.ReadLine();
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        //public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        //{
        //    log.WriteLine(message);
        //}
    }
}
