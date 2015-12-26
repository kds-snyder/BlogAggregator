using System.Collections.Generic;
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Models;

namespace BlogAggregator.Core.Services
{
    public interface IBlogService
    {
        void ExtractAndSaveBlogPosts(Blog blog);       
        void ExtractAndSaveNewBlogPosts(Blog blog);
        void ExtractAndSaveAllNewBlogPosts();
        IEnumerable<Post> ExtractBlogPosts(Blog blog);
        void SaveBlogPosts(int blogId, IEnumerable<Post> posts);
        void SaveNewBlogPosts(int blogId, IEnumerable<Post> posts);
    }
}