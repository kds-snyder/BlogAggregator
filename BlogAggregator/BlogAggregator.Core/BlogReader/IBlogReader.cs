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
        BlogInfo VerifyBlog(string blogLink);
        IEnumerable<Post> GetBlogPosts(string blogLink);
    }
}
