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
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.BlogReader.WordPress;

namespace BlogAggregator.WebJob
{
    public class BlogPostUpdater
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlogService _blogService;
        private readonly IWordPressBlogReader _wordPressBlogReader;
          
        public BlogPostUpdater(IBlogRepository blogRepository, IPostRepository postRepository,
                                      IUnitOfWork unitOfWork, IBlogService blogService, 
                                                    IWordPressBlogReader wordPressBlogReader)
        {
            _blogRepository = blogRepository;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _blogService = blogService;
            _wordPressBlogReader = wordPressBlogReader;
        }

        [NoAutomaticTrigger]
        // Update blog posts in Post table for approved blogs
        public void UpdateBlogPosts()
        {
            IQueryable<Blog> blogs = _blogRepository.GetAll();

            foreach (var blog in blogs)
            {               
                var wordPressBlogReader = new WordPressBlogReader();
                var blogService = new BlogService(wordPressBlogReader, _postRepository, _unitOfWork);
               // var blogPosts = _blogService.ExtractBlogPosts(blog);
            }
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        //public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        //{
        //    log.WriteLine(message);
        //}
    }
}
