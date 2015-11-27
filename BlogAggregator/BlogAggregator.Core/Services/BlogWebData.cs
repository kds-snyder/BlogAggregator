using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Services
{
    public class BlogWebData : IDisposable
    {
        private BlogAggregatorDbContext db = new BlogAggregatorDbContext();

        // Retrieve blog description and title from Word Press blog Website
        //  according to blog link, and store in the blog record
        public void GetBlogInformationWP(BlogModel blog)
        {

        }

        // Parse the posts corresponding to Word Press blog,
        //  and store them in the DB
        public void ParseBlogPostsWP(BlogModel blog)
        {

        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
