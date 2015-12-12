using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlogAggregator.Core.Repository;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Services;
using BlogAggregator.Core.BlogReader.WordPress;

namespace BlogAggregator.Core.Test.BlogServices
{
    [TestClass]
    public class BlogServiceTests
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlogService _blogService;
        private readonly IWordPressBlogReader _wordPressBlogReader;

        public BlogServiceTests(IBlogRepository blogRepository, IPostRepository postRepository,
                                        IUnitOfWork unitOfWork, IBlogService blogService,
                                        IWordPressBlogReader wordPressBlogReader)
        {
            _blogRepository = blogRepository;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _blogService = blogService;
            _wordPressBlogReader = wordPressBlogReader;
        }

        [TestMethod]
        public void ExtractAndSaveAllNewBlogPostsSavesAllNewBlogPosts()
        {
            // Arrange
            int countPosts = _postRepository.Count();

            // Act
            var blogService = new BlogService(_blogRepository, _postRepository,
                                                        _unitOfWork, _wordPressBlogReader);
            blogService.ExtractAndSaveAllNewBlogPosts();

            // Assert
            Assert.IsTrue(_postRepository.Count() > countPosts);

        }
    }
}
