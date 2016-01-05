using BlogAggregator.Core.BlogReader;
using BlogAggregator.Core.BlogReader.WordPress;
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlogAggregator.Core.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWordPressBlogReader _wordPressBlogReader;
        //private readonly ILogger _logger;
        private static ILogger _logger;

        public BlogService(
            IBlogRepository blogRepository,
            IPostRepository postRepository,
            IUnitOfWork unitOfWork,
            IWordPressBlogReader wordPressBlogReader)
            //IWordPressBlogReader wordPressBlogReader,
            //ILogger logger)
        {
            _blogRepository = blogRepository;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _wordPressBlogReader = wordPressBlogReader;
            //_logger = logger;
            _logger = LogManager.GetCurrentClassLogger();
        }

        // Extract posts of blog and save them in Post table
        public void ExtractAndSaveBlogPosts(Blog blog)
        {
            var blogPosts = ExtractBlogPosts(blog);

            _logger.Trace("Extracted {0} posts from {1}",
                            blogPosts.Count(), blog.Link);

            SaveBlogPosts(blog, blogPosts);
        }

        // Extract posts of blog, and save posts in Post table
        // that are not already in the Post table
        public void ExtractAndSaveNewBlogPosts(Blog blog)
        {
            var blogPosts = ExtractBlogPosts(blog);

            _logger.Trace("Extracted {0} posts from {1}",
                            blogPosts.Count(), blog.Link);

            SaveNewBlogPosts(blog, blogPosts);
        }

        // For all approved blogs, extract posts, and save posts in Post table         
        // that are not already in the Post table
        public void ExtractAndSaveAllNewBlogPosts()
        {
            // In below statement, blogs must be List type, because IQuueryable type keeps data connection open;
            //  if the type is IQueryable, then commit to write posts later causes exception: 
            //  the transaction is not allowed because there are other threads running in the session
            List<Blog> blogs = _blogRepository.GetAll().ToList();

            if (blogs.Count() > 0)
            {
                foreach (var blog in blogs)
                {
                    if (blog.Approved)
                    {                        
                        ExtractAndSaveNewBlogPosts(blog);
                    }
                }
            }          
        }

        // Extract blog posts according to blog type 
        public IEnumerable<Post> ExtractBlogPosts(Blog blog)
        {
            switch (blog.BlogType)
            {
                case BlogTypes.WordPress:
                    return extractBlogPosts(_wordPressBlogReader, blog.Link);

                default:
                    throw new ArgumentException(nameof(blog));
            }
        }

        // Extract blog posts
        private IEnumerable<Post> extractBlogPosts(IBlogReader reader, string blogLink)
        {
            BlogInfo bloginfo = reader.VerifyBlog(blogLink);
            if (bloginfo != null)
            {
                _logger.Trace("Getting posts from {0}", blogLink);

                return reader.GetBlogPosts(blogLink);
            }
            else
            {
                throw new Exception("blog at " + blogLink + " could not be verified for extraction");
            }
        }

        // Save blog posts in Post table
        public void SaveBlogPosts(Blog blog, IEnumerable<Post> posts)
        {
            foreach (var post in posts)
            {
                _postRepository.Add(new Post
                {
                    BlogID = blog.BlogID,
                    Content = post.Content,
                    Description = post.Description,
                    Guid = post.Guid,
                    Link = post.Link,
                    PublicationDate = post.PublicationDate,
                    Title = post.Title
                });
            }

            _unitOfWork.Commit();
            _logger.Trace("Saved {0} posts for blog {1}", posts.Count(), blog.Link);
        }

        // Save blog posts that are not already in Post table
        public void SaveNewBlogPosts(Blog blog, IEnumerable<Post> posts)
        {
            int savedPosts = 0;
            foreach (var post in posts)
            {
                // Add post to Post table if its Guid does not match any post
                if (!_postRepository.Any(p => p.Guid == post.Guid))
                {
                    ++savedPosts;
                    _postRepository.Add(new Post
                    {
                        BlogID = blog.BlogID,
                        Content = post.Content,
                        Description = post.Description,
                        Guid = post.Guid,
                        Link = post.Link,
                        PublicationDate = post.PublicationDate,
                        Title = post.Title
                    });
                }
            }

            if (savedPosts > 0)
            {
                _unitOfWork.Commit();               
            }
            _logger.Trace("Saved {0} new posts for blog {1}", savedPosts, blog.Link);
        }
    }
}
