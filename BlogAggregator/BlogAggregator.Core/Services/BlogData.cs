using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Services
{
    public class BlogData : IDisposable
    {
        private BlogAggregatorDbContext db = new BlogAggregatorDbContext();

        // Write the posts in the input posts to the database
        // Return true if successful, otherwise return false
        public bool SavePosts(List<Post> posts, int blogID)
        {
            bool result = true;

            foreach (var post in posts)
            {
                db.Posts.Add(new Post
                {
                    BlogID = blogID,
                    Content = post.Content,
                    Description = post.Description,
                    Link = post.Link,
                    PublicationDate = post.PublicationDate,
                    Title = post.Title
                });
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {

                result = false;
            }

            return result;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
