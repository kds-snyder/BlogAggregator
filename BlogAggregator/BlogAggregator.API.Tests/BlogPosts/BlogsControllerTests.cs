using BlogAggregator.Core.Repository;
using System;
using System.Linq;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.API.Controllers;
using BlogAggregator.Core.Domain;
using System.Web.Http.Results;
using BlogAggregator.Core.Models;
using System.Net;
using BlogAggregator.Core.Services;

namespace BlogAggregator.API.Tests.BlogPosts
{
    [TestClass]
    public class BlogsControllerTests
    {
        private Mock<IBlogRepository> _blogRepositoryMock;
        private Mock<IPostRepository> _postRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IBlogService> _blogServiceMock;
        private BlogsController _controller;
        private Blog[] _blogs;
        private Post[] _posts;

        // Numbers for tests
        private int _blogIDNoMockPosts = 4;
        private int _blogIDMockPosts = 5;
        private int _numberOfMockBlogs = 2;
        private int _blogIDNoMockPostsIndexInData = 0;
        private int _blogIDMockPostsIndexInData = 1;
        private int _blogIDNonexistent = 9;       
        private int _postIDFirst = 2;
        private int _postIDSecond = 3;
        private int _postIDThird = 4;
        private int _postIDFirstIndexInData = 0;
        private int _postIDSecondIndexInData = 1;
        private int _postIDThirdIndexInData = 2;

        [TestInitialize]
        public void Initialize()
        {
            // Set up Automapper
            WebApiConfig.CreateMaps();

            // Set up repositories
            _blogRepositoryMock = new Mock<IBlogRepository>();
            _postRepositoryMock = new Mock<IPostRepository>();

            // Set data in repositories
            _blogs = new[]
            {
                new Blog {
                    BlogID = _blogIDNoMockPosts,
                    //BlogType = BlogTypes.WordPress,
                    CreatedDate = new DateTime(2015, 12, 2, 14, 55, 32),
                    Approved = true,
                    AuthorEmail = "test@test.com",
                    AuthorName = "Testy McTesterson",
                    Description = "Testing",
                    Link = "http://testy.wordpress.com",
                    Title = "Testy's Blog"
                },
                new Blog {
                    BlogID = _blogIDMockPosts,
                    //BlogType = BlogTypes.WordPress,
                    CreatedDate = new DateTime(2015, 12, 3, 10, 55, 32),
                    Approved = true,
                    AuthorEmail = "unit@unit.com",
                    AuthorName = "Unit Testerson",
                    Description = "Unit Testing",
                    Link = "http://testerson.wordpress.com",
                    Title = "Testerson's Blog"
                }
            };

            _posts = new[]
            {
                new Post {
                    PostID = _postIDFirst,
                    BlogID = _blogIDMockPosts,
                    Content = "Test content",
                    Description = "Interesting post",
                    Link = "http://testerson.wordpress.com/post/1",
                    PublicationDate = new DateTime(2015, 11, 2, 9, 55, 32),
                    Title = "Interesting Title"
               },
                new Post {
                    PostID = _postIDSecond,
                    BlogID = _blogIDMockPosts,
                    Content = "More test content",
                    Description = "More interesting post",
                    Link = "http://testerson.wordpress.com/post/2",
                    PublicationDate = new DateTime(2015, 11, 3, 9, 55, 32),
                    Title = "More Interesting Title"
                },
                new Post {
                    PostID = _postIDThird,
                    BlogID = _blogIDMockPosts,
                    Content = "Even more test content",
                    Description = "Even more interesting post",
                    Link = "http://testerson.wordpress.com/post/3",
                    PublicationDate = new DateTime(2015, 11, 5, 9, 55, 32),
                    Title = "Even More Interesting Title"
                }
            };

            _blogRepositoryMock.Setup(b => b.GetAll()).Returns(_blogs.AsQueryable());
            _blogRepositoryMock.Setup(b => b.GetByID(_blogIDMockPosts)).Returns(_blogs[_blogIDMockPostsIndexInData]);
            _blogRepositoryMock.Setup(b => b.GetByID(_blogIDNoMockPosts)).Returns(_blogs[_blogIDNoMockPostsIndexInData]);

            _postRepositoryMock.Setup(p => p.GetAll()).Returns(_posts.AsQueryable());
            _postRepositoryMock.Setup(p => p.GetByID(_postIDFirst)).Returns(_posts[_postIDFirstIndexInData]);
            _postRepositoryMock.Setup(p => p.GetByID(_postIDSecond)).Returns(_posts[_postIDSecondIndexInData]);
            _postRepositoryMock.Setup(p => p.GetByID(_postIDThird)).Returns(_posts[_postIDThirdIndexInData]);

            // Set up unit of work and controller
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _blogServiceMock = new Mock<IBlogService>();
            _controller = new BlogsController(_blogRepositoryMock.Object, _postRepositoryMock.Object, 
                                                _unitOfWorkMock.Object, _blogServiceMock.Object);
        }

        [TestMethod]
        public void GetBlogsReturnsBlogs()
        {
            // Arrange

            // Act
            var content = _controller.GetBlogs();

            // Assert
            // Verify that GetAll is called just once
            // Verify that the correct number of blogs are returned
            _blogRepositoryMock.Verify(p => p.GetAll(), Times.Once);
            Assert.AreEqual(content.Count(), _numberOfMockBlogs);
        }

