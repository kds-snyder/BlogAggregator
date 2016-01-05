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
using BlogAggregator.Core.BlogReader;
using System.Linq.Expressions;

namespace BlogAggregator.Core.Test.BlogServices
{
    [TestClass]
    public class BlogServiceTests
    {
        private Mock<IBlogRepository> _blogRepositoryMock;
        private Mock<IPostRepository> _postRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;        
        private Mock<IWordPressBlogReader> _wordPressBlogReaderMock;
        //private Mock<ILogger> _loggerMock;
        private BlogService _blogService;

        private Blog[] _blogs;
        private Blog _blogA;
        private Blog _blogB;
        private Post[] _posts;
        private Post[] _postsBlogA;
        private Post[] _postsBlogB;
        private Post[] _postsNewForBlogA;
        private Post[] _postsNewForBlogB;

        // Numbers for tests
        private int _blogIDAWithPosts = 1;
        private int _blogIDBWithPosts = 2;
        private int _blogIDAWithPostsIndexInArray = 0;
        private int _blogIDBWithPostsIndexInArray= 1;

        private int _postIDBlogIDAFirst = 2;
        private int _postIDBlogIDASecond = 3;
        private int _postIDBlogIDAThird = 4;
        private int _postIDBlogIDAFourth = 5;
        private int _postIDBlogIDAFirstIndexInArray = 0;
        private int _postIDBlogIDASecondIndexInArray = 1;
        private int _postIDBlogIDAThirdIndexInArray = 2;
        private int _postIDBlogIDAFourthIndexInArray = 3;

        private int _postIDBlogIDBFirst = 7;
        private int _postIDBlogIDBSecond = 8;
        private int _postIDBlogIDBThird = 9;
        private int _postIDBlogIDBFirstIndexInArray = 0;
        private int _postIDBlogIDBSecondIndexInArray = 1;
        private int _postIDBlogIDBThirdIndexInArray = 2;

        private int _postIDNewForBlogAFirst = 11;
        private int _postIDNewForBlogASecond = 12;

        private int _postIDNewForBlogBFirst = 14;

