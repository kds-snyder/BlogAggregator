using System.Collections.Generic;
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Models;

namespace BlogAggregator.Core.Services
{
    public interface IBlogService
    {
        void ExtractAndSaveBlogPosts(BlogModel blog);
        IEnumerable<Post> ExtractBlogPosts(BlogModel blog);
        void SaveBlogPosts(int blogId, IEnumerable<Post> posts);
    }
}