using Microsoft.Azure.WebJobs;
using BlogAggregator.Core.Repository;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Services;

namespace BlogAggregator.WebJob
{
    public class Functions
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Functions(IBlogRepository blogRepository, IPostRepository postRepository,
                                      IUnitOfWork unitOfWork)

        {
            _blogRepository = blogRepository;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
       }

        [NoAutomaticTrigger]
        // Save new blog posts in Post table for approved blogs
        public void SaveNewBlogPosts()
        {
            var blogService = new BlogService(_blogRepository, _postRepository,
                                                        _unitOfWork);
            blogService.ExtractAndSaveAllNewBlogPosts();
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        //public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        //{
        //    log.WriteLine(message);
        //}
    }
}
