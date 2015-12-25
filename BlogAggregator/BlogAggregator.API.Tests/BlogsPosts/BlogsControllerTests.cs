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
using System.Linq.Expressions;
using BlogAggregator.Core.BlogReader.WordPress;
using BlogAggregator.Core.BlogReader;

namespace BlogAggregator.API.Tests.BlogPosts
{
    [TestClass]
    public class BlogsControllerTests
    {
        private Mock<IBlogRepository> _blogRepositoryMock;
        private Mock<IPostRepository> _postRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IBlogService> _blogServiceMock;
        private Mock<IWordPressBlogReader> _wordPressBlogReaderMock;
        private BlogsController _controller;
        private Blog[] _blogs;
        private Post[] _posts;
        private User[] _users;

        // Numbers for tests
        private int _blogIDApprovedNoMockPosts = 4;
        private int _blogIDApprovedMockPosts = 5;
        private int _blogIDNotApproved = 6;
        private int _numberOfMockBlogs = 3;
        private int _blogIDApprovedNoMockPostsIndexInArray = 0;
        private int _blogIDApprovedMockPostsIndexInArray = 1;
        private int _blogIDNotApprovedIndexInArray = 2;
        private int _blogIDNonexistent = 9;
        private int _postIDFirst = 2;
        private int _postIDSecond = 3;
        private int _postIDThird = 4;
        private int _numberOfMockPosts = 3;
        private int _postIDFirstIndexInArray = 0;
        private int _postIDSecondIndexInArray = 1;
        private int _postIDThirdIndexInArray = 2;
        private int _userAuthorizedIndexInArray = 0;
        private int _userUnauthorizedIndexInArray = 1;

