using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.BlogReader
{
    public interface IBlogReader
    {
        bool VerifyBlog(BlogModel blog);
        IEnumerable<Post> GetBlogPosts(BlogModel blog);
    }
}
