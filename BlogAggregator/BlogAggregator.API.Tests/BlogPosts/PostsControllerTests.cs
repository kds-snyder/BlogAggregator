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

namespace BlogAggregator.API.Tests
{
    [TestClass]
    public class PostControllerTests
    {
        private Mock<IBlogRepository> _blogRepositoryMock;
        private Mock<IPostRepository> _postRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private PostsController _controller;
        private Blog[] _blogs;
        private Post[] _posts;

        // Numbers for tests
        private int _blogIDNoMockPosts = 4;
        private int _blogIDMockPosts = 5;
        private int _blogIDNonexistent = 9;
        private int _numberOfMockPosts = 3;
        private int _postIDFirst = 2;
        private int _postIDSecond = 3;
        private int _postIDThird = 4;
        private int _postIDFirstIndexInData = 0;
        private int _postIDSecondIndexInData = 1;
        private int _postIDThirdIndexInData = 2;
        private int _postIDNonexistent = 8;

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
            _blogRepositoryMock.Setup(b => b.Any(bl => bl.BlogID == _blogIDMockPosts)).Returns(true);
            _blogRepositoryMock.Setup(b => b.Any(bl => bl.BlogID == _blogIDNoMockPosts)).Returns(true);

            _postRepositoryMock.Setup(p => p.GetAll()).Returns(_posts.AsQueryable());
            _postRepositoryMock.Setup(p => p.GetByID(_postIDFirst)).Returns(_posts[_postIDFirstIndexInData]);
            _postRepositoryMock.Setup(p => p.GetByID(_postIDSecond)).Returns(_posts[_postIDSecondIndexInData]);
            _postRepositoryMock.Setup(p => p.GetByID(_postIDThird)).Returns(_posts[_postIDThirdIndexInData]);

            // Set up unit of work and controller
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _controller = new PostsController(_blogRepositoryMock.Object, _postRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [TestMethod]
        public void GetPostsReturnsPosts()
        {
            // Arrange

            // Act
            var content = _controller.GetPosts();

            // Assert
            // Verify that GetAll is called just once
            // Verify that the correct number of posts are returned
            _postRepositoryMock.Verify(p => p.GetAll(), Times.Once);
            Assert.AreEqual(content.Count(), _numberOfMockPosts);
        }

        [TestMethod]
        public void GetPostReturnsSameID()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult = _controller.GetPost(_postIDSecond);

            // Assert
            // Verify that GetByID is called just once
            // Verify that HTTP status code is Ok, with object of correct type
            _postRepositoryMock.Verify(p => p.GetByID(_postIDSecond), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<PostModel>));

            // Extract the content from the HTTP result
            // Verify the content is not null, and the IDs match
            var contentResult = actionResult as OkNegotiatedContentResult<PostModel>;
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(contentResult.Content.PostID, _postIDSecond);
        }

        [TestMethod]
        public void GetNonExistentPostReturnsNotFound()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult = _controller.GetPost(_postIDNonexistent);

            // Assert
            // Verify that GetByID is called just once
            // Verify that HTTP status code is NotFound
            _postRepositoryMock.Verify(p => p.GetByID(_postIDNonexistent), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PutPostReturnsHttpStatusCodeNoContent()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult =
                _controller.PutPost(
                    _postIDThird,
                     new PostModel
                     {
                         PostID = _postIDThird,
                         BlogID = _blogIDMockPosts,
                         Content = "Boring content",
                         Description = "Boring post",
                         Link = "http://testerson.wordpress.com/post/3",
                         PublicationDate = new DateTime(2015, 11, 5, 9, 55, 32),
                         Title = "Boring Title"
                     });
            var statusCodeResult = actionResult as StatusCodeResult;

            // Assert
            // Verify that:
            //  GetByID is called just once
            //  Update is called for Post object
            //  unit of work is committed once
            //  result of update is HTTP status code with no content
            _postRepositoryMock.Verify(p => p.GetByID(_postIDThird), Times.Once);
            _postRepositoryMock.Verify(p => p.Update(It.IsAny<Post>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(statusCodeResult.StatusCode, HttpStatusCode.NoContent);
        }

        [TestMethod]
        public void PutPostNonexistentPostIDReturnsNotFound()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult =
                _controller.PutPost(
                    _postIDNonexistent,
                     new PostModel
                     {
                         PostID = _postIDNonexistent,
                         BlogID = _blogIDMockPosts,
                         Content = "Boring content",
                         Description = "Boring post",
                         Link = "http://testerson.wordpress.com/post/x",
                         PublicationDate = new DateTime(2015, 11, 5, 9, 55, 32),
                         Title = "Boring Title"
                     });

            // Assert
            // Verify:
            //  GetByID is called once
            //  Result is NotFound
            _postRepositoryMock.Verify(p => p.GetByID(_postIDNonexistent), Times.Once);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PostPostAddsPost()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult =
                _controller.PostPost(
                    new PostModel
                    {
                        PostID = 0,
                        BlogID = _blogIDMockPosts,
                        Content = "New content",
                        Description = "New post",
                        Link = "http://testerson.wordpress.com/post/n",
                        PublicationDate = new DateTime(2015, 11, 5, 9, 55, 32),
                        Title = "New Title"
                    });

            // Assert
            // Verify:
            //  Add is called just once with Post object
            //  Unit of work is committed just once
            //  HTTP result is CreatedAtRouteNegotiatedContentResult
            //  Location header is set in created result
            _postRepositoryMock.Verify(p => p.Add(It.IsAny<Post>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsInstanceOfType
                    (actionResult, typeof(CreatedAtRouteNegotiatedContentResult<PostModel>));
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<PostModel>;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(createdResult.RouteName, "DefaultApi");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception),
            "Unable to add the post to the database, as it does not correspond to a blog")]
        public void PostPostNonExistentBlogIdThrowsException()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult =
                _controller.PostPost(
                    new PostModel
                    {
                        PostID = 0,
                        BlogID = _blogIDNonexistent,
                        Content = "Content",
                        Description = "Post",
                        Link = "http://testerson.wordpress.com/post/z",
                        PublicationDate = new DateTime(2015, 11, 5, 9, 55, 32),
                        Title = "Title"
                    });

            // Assert
            // Test fails if it reaches here, as API method should throw exception
            Assert.Fail();
        }

        [TestMethod]
        public void DeletePostReturnsOk()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult = _controller.DeletePost(_postIDFirst);

            // Assert
            // Verify:
            //  GetByID is called once
            //  Delete is called once with correct object
            //  Unit of work commit is called once
            //  Result is OK, and content result ID matches
            _postRepositoryMock.Verify(p => p.GetByID(_postIDFirst), Times.Once);
            _postRepositoryMock.Verify(p => p.Delete(_posts[_postIDFirstIndexInData]), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<PostModel>));
            var contentResult = actionResult as OkNegotiatedContentResult<PostModel>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsTrue(contentResult.Content.PostID == _postIDFirst);           
        }

        [TestMethod]
        public void DeleteNonExistentPostReturnsNotFound()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult = _controller.DeletePost(_postIDNonexistent);

            // Assert
            // Verify that GetByID is called once
            // Verify that result is NotFound
            _postRepositoryMock.Verify(p => p.GetByID(_postIDNonexistent), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }
    }
}