        [TestInitialize]
        public void Initialize()
        {
            // Set up Automapper
            WebApiConfig.CreateMaps();

            // Set up repositories
            _blogRepositoryMock = new Mock<IBlogRepository>();
            _postRepositoryMock = new Mock<IPostRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();

            // Set data in repositories
            _blogs = new[]
            {
                new Blog {
                    BlogID = _blogIDApprovedNoMockPosts,
                    BlogType = BlogTypes.WordPress,
                    CreatedDate = new DateTime(2015, 12, 2, 14, 55, 32),
                    Approved = true,
                    AuthorEmail = "test@test.com",
                    AuthorName = "Testy McTesterson",
                    Description = "Testing",
                    Link = "http://testy.wordpress.com",
                    Title = "Testy's Blog"
                },
                new Blog {
                    BlogID = _blogIDApprovedMockPosts,
                    BlogType = BlogTypes.WordPress,
                    CreatedDate = new DateTime(2015, 12, 3, 10, 55, 32),
                    Approved = true,
                    AuthorEmail = "unit@unit.com",
                    AuthorName = "Unit Testerson",
                    Description = "Unit Testing",
                    Link = "http://testerson.wordpress.com",
                    Title = "Testerson's Blog"
                },
                new Blog {
                    BlogID = _blogIDNotApproved,
                    BlogType = BlogTypes.WordPress,
                    CreatedDate = new DateTime(2015, 12, 1, 10, 55, 32),
                    Approved = false,
                    AuthorEmail = "k@s.com",
                    AuthorName = "KDS",
                    Description = "Great Blog",
                    Link = "kdstestyxxxcom",
                    Title = "KDS Blog"
                }
            };

            _posts = new[]
            {
                new Post {
                    PostID = _postIDFirst,
                    BlogID = _blogIDApprovedMockPosts,
                    Blog = _blogs[_blogIDApprovedMockPostsIndexInArray],
                    Content = "Test content",
                    Description = "Interesting post",
                    Link = "http://testerson.wordpress.com/post/1",
                    PublicationDate = new DateTime(2015, 11, 2, 9, 55, 32),
                    Title = "Interesting Title"
               },
                new Post {
                    PostID = _postIDSecond,
                    BlogID = _blogIDApprovedMockPosts,
                    Blog = _blogs[_blogIDApprovedMockPostsIndexInArray],
                    Content = "More test content",
                    Description = "More interesting post",
                    Link = "http://testerson.wordpress.com/post/2",
                    PublicationDate = new DateTime(2015, 11, 3, 9, 55, 32),
                    Title = "More Interesting Title"
                },
                new Post {
                    PostID = _postIDThird,
                    BlogID = _blogIDApprovedMockPosts,
                    Blog = _blogs[_blogIDApprovedMockPostsIndexInArray],
                    Content = "Even more test content",
                    Description = "Even more interesting post",
                    Link = "http://testerson.wordpress.com/post/3",
                    PublicationDate = new DateTime(2015, 11, 5, 9, 55, 32),
                    Title = "Even More Interesting Title"
                }
            };

            // Set up users: first is authorized, second is unauthorized
            _users = new[] 
            {
                new User { 
                Id = 1,
                Authorized = true,
                PasswordHash = "XXX",
                SecurityStamp = "YYY",
                UserName = "userAuthorized"
                },
                 new User {
                Id = 2,
                Authorized = false,
                PasswordHash = "XXX",
                SecurityStamp = "YYY",
                UserName = "userUnauthorized"
                }
            };

            _blogRepositoryMock.Setup(br => br.GetAll()).Returns(_blogs.AsQueryable());
            _blogRepositoryMock.Setup(br => br.GetByID(_blogIDApprovedMockPosts)).Returns(_blogs[_blogIDApprovedMockPostsIndexInArray]);
            _blogRepositoryMock.Setup(br => br.GetByID(_blogIDApprovedNoMockPosts)).Returns(_blogs[_blogIDApprovedNoMockPostsIndexInArray]);
            _blogRepositoryMock.Setup(br => br.GetByID(_blogIDNotApproved)).Returns(_blogs[_blogIDNotApprovedIndexInArray]);

            _postRepositoryMock.Setup(pr => pr.GetAll()).Returns(_posts.AsQueryable());
             _postRepositoryMock.Setup(pr => pr.GetByID(_postIDFirst)).Returns(_posts[_postIDFirstIndexInArray]);
            _postRepositoryMock.Setup(pr => pr.GetByID(_postIDSecond)).Returns(_posts[_postIDSecondIndexInArray]);
            _postRepositoryMock.Setup(pr => pr.GetByID(_postIDThird)).Returns(_posts[_postIDThirdIndexInArray]); 
                          
            // Set up unit of work and controller
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _blogServiceMock = new Mock<IBlogService>();
            _wordPressBlogReaderMock = new Mock<IWordPressBlogReader>();
            _controller = new BlogsController(_blogRepositoryMock.Object, _postRepositoryMock.Object,
                                                _userRepositoryMock.Object,
                                                _unitOfWorkMock.Object, _blogServiceMock.Object,
                                                 _wordPressBlogReaderMock.Object);
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
            IHttpActionResult actionResult = _controller.GetBlog(_blogIDApprovedNoMockPosts);

            // Assert
            // Verify that GetByID is called just once
            // Verify that HTTP status code is Ok, with object of correct type
            _blogRepositoryMock.Verify(p => p.GetByID(_blogIDApprovedNoMockPosts), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<BlogModel>));