        [TestInitialize]
        public void Initialize()
        {
            // Set up Automapper
            WebApiConfig.CreateMaps();

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

            _postsBlogA = new[]
     {
                new Post {
                    PostID = _postIDBlogIDAFirst,
                    BlogID = _blogIDAWithPosts,
                    Blog = _blogs[_blogIDAWithPostsIndexInArray],
                    Content = "Test content A",
                    Description = "Interesting post A",
                    Guid = _postIDBlogIDAFirst.ToString(),
                    Link = "http://testy.wordpress.com/post/1",
                    PublicationDate = DateTime.Now,
                    Title = "Interesting Title A"
               },
                new Post {
                    PostID = _postIDBlogIDASecond,
                    BlogID = _blogIDAWithPosts,
                    Blog = _blogs[_blogIDAWithPostsIndexInArray],
                    Content = "More test content A",
                    Description = "More interesting post A",
                    Guid = _postIDBlogIDASecond.ToString(),
                    Link = "http://testy.wordpress.com/post/2",
                    PublicationDate = DateTime.Now,
                    Title = "More Interesting Title A"
                },
                new Post {
                    PostID = _postIDBlogIDAThird,
                    BlogID = _blogIDAWithPosts,
                    Blog = _blogs[_blogIDAWithPostsIndexInArray],
                    Content = "Even more test content A",
                    Description = "Even more interesting post A",
                    Guid = _postIDBlogIDAThird.ToString(),
                    Link = "http://testy.wordpress.com/post/3",
                    PublicationDate = DateTime.Now,
                    Title = "Even More Interesting Title A"
                },
                    new Post {
                    PostID = _postIDBlogIDAFourth,
                    BlogID = _blogIDAWithPosts,
                    Blog = _blogs[_blogIDAWithPostsIndexInArray],
                    Content = "Yet more test content A",
                    Description = "Yet more interesting post A",
                    Guid = _postIDBlogIDAFourth.ToString(),
                    Link = "http://testy.wordpress.com/post/3",
                    PublicationDate = DateTime.Now,
                    Title = "Yet More Interesting Title A"
                }
            };

            _postsBlogB = new[]
{
                new Post {
                    PostID = _postIDBlogIDBFirst,
                    BlogID = _blogIDBWithPosts,
                    Blog = _blogs[_blogIDBWithPostsIndexInArray],
                    Content = "Test content B",
                    Description = "Interesting post B",
                    Guid = _postIDBlogIDBFirst.ToString(),
                    Link = "http://testy.wordpress.com/post/1",
                    PublicationDate = DateTime.Now,
                    Title = "Interesting Title B"
               },
                new Post {
                    PostID = _postIDBlogIDBSecond,
                    BlogID = _blogIDBWithPosts,
                    Blog = _blogs[_blogIDBWithPostsIndexInArray],
                    Content = "More test content B",
                    Description = "More interesting post B",
                    Guid = _postIDBlogIDBSecond.ToString(),
                    Link = "http://testy.wordpress.com/post/2",
                    PublicationDate = DateTime.Now,
                    Title = "More Interesting Title B"
                },
                    new Post {
                    PostID = _postIDBlogIDBThird,
                    BlogID = _blogIDBWithPosts,
                    Blog = _blogs[_blogIDBWithPostsIndexInArray],
                    Content = "Even more test content B",
                    Description = "Even more interesting post B",
                    Guid = _postIDBlogIDBThird.ToString(),
                    Link = "http://testy.wordpress.com/post/2",
                    PublicationDate = DateTime.Now,
                    Title = "Even More Interesting Title B"
                }
            };

            _postsNewForBlogA = new[]
{
                new Post {
                    PostID = _postIDNewForBlogAFirst,
                    BlogID = 0,
                    Content = "New test content A",
                    Description = "New interesting post A",
                    Guid = _postIDNewForBlogAFirst.ToString(),
                    Link = "someblogA.com/post/1",
                    PublicationDate = DateTime.Now,
                    Title = "New Interesting Title A"
               },
                new Post {
                    PostID = _postIDNewForBlogASecond,
                    BlogID = 0,
                    Content = "More new test content A",
                    Description = "More new interesting post A",
                    Guid = _postIDNewForBlogASecond.ToString(),
                    Link = "someblogA.com/post/2",
                    PublicationDate = DateTime.Now,
                    Title = "More New Interesting Title A"
                }
            };

            _postsNewForBlogB = new[]
{
                new Post {
                    PostID = _postIDNewForBlogBFirst,
                    BlogID = 0,
                    Content = "New test content B",
                    Description = "New interesting post B",
                    Guid = _postIDNewForBlogBFirst.ToString(),
                    Link = "someblogB.com/post/1",
                    PublicationDate = DateTime.Now,
                    Title = "New Interesting Title B"
               }
            };

            _posts = _postsBlogA.Concat(_postsBlogB).ToArray();

            // Set up repositories 
            _blogRepositoryMock = new Mock<IBlogRepository>();
            _postRepositoryMock = new Mock<IPostRepository>();

            _blogRepositoryMock.Setup(br => br.GetAll()).Returns(_blogs.AsQueryable());
            _blogRepositoryMock.Setup(br => br.GetByID(_blogIDAWithPosts)).Returns(_blogs[_blogIDAWithPostsIndexInArray]);
            _blogRepositoryMock.Setup(br => br.GetByID(_blogIDBWithPosts)).Returns(_blogs[_blogIDBWithPostsIndexInArray]);
 
            _postRepositoryMock.Setup(pr => pr.GetAll()).Returns(_posts.AsQueryable());
            _postRepositoryMock.Setup(pr => pr.GetByID(_postIDBlogIDAFirst)).Returns(_posts[_postIDBlogIDAFirstIndexInArray]);
            _postRepositoryMock.Setup(pr => pr.GetByID(_postIDBlogIDASecond)).Returns(_posts[_postIDBlogIDASecondIndexInArray]);
            _postRepositoryMock.Setup(pr => pr.GetByID(_postIDBlogIDAThird)).Returns(_posts[_postIDBlogIDAThirdIndexInArray]);
            _postRepositoryMock.Setup(pr => pr.GetByID(_postIDBlogIDAFourth)).Returns(_posts[_postIDBlogIDAFourthIndexInArray]);
            _postRepositoryMock.Setup(pr => pr.GetByID(_postIDBlogIDBFirst)).Returns(_posts[_postIDBlogIDBFirstIndexInArray]);
            _postRepositoryMock.Setup(pr => pr.GetByID(_postIDBlogIDBSecond)).Returns(_posts[_postIDBlogIDBSecondIndexInArray]);
            _postRepositoryMock.Setup(pr => pr.GetByID(_postIDBlogIDBThird)).Returns(_posts[_postIDBlogIDBThirdIndexInArray]);

            // Set up unit of work and services
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _wordPressBlogReaderMock = new Mock<IWordPressBlogReader>();
            //_loggerMock = new Mock<ILogger>();
            _blogService = new BlogService(_blogRepositoryMock.Object, _postRepositoryMock.Object,
                                            _unitOfWorkMock.Object, _wordPressBlogReaderMock.Object);
                                                //_unitOfWorkMock.Object, _wordPressBlogReaderMock.Object,
                                                //_loggerMock.Object);

        }

        [TestMethod]
        public void ExtractBlogPostsReturnsBlogPosts()
        {
            // Arrange                      
            BlogInfo blogInfo = new BlogInfo
            {
                Description = _blogA.Description,
                Title = _blogA.Title
            };
            _wordPressBlogReaderMock.Setup(wp => wp.VerifyBlog(_blogA.Link)).Returns(blogInfo);
            _wordPressBlogReaderMock.Setup(wp => 
                        wp.GetBlogPosts(_blogA.Link)).Returns(_postsBlogA.AsQueryable());

            // Act
            IEnumerable<Post> posts = _blogService.ExtractBlogPosts(_blogs[_blogIDAWithPostsIndexInArray]);

            // Assert
            Assert.AreEqual(posts.Count(), _postsBlogA.Length);
        }