        [TestMethod]
        public void GetBlogReturnsSameID()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult = _controller.GetBlog(_blogIDNoMockPosts);

            // Assert
            // Verify that GetByID is called just once
            // Verify that HTTP status code is Ok, with object of correct type
            _blogRepositoryMock.Verify(p => p.GetByID(_blogIDNoMockPosts), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<BlogModel>));

            // Extract the content from the HTTP result
            // Verify the content is not null, and the IDs match
            var contentResult = actionResult as OkNegotiatedContentResult<BlogModel>;
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(contentResult.Content.BlogID, _blogIDNoMockPosts);
        }

        [TestMethod]
        public void GetNonExistentBlogReturnsNotFound()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult = _controller.GetBlog(_blogIDNonexistent);

            // Assert
            // Verify that GetByID is called just once
            // Verify that HTTP status code is NotFound
            _blogRepositoryMock.Verify(p => p.GetByID(_blogIDNonexistent), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PutBlogNoPostsNotApprovedReturnsHttpStatusCodeNoContent()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult =
                _controller.PutBlog(
                    _blogIDNoMockPosts,
                     new BlogModel
                     {
                         BlogID = _blogIDNoMockPosts,
                         //BlogType = BlogTypes.WordPress,
                         CreatedDate = new DateTime(2015, 12, 2, 14, 55, 32),
                         Approved = false,
                         AuthorEmail = "test@test.com",
                         AuthorName = "Testy McTesterson",
                         Description = "Testing",
                         Link = "http://testy.wordpress.com",
                         Title = "Testy's Blog"
                     });
            var statusCodeResult = actionResult as StatusCodeResult;

            // Assert
            // Verify that:
            //  GetByID is called just once
            //  Update is called for Blog object
            //  unit of work is committed once
            //  result of update is HTTP status code with no content
            _blogRepositoryMock.Verify(b => b.GetByID(_blogIDNoMockPosts), Times.Once);
            _blogRepositoryMock.Verify(b => b.Update(It.IsAny<Blog>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(statusCodeResult.StatusCode, HttpStatusCode.NoContent);
        }

        [TestMethod]
        public void PutBlogNonexistentBlogIDReturnsNotFound()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult =
                _controller.PutBlog(
                    _blogIDNonexistent,
                     new BlogModel
                     {
                         BlogID = _blogIDNonexistent,
                         //BlogType = BlogTypes.WordPress,
                         CreatedDate = new DateTime(2015, 12, 2, 14, 55, 32),
                         Approved = false,
                         AuthorEmail = "test@test.com",
                         AuthorName = "Testy McTesterson",
                         Description = "Testing",
                         Link = "http://testy.wordpress.com",
                         Title = "Testy's Blog"
                     });

            // Assert
            // Verify:
            //  GetByID is called once
            //  Result is NotFound
            _blogRepositoryMock.Verify(p => p.GetByID(_blogIDNonexistent), Times.Once);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PostBlogNotApprovedAddsBlog()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult =
                _controller.PostBlog(
                    new BlogModel
                    {
                        BlogID = 0,
                        //BlogType = BlogTypes.WordPress,
                        CreatedDate = new DateTime(2015, 12, 2, 14, 55, 32),
                        Approved = false,
                        AuthorEmail = "test@test.com",
                        AuthorName = "TestNew McTesterson",
                        Description = "New Testing",
                        Link = "http://testy.wordpress.com",
                        Title = "New Testy's Blog"
                    });

            // Assert
            // Verify:
            //  Add is called just once with Blog object
            //  Unit of work is committed just once
            //  HTTP result is CreatedAtRouteNegotiatedContentResult
            //  Location header is set in created result
            _blogRepositoryMock.Verify(p => p.Add(It.IsAny<Blog>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsInstanceOfType
                    (actionResult, typeof(CreatedAtRouteNegotiatedContentResult<BlogModel>));
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<BlogModel>;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(createdResult.RouteName, "DefaultApi");
        }

        [TestMethod]
        public void DeleteBlogNoPostsReturnsOk()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult = _controller.DeleteBlog(_blogIDNoMockPosts);

            // Assert
            // Verify:
            //  GetByID is called once
            //  Delete is called once with correct object
            //  Unit of work commit is called once
            //  Result is OK, and content result ID matches
            _blogRepositoryMock.Verify(p => p.GetByID(_blogIDNoMockPosts), Times.Once);
            _blogRepositoryMock.Verify(p => p.Delete(_blogs[_blogIDNoMockPostsIndexInData]), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<BlogModel>));
            var contentResult = actionResult as OkNegotiatedContentResult<BlogModel>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsTrue(contentResult.Content.BlogID == _blogIDNoMockPosts);
        }

        [TestMethod]
        public void DeleteNonExistentBlogReturnsNotFound()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult = _controller.DeleteBlog(_blogIDNonexistent);

            // Assert
            // Verify that GetByID is called once
            // Verify that result is NotFound
            _blogRepositoryMock.Verify(p => p.GetByID(_blogIDNonexistent), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }
    }
}