            // Extract the content from the HTTP result
            // Verify the content is not null, and the IDs match
            var contentResult = actionResult as OkNegotiatedContentResult<BlogModel>;
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(contentResult.Content.BlogID, _blogIDApprovedNoMockPosts);
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
        public void PutBlogUnauthorizedUserReturnsUnauthorized()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                       .Returns(_users[_userUnauthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult =
                _controller.PutBlog(
                    _blogIDApprovedNoMockPosts,
                     new BlogModel
                     {
                         BlogID = _blogIDApprovedNoMockPosts,
                         BlogType = BlogTypes.WordPress,
                         CreatedDate = new DateTime(2015, 12, 2, 14, 55, 32),
                         Approved = false,
                         AuthorEmail = "test@test.com",
                         AuthorName = "Testy McTesterson",
                         Description = "Testing",
                         Link = "http://testy.wordpress.com",
                         Title = "Testy's Blog"
                     });
 
            // Assert
            // Verify that HTTP status code result of update is unauthorized
            Assert.IsInstanceOfType(actionResult, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void PutBlogNoPostsApprovedChangedtoFalseReturnsHttpStatusCodeNoContent()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                       .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult =
                _controller.PutBlog(
                    _blogIDApprovedNoMockPosts,
                     new BlogModel
                     {
                         BlogID = _blogIDApprovedNoMockPosts,
                         BlogType = BlogTypes.WordPress,
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
            _blogRepositoryMock.Verify(b => b.GetByID(_blogIDApprovedNoMockPosts), Times.Once);
            _blogRepositoryMock.Verify(b => b.Update(It.IsAny<Blog>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(statusCodeResult.StatusCode, HttpStatusCode.NoContent);
        }


        [TestMethod]
        public void PutBlogNoPostsNotApprovedReturnsHttpStatusCodeNoContent()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                       .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult =
                _controller.PutBlog(
                    _blogIDNotApproved,
                     new BlogModel
                     {
                         BlogID = _blogIDNotApproved,
                         BlogType = BlogTypes.WordPress,
                         CreatedDate = new DateTime(2015, 12, 1, 10, 55, 32),
                         Approved = false,
                         AuthorEmail = "k@s.com",
                         AuthorName = "KDS",
                         Description = "Stupendous Blog",
                         Link = "kdssnyder.wordpress.com",
                         Title = "KDS Blog"
                     });
            var statusCodeResult = actionResult as StatusCodeResult;

            // Assert
            // Verify that:
            //  GetByID is called just once
            //  Update is called for Blog object
            //  unit of work is committed once
            //  result of update is HTTP status code with no content
            _blogRepositoryMock.Verify(b => b.GetByID(_blogIDNotApproved), Times.Once);
            _blogRepositoryMock.Verify(b => b.Update(It.IsAny<Blog>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(statusCodeResult.StatusCode, HttpStatusCode.NoContent);
        }

        [TestMethod]
        public void PutBlogWithPostsApprovedNotChangedReturnsHttpStatusCodeNoContent()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                       .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult =
                _controller.PutBlog(
                    _blogIDApprovedMockPosts,
                     new BlogModel
                     {
                         BlogID = _blogIDApprovedMockPosts,
                         BlogType = BlogTypes.WordPress,
                         CreatedDate = new DateTime(2015, 12, 3, 10, 55, 32),
                         Approved = true,
                         AuthorEmail = "unit@unit.com",
                         AuthorName = "Unit Testerson",
                         Description = "Unit Testing Again",
                         Link = "http://testerson.wordpress.com",
                         Title = "Testerson's Blog"
                     });
            var statusCodeResult = actionResult as StatusCodeResult;

            // Assert
            // Verify that:
            //  GetByID is called just once
            //  Update is called for Blog object
            //  unit of work is committed once
            //  result of update is HTTP status code with no content
            _blogRepositoryMock.Verify(b => b.GetByID(_blogIDApprovedMockPosts), Times.Once);
            _blogRepositoryMock.Verify(b => b.Update(It.IsAny<Blog>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(statusCodeResult.StatusCode, HttpStatusCode.NoContent);
        }

        [TestMethod]
        public void PutBlogApprovedChangedtoTrueReturnsHttpStatusCodeNoContentAndAddsPosts()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                       .Returns(_users[_userAuthorizedIndexInArray]);
            BlogInfo blogInfo = new BlogInfo
            {
                Description = "Stupendous Blog",              
                Title = "KDS Blog"
            };
            _wordPressBlogReaderMock.Setup(m => m.VerifyBlog(It.IsAny<string>())).Returns(blogInfo);

            // Act
            IHttpActionResult actionResult =
                _controller.PutBlog(
                    _blogIDNotApproved,
                     new BlogModel
                     {
                         BlogID = _blogIDNotApproved,
                         BlogType = BlogTypes.WordPress,
                         CreatedDate = new DateTime(2015, 12, 1, 10, 55, 32),
                         Approved = true,
                         AuthorEmail = "k@s.com",
                         AuthorName = "KDS",
                         Description = "Stupendous Blog",
                         Link = "XXXX",
                         Title = "KDS Blog"
                     });
            var statusCodeResult = actionResult as StatusCodeResult;

            // Assert
            // Verify that:
            //  GetByID is called just once
            //  Update is called for Blog object            
            //  unit of work is committed at least once
            //  ExtractAndSaveBlogPosts is called
            //  result of update is HTTP status code with no content
            _blogRepositoryMock.Verify(b => b.GetByID(_blogIDNotApproved), Times.Once);
            _blogRepositoryMock.Verify(b => b.Update(It.IsAny<Blog>()), Times.Once);
            _blogServiceMock.Verify(bs => bs.ExtractAndSaveBlogPosts(It.IsAny<BlogModel>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.AtLeastOnce);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(statusCodeResult.StatusCode, HttpStatusCode.NoContent);
        }


        [TestMethod]
        public void PutBlogWithPostsApprovedChangedtoFalseReturnsHttpStatusCodeNoContentAndDeletesPosts()
        {
            // Arrange
            _postRepositoryMock.Setup(pr => pr.Where(It.IsAny<Expression<Func<Post, bool>>>()))
                                                                        .Returns(_posts.AsQueryable());
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                       .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult =
                _controller.PutBlog(
                    _blogIDApprovedMockPosts,
                     new BlogModel
                     {
                         BlogID = _blogIDApprovedMockPosts,
                         BlogType = BlogTypes.WordPress,
                         CreatedDate = new DateTime(2015, 12, 3, 10, 55, 32),
                         Approved = false,
                         AuthorEmail = "unit@unit.com",
                         AuthorName = "Unit Testerson",
                         Description = "Unit Testing",
                         Link = "http://testerson.wordpress.com",
                         Title = "Testerson's Blog"
                     });
            var statusCodeResult = actionResult as StatusCodeResult;

            // Assert
            // Verify that:
            //  GetByID is called just once
            //  Update is called for Blog object
            //  Delete is called for Post object the correct number of times
            //  unit of work is committed at least once
            //  result of update is HTTP status code with no content
            _blogRepositoryMock.Verify(b => b.GetByID(_blogIDApprovedMockPosts), Times.Once);
            _blogRepositoryMock.Verify(b => b.Update(It.IsAny<Blog>()), Times.Once);
            _postRepositoryMock.Verify(p => p.Delete(It.IsAny<Post>()), Times.Exactly(_numberOfMockPosts));
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.AtLeastOnce);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(statusCodeResult.StatusCode, HttpStatusCode.NoContent);
        }


        [TestMethod]
        public void PutBlogNonexistentBlogIDReturnsNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                       .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult =
                _controller.PutBlog(
                    _blogIDNonexistent,
                     new BlogModel
                     {
                         BlogID = _blogIDNonexistent,
                         BlogType = BlogTypes.WordPress,
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
            BlogInfo blogInfo = new BlogInfo
            {
                Description = "New Testing",
                Title = "New Testy's Blog"
            };
            _wordPressBlogReaderMock.Setup(m => m.VerifyBlog(It.IsAny<string>())).Returns(blogInfo);            

            // Act
            IHttpActionResult actionResult =
                _controller.PostBlog(
                    new BlogModel
                    {
                        BlogID = 0,
                        BlogType = BlogTypes.WordPress,
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
        public void PostBlogApprovedAddsBlogAndPosts()
        {
            // Arrange          
            BlogInfo blogInfo = new BlogInfo
            {
                Description = "Stupendous Blog",
                Title = "KDS Blog"
            };
            _wordPressBlogReaderMock.Setup(m => m.VerifyBlog(It.IsAny<string>())).Returns(blogInfo);

            // Act
            IHttpActionResult actionResult =
                _controller.PostBlog(
                     new BlogModel
                     {
                         BlogID = _blogIDNotApproved,
                         BlogType = BlogTypes.WordPress,
                         CreatedDate = new DateTime(2015, 12, 1, 10, 55, 32),
                         Approved = true,
                         AuthorEmail = "k@s.com",
                         AuthorName = "KDS",
                         Description = "Stupendous Blog",
                         Link = "XXXX",
                         Title = "KDS Blog"
                     });
            var statusCodeResult = actionResult as StatusCodeResult;

           // Assert
           // Verify that:
           //  Add is called for Blog object
           //  Unit of Work is called at least once
           //  ExtractAndSaveBlogPosts is called
           //  HTTP result is CreatedAtRouteNegotiatedContentResult
           //  Location header is set in created result
            _blogRepositoryMock.Verify(b => b.Add(It.IsAny<Blog>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.AtLeastOnce);
            _blogServiceMock.Verify(bs => bs.ExtractAndSaveBlogPosts(It.IsAny<BlogModel>()), Times.Once);
           Assert.IsInstanceOfType
                    (actionResult, typeof(CreatedAtRouteNegotiatedContentResult<BlogModel>));
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<BlogModel>;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(createdResult.RouteName, "DefaultApi");
        }

        [TestMethod]
        public void DeleteBlogUnauthorizedUserReturnsUnauthorized()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                       .Returns(_users[_userUnauthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult = _controller.DeleteBlog(_blogIDApprovedNoMockPosts);

            // Assert
            // Verify that HTTP status code result of delete is unauthorized
            Assert.IsInstanceOfType(actionResult, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void DeleteBlogNoPostsReturnsOk()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                       .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult = _controller.DeleteBlog(_blogIDApprovedNoMockPosts);

            // Assert
            // Verify:
            //  GetByID is called once
            //  Delete is called once with correct object
            //  Unit of work commit is called once
            //  Result is OK, and content result ID matches
            _blogRepositoryMock.Verify(p => p.GetByID(_blogIDApprovedNoMockPosts), Times.Once);
            _blogRepositoryMock.Verify(p => p.Delete(_blogs[_blogIDApprovedNoMockPostsIndexInArray]), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<BlogModel>));
            var contentResult = actionResult as OkNegotiatedContentResult<BlogModel>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsTrue(contentResult.Content.BlogID == _blogIDApprovedNoMockPosts);
        }


        [TestMethod]
        public void DeleteBlogWithPostsReturnsOkAndDeletesPosts()
        {
            // Arrange
            _postRepositoryMock.Setup(pr => pr.Where(It.IsAny<Expression<Func<Post, bool>>>()))
                                                                        .Returns(_posts.AsQueryable());
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                       .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult = _controller.DeleteBlog(_blogIDApprovedMockPosts);

            // Assert
            // Verify:
            //  GetByID is called once
            //  Delete is called once with Blog object
            //  Delete is called for Post object the correct number of times
            //  Unit of work is committed at least  once
            //  Result is OK, and content result ID matches
            _blogRepositoryMock.Verify(p => p.GetByID(_blogIDApprovedMockPosts), Times.Once);
            _blogRepositoryMock.Verify(p => p.Delete(_blogs[_blogIDApprovedMockPostsIndexInArray]), Times.Once);
            _postRepositoryMock.Verify(p => p.Delete(It.IsAny<Post>()), Times.Exactly(_numberOfMockPosts));
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.AtLeastOnce);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<BlogModel>));
            var contentResult = actionResult as OkNegotiatedContentResult<BlogModel>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsTrue(contentResult.Content.BlogID == _blogIDApprovedMockPosts);            
        }

        [TestMethod]
        public void DeleteNonExistentBlogReturnsNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                       .Returns(_users[_userAuthorizedIndexInArray]);

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
