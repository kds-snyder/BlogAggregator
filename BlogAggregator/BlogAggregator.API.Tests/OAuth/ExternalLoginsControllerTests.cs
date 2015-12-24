using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BlogAggregator.Core.Repository;
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.API.Controllers.OAuth;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using BlogAggregator.Core.Models;
using System.Net;
using System;
using System.Linq.Expressions;

namespace BlogAggregator.API.Tests.OAuth
{
    [TestClass]
    public class ExternalLoginsControllerTests
    {
        private Mock<IExternalLoginRepository> _externalLoginRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private ExternalLoginsController _controller;
        private ExternalLogin[] _externalLogins;
        private User[] _users;

        // Numbers for tests
        private int _userAuthorizedIndexInData = 0;
        private int _userUnauthorizedIndexInData = 1;
        private int _userIDAuthorized = 1;
        private int _userIDUnauthorized = 2;
        private int _userIDNonexistent = 8;
        private int _externalLoginIDWithUnauthorizedUser = 1;
        private int _externalLoginIDWithAuthorizedUser = 2;
        private int _externalLoginIDWithUnauthorizedUserIndexInData = 0;
        private int _externalLoginIDWithAuthorizedUserIndexInData = 1;
        private int _externalLoginIDNonexistent = 9;
        private int _numberOfMockExternalLogins = 2;

        [TestInitialize]
        public void Initialize()
        {
            // Set up Automapper
            WebApiConfig.CreateMaps();

            // Set up repositories          
            _externalLoginRepositoryMock = new Mock<IExternalLoginRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();

            // Set data for external logins: first corresponds to unauthorized user ID,
            //  second corresponds to authorized user ID
            _externalLogins = new[]
            {
                new ExternalLogin
                {
                    ExternalLoginID = _externalLoginIDWithUnauthorizedUser,
                    UserID = _userIDUnauthorized,
                    LoginProvider = "AAAA",
                    ProviderKey = "BBBBB"
                },
                               new ExternalLogin
                {
                    ExternalLoginID = _externalLoginIDWithAuthorizedUser,
                    UserID = _userIDAuthorized,
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

            _externalLoginRepositoryMock.Setup(el => el.GetAll()).Returns(_externalLogins.AsQueryable());
            _externalLoginRepositoryMock.Setup(el => el.GetByID(_externalLoginIDWithAuthorizedUser))
                                                    .Returns(_externalLogins[_externalLoginIDWithAuthorizedUserIndexInData]);
            _externalLoginRepositoryMock.Setup(el => el.GetByID(_externalLoginIDWithUnauthorizedUser))
                                         .Returns(_externalLogins[_externalLoginIDWithUnauthorizedUserIndexInData]);

            // Set up unit of work and controller
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _controller = new ExternalLoginsController(_externalLoginRepositoryMock.Object, _userRepositoryMock.Object,
                                                        _unitOfWorkMock.Object);
        }

        [TestMethod]
        public void GetExternalLoginsReturnsExternalLogins()
        {
            // Arrange

            // Act
            var content = _controller.GetExternalLogins();

            // Assert
            // Verify that GetAll is called just once
            // Verify that the correct number of externalLogins are returned
            _externalLoginRepositoryMock.Verify(el => el.GetAll(), Times.Once);
            Assert.AreEqual(content.Count(), _numberOfMockExternalLogins);
        }

        [TestMethod]
        public void GetExternalLoginReturnsSameID()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult = _controller.GetExternalLogin(_externalLoginIDWithAuthorizedUser);

            // Assert
            // Verify that GetByID is called just once
            // Verify that HTTP status code is Ok, with object of correct type
            _externalLoginRepositoryMock.Verify(el => el.GetByID(_externalLoginIDWithAuthorizedUser), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<ExternalLoginModel>));

            // Extract the content from the HTTP result
            // Verify the content is not null, and the IDs match
            var contentResult = actionResult as OkNegotiatedContentResult<ExternalLoginModel>;
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(contentResult.Content.ExternalLoginID, _externalLoginIDWithAuthorizedUser);
        }

        [TestMethod]
        public void GetNonExistentExternalLoginReturnsNotFound()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult = _controller.GetExternalLogin(_externalLoginIDNonexistent);