        [TestMethod]
        public void ExtractAndSaveBlogPostsExtractsAndSavesBlogPosts()
        {
            // Arrange                      
            BlogInfo blogInfo = new BlogInfo
            {
                Description = _blogA.Description,
                Title = _blogA.Title
            };
            _wordPressBlogReaderMock.Setup(wp => wp.VerifyBlog(_blogA.Link)).Returns(blogInfo);
            _wordPressBlogReaderMock.Setup(wp =>
                        wp.GetBlogPosts(_blogA.Link)).Returns(_postsBlogA.AsQueryable());

            // Act
            _blogService.ExtractAndSaveBlogPosts(_blogA);

            // Assert
            // Verify that:
            //  Add is called for Post object the correct number of times
            //  unit of work is committed once
            _postRepositoryMock.Verify(p => p.Add(It.IsAny<Post>()), Times.Exactly(_postsBlogA.Length));
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);
        }

        [TestMethod]
        public void ExtractAndSaveNewBlogPostsSavesNewBlogPosts()
        {
            // Arrange 
            BlogInfo blogInfo = new BlogInfo
            {
                Description = _blogB.Description,
                Title = _blogB.Title
            };
            _wordPressBlogReaderMock.Setup(wp => wp.VerifyBlog(_blogB.Link)).Returns(blogInfo);
            _wordPressBlogReaderMock.Setup(wp =>
                        wp.GetBlogPosts(_blogB.Link)).Returns(_postsNewForBlogB.AsQueryable());
            _postRepositoryMock.Setup(pr => pr.Any(It.IsAny<Expression<Func<Post, bool>>>())).Returns(false);

            // Act
            _blogService.ExtractAndSaveNewBlogPosts(_blogB);

            // Assert
            // Verify that:
            //  Add is called for Post object the correct number of times
            //  unit of work is committed once
            _postRepositoryMock.Verify(p => p.Add(It.IsAny<Post>()), Times.Exactly(_postsNewForBlogB.Length));
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Once);

        }


        [TestMethod]
        public void ExtractAndSaveAllNewBlogPostsSavesAllNewBlogPosts()
        {
            // Arrange 
            BlogInfo blogInfoA = new BlogInfo
            {
                Description = _blogA.Link,
                Title = _blogA.Title
            };
            BlogInfo blogInfoB = new BlogInfo
            {
                Description = _blogB.Link,
                Title = _blogB.Title
            };
            _wordPressBlogReaderMock.Setup(wp => wp.VerifyBlog(_blogA.Link)).Returns(blogInfoA);
            _wordPressBlogReaderMock.Setup(wp => wp.VerifyBlog(_blogB.Link)).Returns(blogInfoB);
            _wordPressBlogReaderMock.Setup(wp =>
                    wp.GetBlogPosts(_blogA.Link)).Returns(_postsNewForBlogA.AsQueryable());
            _wordPressBlogReaderMock.Setup(wp =>
                    wp.GetBlogPosts(_blogB.Link)).Returns(_postsNewForBlogB.AsQueryable());
            _postRepositoryMock.Setup(pr => pr.Any(It.IsAny<Expression<Func<Post, bool>>>())).Returns(false);

            // Act
            _blogService.ExtractAndSaveAllNewBlogPosts();

            // Assert
            // Verify that:
            //  Add is called for Post object the correct number of times
            //  unit of work is committed for each blog
            _postRepositoryMock.Verify(p => p.Add(It.IsAny<Post>()), 
                                            Times.Exactly(_postsNewForBlogA.Length + _postsNewForBlogB.Length));            
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.Exactly(_blogs.Length));
        }

        [TestMethod]
        public void SaveBlogPostsSavesBlogPosts()
        {
            // Arrange                      
            IEnumerable<Post> posts = _postsBlogA.ToList();

            // Act
            _blogService.SaveBlogPosts(_blogA, posts);

            // Assert
            // Verify that:
            //  Add is called for Post object the correct number of times
            //  unit of work is committed at least once
            _postRepositoryMock.Verify(p => p.Add(It.IsAny<Post>()), Times.Exactly(_postsBlogA.Length));
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.AtLeastOnce);
        }

        [TestMethod]
        public void SaveNewBlogPostsSavesNewBlogPosts()
        {
            // Arrange                      
            IEnumerable<Post> posts = _postsNewForBlogB.ToList();
            _postRepositoryMock.Setup(pr => pr.Any(It.IsAny<Expression<Func<Post, bool>>>())).Returns(false);

            // Act
            _blogService.SaveNewBlogPosts(_blogB, posts);

            // Assert
            // Verify that:
            //  Add is called for Post object the correct number of times
            //  unit of work is committed at least once
            _postRepositoryMock.Verify(p => p.Add(It.IsAny<Post>()), Times.Exactly(_postsNewForBlogB.Length));
            _unitOfWorkMock.Verify(uow => uow.Commit(), Times.AtLeastOnce);
        }
    }
}