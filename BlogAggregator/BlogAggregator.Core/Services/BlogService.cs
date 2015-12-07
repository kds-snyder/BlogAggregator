using BlogAggregator.Core.BlogReader;
using BlogAggregator.Core.BlogReader.WordPress;
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Models;
using BlogAggregator.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Services
{
    public class BlogService : IBlogService
    {
        private readonly IWordPressBlogReader _wordPressBlogReader;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BlogService(
            IWordPressBlogReader wordPressBlogReader, 
            IPostRepository postRepository, 
            IUnitOfWork unitOfWork)
        {
            _wordPressBlogReader = wordPressBlogReader;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
        }
 
        public void ExtractAndSaveBlogPosts (BlogModel blog)
        {
            var blogPosts = ExtractBlogPosts(blog);

            SaveBlogPosts(blog.BlogID, blogPosts);
        }

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

        public void SaveBlogPosts(int blogId, IEnumerable<Post> posts)
        {
            foreach (var post in posts)
            {
                _postRepository.Add(new Post
                {
                    BlogID = blogId,
                    Content = post.Content,
                    Description = post.Description,
                    Link = post.Link,
                    PublicationDate = post.PublicationDate,
                    Title = post.Title
                });
            }

            _unitOfWork.Commit();
        }

        private IEnumerable<Post> extractBlogPosts(IBlogReader reader, BlogModel blog)
        {
            if(reader.VerifyBlog(blog))
            {
                return reader.GetBlogPosts(blog);
            }
            else
            {
                throw new Exception("Blog could not be verified for extraction");
            }
        }
    }
}
