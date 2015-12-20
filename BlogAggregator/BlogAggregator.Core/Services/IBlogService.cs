using System.Collections.Generic;
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Models;

namespace BlogAggregator.Core.Services
{
    public interface IBlogService
    {
        void ExtractAndSaveBlogPosts(BlogModel blog);       
        void ExtractAndSaveNewBlogPosts(BlogModel blog);
        void ExtractAndSaveAllNewBlogPosts();
        IEnumerable<Post> ExtractBlogPosts(BlogModel blog);
        void SaveBlogPosts(int blogId, IEnumerable<Post> posts);
        void SaveNewBlogPosts(int blogId, IEnumerable<Post> posts);
    }
}