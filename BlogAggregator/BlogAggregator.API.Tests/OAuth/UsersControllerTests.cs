using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlogAggregator.Core.Repository;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.API.Controllers.OAuth;
using Moq;
using BlogAggregator.Core.Domain;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using System.Web.Http.Results;
using BlogAggregator.Core.Models;
using System.Net;

namespace BlogAggregator.API.Tests.OAuth
{
    [TestClass]
    public class UsersControllerTests
    {
        private Mock<IExternalLoginRepository> _externalLoginRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private ExternalLogin[] _externalLogins;
        private UsersController _controller;
        private User[] _users;      

        // Numbers for tests
        private int _userAuthorizedIndexInArray = 0;
        private int _userUnauthorizedIndexInArray = 1;
        private int _userIDAuthorized = 1;
        private int _userIDUnauthorized = 2;
        private int _userNoExternalLoginsIndexInArray = 0;
        private int _userWithExternalLoginsIndexInArray = 1;
        private int _userIDNoExternalLogins = 1;
        private int _userIDWithExternalLogins = 2;
        private int _userIDNonexistent = 8;
        private int _numberOfMockUsers = 2;
        private int _numberOfMockExternalLogins = 2;

        [TestInitialize]
        public void Initialize()
        {
            // Set up Automapper
            WebApiConfig.CreateMaps();

            // Set up repositories          
            _userRepositoryMock = new Mock<IUserRepository>();
            _externalLoginRepositoryMock = new Mock<IExternalLoginRepository>();

            // Set data for external logins          
            _externalLogins = new[]
            {
                new ExternalLogin
                {
                    ExternalLoginID = 1,
                    UserID = _userIDWithExternalLogins,
                    LoginProvider = "AAAA",
                    ProviderKey = "BBBBB"
                },
                               new ExternalLogin
                {
                    ExternalLoginID = 2,
                    UserID = _userIDWithExternalLogins,
                    LoginProvider = "AAAA",
                    ProviderKey = "CCCCC"
                }
            };

            // Set up users: first is authorized, second is unauthorized
            _users = new[]
            {
                new User {
                Id = _userIDAuthorized,
                Authorized = true,
                PasswordHash = "XXX",
                SecurityStamp = "YYY",
                UserName = "userAuthorized"
                },
                 new User {
                Id = _userIDUnauthorized,
                Authorized = false,
                PasswordHash = "XXX",
                SecurityStamp = "YYY",
                UserName = "userUnauthorized"
                }
            };

            _userRepositoryMock.Setup(u => u.GetAll()).Returns(_users.AsQueryable());
            _userRepositoryMock.Setup(u => u.GetByID(_userIDAuthorized))
                                                    .Returns(_users[_userAuthorizedIndexInArray]);
            _userRepositoryMock.Setup(u => u.GetByID(_userIDUnauthorized))
                                         .Returns(_users[_userUnauthorizedIndexInArray]);

            // Set up unit of work and controller
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _controller = new UsersController(_externalLoginRepositoryMock.Object, _userRepositoryMock.Object,
                                                        _unitOfWorkMock.Object);
        }

        [TestMethod]
        public void GetUsersReturnsUsers()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                      .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            var content = _controller.GetUsers();

            // Assert
            // Verify that GetAll is called just once
            // Verify that the correct number of users are returned
            _userRepositoryMock.Verify(u => u.GetAll(), Times.Once);
            Assert.AreEqual(content.Count(), _numberOfMockUsers);
        }

        [TestMethod]
        public void GetUserReturnsSameID()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                      .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult = _controller.GetUser(_userIDUnauthorized);

