using BlogAggregator.Core.Repository;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.API.Controllers;

namespace BlogAggregator.API.Tests
{
    [TestClass]
    public class PostControllerTests
    {
        private Mock<IBlogRepository> _blogRepositoryMock;
        private Mock<IPostRepository> _postRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private PostsController _controller;

        [TestInitialize]
        public void Initialize()
        {
            // Set up Automapper
            WebApiConfig.CreateMaps();

            // Set up repositories
            _blogRepositoryMock = new Mock<IBlogRepository>();
            _postRepositoryMock = new Mock<IPostRepository>();

            // Set data in repositories

            // Set up unit of work and controller
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _controller = new PostsController(_blogRepositoryMock.Object, _postRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
