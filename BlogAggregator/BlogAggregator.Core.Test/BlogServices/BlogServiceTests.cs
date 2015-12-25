using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlogAggregator.Core.Repository;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Services;
using BlogAggregator.Core.BlogReader.WordPress;
using Moq;
using BlogAggregator.API;
using BlogAggregator.Core.Domain;
using System.Linq;
using System.Collections.Generic;
using BlogAggregator.Core.Models;
using BlogAggregator.Core.BlogReader;

namespace BlogAggregator.Core.Test.BlogServices
{
    [TestClass]
    public class BlogServiceTests
    {
        private Mock<IBlogRepository> _blogRepositoryMock;
        private Mock<IPostRepository> _postRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;        
        private Mock<IWordPressBlogReader> _wordPressBlogReaderMock;
        private BlogService _blogService;

        private Blog[] _blogs;
        private Blog _blogA;
        private Blog _blogB;       
        private Post[] _posts;

        // Numbers for tests
        private int _blogIDAWithPosts = 1;
        private int _blogIDBWithPosts = 2;
        private int _blogIDAWithPostsIndexInArray = 0;
        private int _blogIDBWithPostsIndexInArray= 1;

        private int _postIDBlogIDAFirst = 2;
        private int _postIDBlogIDASecond = 3;
        private int _postIDBlogIDAThird = 4;
        private int _postIDBlogIDAFirstIndexInArray = 0;
        private int _postIDBlogIDASecondIndexInArray = 1;
        private int _postIDBlogIDAThirdIndexInArray = 2;
        private int _numBlogAPosts = 3;

        private int _postIDBlogIDBFirst = 7;
        private int _postIDBlogIDBSecond = 8;
        private int _postIDBlogIDBFirstIndexInArray = 3;
        private int _postIDBlogIDBSecondIndexInArray = 4;

        [TestInitialize]
        public void Initialize()
        {

            // Set up Automapper
            WebApiConfig.CreateMaps();

            // Set up repositories 
            _blogRepositoryMock = new Mock<IBlogRepository>();
            _postRepositoryMock = new Mock<IPostRepository>();

            // Set data for repositories

            _blogA = new Blog
                {
                    BlogID = _blogIDAWithPosts,
                    BlogType = BlogTypes.WordPress,
                    CreatedDate = DateTime.Now,
                    Approved = true,
                    AuthorEmail = "test@test.com",
                    AuthorName = "Testy McTesterson",
                    Description = "Testing",
                    Link = "http://testy.wordpress.com",
                    Title = "Testy's Blog"
                };

            _blogB = new Blog
            {
                BlogID = _blogIDBWithPosts,
                BlogType = BlogTypes.WordPress,
                CreatedDate = DateTime.Now,
                Approved = true,
                AuthorEmail = "unit@unit.com",
                AuthorName = "Unit Testerson",
                Description = "Unit Testing",
                Link = "http://testerson.wordpress.com",
                Title = "Testerson's Blog"
            };

            _blogs = new[]
            {
                _blogA,
                _blogB
            };

            _posts = new[]
     {
                new Post {
                    PostID = _postIDBlogIDAFirst,
                    BlogID = _blogIDAWithPosts,
                    Blog = _blogs[_blogIDAWithPostsIndexInArray],
                    Content = "Test content",
                    Description = "Interesting post",
                    Link = "http://testy.wordpress.com/post/1",
                    PublicationDate = DateTime.Now,
                    Title = "Interesting Title"
               },
                new Post {
                    PostID = _postIDBlogIDASecond,
                    BlogID = _blogIDAWithPosts,
                    Blog = _blogs[_blogIDAWithPostsIndexInArray],
                    Content = "More test content",
                    Description = "More interesting post",
                    Link = "http://testy.wordpress.com/post/2",
                    PublicationDate = DateTime.Now,
                    Title = "More Interesting Title"
                },
                new Post {
                    PostID = _postIDBlogIDAThird,
                    BlogID = _blogIDAWithPosts,
                    Blog = _blogs[_blogIDAWithPostsIndexInArray],
                    Content = "Even more test content",
                    Description = "Even more interesting post",
                    Link = "http://testy.wordpress.com/post/3",
                    PublicationDate = DateTime.Now,
                    Title = "Even More Interesting Title"
                }
            };

            _blogRepositoryMock.Setup(br => br.GetAll()).Returns(_blogs.AsQueryable());
            _blogRepositoryMock.Setup(br => br.GetByID(_blogIDAWithPosts)).Returns(_blogs[_blogIDAWithPostsIndexInArray]);
            _blogRepositoryMock.Setup(br => br.GetByID(_blogIDBWithPosts)).Returns(_blogs[_blogIDBWithPostsIndexInArray]);
 
            _postRepositoryMock.Setup(pr => pr.GetAll()).Returns(_posts.AsQueryable());
            _postRepositoryMock.Setup(pr => pr.GetByID(_postIDBlogIDAFirst)).Returns(_posts[_postIDBlogIDAFirstIndexInArray]);
            _postRepositoryMock.Setup(pr => pr.GetByID(_postIDBlogIDASecond)).Returns(_posts[_postIDBlogIDASecondIndexInArray]);
            _postRepositoryMock.Setup(pr => pr.GetByID(_postIDBlogIDAThird)).Returns(_posts[_postIDBlogIDAThirdIndexInArray]);

            // Set up unit of work and services
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _wordPressBlogReaderMock = new Mock<IWordPressBlogReader>();
            _blogService = new BlogService(_blogRepositoryMock.Object, _postRepositoryMock.Object,
                                                _unitOfWorkMock.Object, _wordPressBlogReaderMock.Object);

        }


        [TestMethod]
        public void ExtractBlogPostsReturnsBlogPosts()
        {
            // Arrange           
            var _blogAModel = new BlogModel
            {
                BlogID = _blogA.BlogID,
                BlogType = _blogA.BlogType,
                CreatedDate = _blogA.CreatedDate,
                Approved = _blogA.Approved,
                AuthorEmail = _blogA.AuthorEmail,
                AuthorName = _blogA.AuthorName,
                Description = _blogA.Description,
                Link = _blogA.Link,
                Title = _blogA.Title
            };
            BlogInfo blogInfo = new BlogInfo
            {
                Description = _blogA.Description,
                Title = _blogA.Title
            };
            _wordPressBlogReaderMock.Setup(m => m.VerifyBlog(It.IsAny<string>())).Returns(blogInfo);
            _wordPressBlogReaderMock.Setup(wp => wp.GetBlogPosts(It.IsAny<string>())).Returns(_posts.AsQueryable());

            // Act
            //blogService.ExtractAndSaveAllNewBlogPosts();
            IEnumerable<Post> posts = _blogService.ExtractBlogPosts(_blogAModel);

            // Assert
            Assert.AreEqual(posts.Count(), _numBlogAPosts);
        }
    }
}