            // Assert
            // Verify that GetByID is called just once
            // Verify that HTTP status code is NotFound
            _externalLoginRepositoryMock.Verify(el => el.GetByID(_externalLoginIDNonexistent), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PutExternalLoginUnauthorizedUserReturnsUnauthorized()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                       .Returns(_users[_userUnauthorizedIndexInData]);

            // Act
            IHttpActionResult actionResult =
                _controller.PutExternalLogin(
                    _externalLoginIDWithAuthorizedUser,
                     new ExternalLoginModel
                     {
                         ExternalLoginID = _externalLoginIDWithAuthorizedUser,
                         UserID = _userIDAuthorized,
                         LoginProvider = "AAAA",
                         ProviderKey = "DDDD"
                     });

            // Assert
            // Verify that HTTP status code result of update is unauthorized
            Assert.IsInstanceOfType(actionResult, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void PutExternalLoginReturnsHttpStatusCodeNoContent()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                      .Returns(_users[_userAuthorizedIndexInData]);

            // Act
            IHttpActionResult actionResult =
                _controller.PutExternalLogin(
                    _externalLoginIDWithAuthorizedUser,
                     new ExternalLoginModel
                     {
                         ExternalLoginID = _externalLoginIDWithAuthorizedUser,
                         UserID = _userIDAuthorized,
                         LoginProvider = "AAAA",
                         ProviderKey = "DDDD"
                     });
            var statusCodeResult = actionResult as StatusCodeResult;

            // Assert
            // Verify that:
            //  GetByID is called just once
            //  Update is called for ExternalLogin object
            //  unit of work is committed once
            //  result of update is HTTP status code with no content
            _externalLoginRepositoryMock.Verify(el => el.GetByID(_externalLoginIDWithAuthorizedUser), Times.Once);
            _externalLoginRepositoryMock.Verify(el => el.Update(It.IsAny<ExternalLogin>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(statusCodeResult.StatusCode, HttpStatusCode.NoContent);
        }

        [TestMethod]
        public void PutNonexistentExternalLoginReturnsNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                      .Returns(_users[_userAuthorizedIndexInData]);

            // Act
            IHttpActionResult actionResult =
                _controller.PutExternalLogin(
                    _externalLoginIDNonexistent,
                     new ExternalLoginModel
                     {
                         ExternalLoginID = _externalLoginIDNonexistent,
                         UserID = _userIDAuthorized,
                         LoginProvider = "AAAA",
                         ProviderKey = "DDDD"
                     });

            // Assert
            // Verify:
            //  GetByID is called once
            //  Result is NotFound
            _externalLoginRepositoryMock.Verify(el => el.GetByID(_externalLoginIDNonexistent), Times.Once);
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PostExternalLoginAddsExternalLogin()
        {
            // Arrange
           _userRepositoryMock.Setup(u => u.Any(It.IsAny<Expression<Func<User, bool>>>())).Returns(true);

            // Act
            IHttpActionResult actionResult =
                _controller.PostExternalLogin(
                    new ExternalLoginModel
                    {
                        ExternalLoginID = 0,
                        UserID = _userIDUnauthorized,
                        LoginProvider = "FFFF",
                        ProviderKey = "GGGG"
                    });

            // Assert
            // Verify:
            //  Add is called just once with ExternalLogin object
            //  Unit of work is committed just once
            //  HTTP result is CreatedAtRouteNegotiatedContentResult
            //  Location header is set in created result
            _externalLoginRepositoryMock.Verify(el => el.Add(It.IsAny<ExternalLogin>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsInstanceOfType
                    (actionResult, typeof(CreatedAtRouteNegotiatedContentResult<ExternalLoginModel>));
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<ExternalLoginModel>;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(createdResult.RouteName, "DefaultApi");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception),
            "Unable to add the external login to the database, as it does not correspond to a user")]
        public void PostExternalLoginNonExistentUserIdThrowsException()
        {
            // Arrange

            // Act
            IHttpActionResult actionResult =
                _controller.PostExternalLogin(
                    new ExternalLoginModel
                    {
                        ExternalLoginID = 0,
                        UserID = _userIDNonexistent,
                        LoginProvider = "HHH",
                        ProviderKey = "JJJJ"
                    });

            // Assert
            // Test fails if it reaches here, as API method should throw exception
            Assert.Fail();
        }

        [TestMethod]
        public void DeleteExternalLoginUnauthorizedUserReturnsUnauthorized()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                       .Returns(_users[_userUnauthorizedIndexInData]);

            // Act
            IHttpActionResult actionResult = _controller.DeleteExternalLogin(_externalLoginIDWithUnauthorizedUser);

            // Assert
            // Verify that HTTP status code result of delete is unauthorized
            Assert.IsInstanceOfType(actionResult, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void DeleteExternalLoginReturnsOk()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                      .Returns(_users[_userAuthorizedIndexInData]);

            // Act
            IHttpActionResult actionResult = _controller.DeleteExternalLogin(_externalLoginIDWithUnauthorizedUser);

            // Assert
            // Verify:
            //  GetByID is called once
            //  Delete is called once with correct object
            //  Unit of work commit is called once
            //  Result is OK, and content result ID matches
            _externalLoginRepositoryMock.Verify(el => el.GetByID(_externalLoginIDWithUnauthorizedUser), Times.Once);
            _externalLoginRepositoryMock.Verify(el => el.Delete
                                        (_externalLogins[_externalLoginIDWithUnauthorizedUserIndexInData]), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<ExternalLoginModel>));
            var contentResult = actionResult as OkNegotiatedContentResult<ExternalLoginModel>;
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsTrue(contentResult.Content.ExternalLoginID == _externalLoginIDWithUnauthorizedUser);
        }

        [TestMethod]
        public void DeleteNonExistentExternalLoginReturnsNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(pr => pr.FirstOrDefault(It.IsAny<Expression<Func<User, bool>>>()))
                                                                      .Returns(_users[_userAuthorizedIndexInData]);

            // Act
            IHttpActionResult actionResult = _controller.DeleteExternalLogin(_externalLoginIDNonexistent);

            // Assert
            // Verify that GetByID is called once
            // Verify that result is NotFound
            _externalLoginRepositoryMock.Verify(el => el.GetByID(_externalLoginIDNonexistent), Times.Once);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }
    }
}
