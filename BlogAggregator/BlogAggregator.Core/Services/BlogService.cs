﻿using BlogAggregator.Core.BlogReader;
using BlogAggregator.Core.BlogReader.WordPress;
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Models;
using BlogAggregator.Core.Repository;
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

        public BlogService(
            IBlogRepository blogRepository,
            IPostRepository postRepository,
            IUnitOfWork unitOfWork,
            IWordPressBlogReader wordPressBlogReader)
        {
            _blogRepository = blogRepository;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _wordPressBlogReader = wordPressBlogReader;
        }

        // Extract posts of blog and save them in Post table
        public void ExtractAndSaveBlogPosts(BlogModel blog)
        {
            var blogPosts = ExtractBlogPosts(blog);

            SaveBlogPosts(blog.BlogID, blogPosts);
        }

        // Extract posts of blog, and save posts in Post table
        // that are not already in the Post table
        public void ExtractAndSaveNewBlogPosts(BlogModel blog)
        {
            var blogPosts = ExtractBlogPosts(blog);

            SaveNewBlogPosts(blog.BlogID, blogPosts);
        }

        // For all approved blogs, extract posts, and save posts in Post table         
        // that are not already in the Post table
        public void ExtractAndSaveAllNewBlogPosts()
        {
            // The blogs must be List type, because IQuueryable type keeps data connection open;
            //  if the type is IQueryable, then commit (save changes) to write posts later causes exception: 
            //  the transaction is not allowed because there are other threads running in the session
            List<Blog> blogs = _blogRepository.GetAll().ToList();

            if (blogs.Count() > 0)
            {
                foreach (var blog in blogs)
                {
                    if (blog.Approved)
                    {
                        var blogModel = new BlogModel
                        {
                            BlogID = blog.BlogID,
                            BlogType = blog.BlogType,
                            CreatedDate = blog.CreatedDate,
                            Approved = blog.Approved,
                            AuthorEmail = blog.AuthorEmail,
                            AuthorName = blog.AuthorName,
                            Description = blog.Description,
                            Link = blog.Link,
                            Title = blog.Title
                        };
                        ExtractAndSaveNewBlogPosts(blogModel);
                    }
                }
            }          
        }

        // Extract blog posts according to blog type 
        public IEnumerable<Post> ExtractBlogPosts(BlogModel blog)
        {
            switch (blog.BlogType)
            {
                case BlogTypes.WordPress:
                    return extractBlogPosts(_wordPressBlogReader, blog);
                default:
                    throw new ArgumentException(nameof(blog));
            }
        }

        // Extract blog posts
        private IEnumerable<Post> extractBlogPosts(IBlogReader reader, BlogModel blog)
        {
            if (reader.VerifyBlog(blog))
            {
                return reader.GetBlogPosts(blog);
            }
            else
            {
                throw new Exception("Blog at " + blog.Link + " could not be verified for extraction");
            }
        }

        // Save blog posts in Post table
        public void SaveBlogPosts(int blogId, IEnumerable<Post> posts)
        {
            foreach (var post in posts)
            {
                _postRepository.Add(new Post
                {
                    BlogID = blogId,
                    Content = post.Content,
                    Description = post.Description,
                    Guid = post.Guid,
                    Link = post.Link,
                    PublicationDate = post.PublicationDate,
                    Title = post.Title
                });
            }

            _unitOfWork.Commit();
        }

        // Save blog posts that are not already in Post table
        public void SaveNewBlogPosts(int blogId, IEnumerable<Post> posts)
        {
            bool savedPosts = false;
            foreach (var post in posts)
            {
                // Add post to Post table if its Guid does not match any post
                if (!_postRepository.Any(p => p.Guid == post.Guid))
                {
                    savedPosts = true;
                    _postRepository.Add(new Post
                    {
                        BlogID = blogId,
                        Content = post.Content,
                        Description = post.Description,
                        Guid = post.Guid,
                        Link = post.Link,
                        PublicationDate = post.PublicationDate,
                        Title = post.Title
                    });
                }
            }

            if (savedPosts)
            {
                _unitOfWork.Commit();

            }
        }
    }
}