            // Assert
            // Verify that GetByID is called just once
            // Verify that HTTP status code is Ok, with object of correct type
            _userRepositoryMock.Verify(u => u.GetByID(_userIDUnauthorized), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<UserModel>));

            // Extract the content from the HTTP result
            // Verify the content is not null, and the IDs match
            var contentResult = actionResult as OkNegotiatedContentResult<UserModel>;
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(contentResult.Content.Id, _userIDUnauthorized);
        }

        [TestMethod]
        public void GetNonExistentUserReturnsNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                      .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult = _controller.GetUser(_userIDNonexistent);

            // Assert
            // Verify that GetByID is called just once
            // Verify that HTTP status code is NotFound
            _userRepositoryMock.Verify(u => u.GetByID(_userIDNonexistent), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PutUserUnauthorizedUserReturnsUnauthorized()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                       .Returns(_users[_userUnauthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult =
                _controller.PutUser(
                    _userIDAuthorized,
                     new UserModel
                     {
                         Id = _userIDAuthorized,
                         Authorized = true,
                         PasswordHash = "XXX",
                         SecurityStamp = "YYY",
                         UserName = "userAuthorized"
                     });

            // Assert
            // Verify that HTTP status code result of update is unauthorized
            Assert.IsInstanceOfType(actionResult, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void PutUserReturnsHttpStatusCodeNoContent()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                      .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult =
                _controller.PutUser(
                    _userIDUnauthorized,
                     new UserModel
                     {
                         Id = _userIDUnauthorized,
                         Authorized = true,
                         PasswordHash = "XXX",
                         SecurityStamp = "YYY",
                         UserName = "userAuthorized"
                     });
            var statusCodeResult = actionResult as StatusCodeResult;

            // Assert
            // Verify that:
            //  GetByID is called just once
            //  Update is called for User object
            //  unit of work is committed once
            //  result of update is HTTP status code with no content
            _userRepositoryMock.Verify(u => u.GetByID(_userIDUnauthorized), Times.Once);
            _userRepositoryMock.Verify(u => u.Update(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(statusCodeResult.StatusCode, HttpStatusCode.NoContent);
        }

        [TestMethod]
        public void PutNonexistentUserReturnsNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                      .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult =
                _controller.PutUser(
                    _userIDNonexistent,
                     new UserModel
                     {
                         Id = _userIDNonexistent,
                         Authorized = true,
                         PasswordHash = "XXXX",
                         SecurityStamp = "ZZZZ",
                         UserName = "user"                       
                     });

            // Assert
            // Verify:
            //  GetByID is called once
            //  Result is NotFound
            _userRepositoryMock.Verify(u => u.GetByID(_userIDNonexistent), Times.Once);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PostUserAddsUser()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                        .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult =
                _controller.PostUser(
                    new UserModel
                    {
                        Id = _userIDUnauthorized,
                        Authorized = false,
                        PasswordHash = "XXX",
                        SecurityStamp = "YYY",
                        UserName = "userUnauthorized"
                    });

            // Assert
            // Verify:
            //  Add is called just once with User object
            //  Unit of work is committed just once
            //  HTTP result is CreatedAtRouteNegotiatedContentResult
            //  Location header is set in created result
            _userRepositoryMock.Verify(u => u.Add(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsInstanceOfType
                    (actionResult, typeof(CreatedAtRouteNegotiatedContentResult<UserModel>));
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<UserModel>;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(createdResult.RouteName, "DefaultApi");
        }
       

        [TestMethod]
        public void DeleteUserUnauthorizedUserReturnsUnauthorized()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                       .Returns(_users[_userUnauthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult = _controller.DeleteUser(_userIDAuthorized);

            // Assert
            // Verify that HTTP status code result of delete is unauthorized
            Assert.IsInstanceOfType(actionResult, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void DeleteUserNoExternalLoginsReturnsOk()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                      .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult = _controller.DeleteUser(_userIDNoExternalLogins);

            // Assert
            // Verify:
            //  GetByID is called once
            //  Delete is called once with correct object
            //  Unit of work commit is called once
            //  Result is OK, and content result ID matches
            _userRepositoryMock.Verify(u => u.GetByID(_userIDNoExternalLogins), Times.Once);
            _userRepositoryMock.Verify(u => u.Delete
                                        (_users[_userNoExternalLoginsIndexInArray]), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<UserModel>));
            var contentResult = actionResult as OkNegotiatedContentResult<UserModel>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsTrue(contentResult.Content.Id == _userIDNoExternalLogins);
        }

        [TestMethod]
        public void DeleteUserWithExternalLoginsReturnsOkAndDeletesExternalLogins()
        {
            // Arrange
            _externalLoginRepositoryMock.Setup(pr => pr.Where(It.IsAny<Expression<Func<ExternalLogin, bool>>>()))
                                                                       .Returns(_externalLogins.AsQueryable());
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                      .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult = _controller.DeleteUser(_userIDWithExternalLogins);

            // Assert
            // Verify:
            //  GetByID is called once
            //  Delete is called once with User object
            //  Delete is called for ExternalLogin object the correct number of times
            //  Unit of work commit is called once
            //  Result is OK, and content result ID matches
            _userRepositoryMock.Verify(u => u.GetByID(_userIDWithExternalLogins), Times.Once);
            _userRepositoryMock.Verify(u => u.Delete
                                        (_users[_userWithExternalLoginsIndexInArray]), Times.Once);
            _externalLoginRepositoryMock.Verify(el => el.Delete(It.IsAny<ExternalLogin>()), 
                                                    Times.Exactly(_numberOfMockExternalLogins));
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<UserModel>));
            var contentResult = actionResult as OkNegotiatedContentResult<UserModel>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsTrue(contentResult.Content.Id == _userIDWithExternalLogins);
        }

        [TestMethod]
        public void DeleteNonExistentUserReturnsNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                      .Returns(_users[_userAuthorizedIndexInArray]);

            // Act
            IHttpActionResult actionResult = _controller.DeleteUser(_userIDNonexistent);

            // Assert
            // Verify that GetByID is called once
            // Verify that result is NotFound
            _userRepositoryMock.Verify(u => u.GetByID(_userIDNonexistent), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }
    }
}
