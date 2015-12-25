
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlogAggregator.Core.BlogReader.WordPress;
using BlogAggregator.Core.Domain;
using System.Collections.Generic;
using BlogAggregator.Core.Models;

namespace BlogAggregator.Core.Test.BlogReader
{
    [TestClass]
    public class WordPressBlogReaderTests
    {
        [TestMethod]
        public void GetBlogPostsReturnsPosts()
        {
            //Arrange
            int blogIDForTest = 1;
            var wordPressBlogReader = new WordPressBlogReader();
            var posts = new List<Post>();

            // Real blog links
            string[] blogLinks =
                { "bracken.design",
                   "jeffwarddevelopment.com/index.php",
                   "kdssnyder.wordpress.com",
                   "zakdietzen.com"
            };
            var blogModel = new BlogModel
            {
                BlogID = blogIDForTest,
                BlogType = BlogTypes.WordPress,
                AuthorEmail = "a@b.com",
                AuthorName = "Testy Testerson",
                Approved = true,
                CreatedDate = DateTime.Now,
                Description = "Great Blog",
                Link = "",
                Title = "Stupendous Blog"
            };

            for (int i = 0; i < blogLinks.Length; i++)
            {
                //Act
                blogModel.Link = blogLinks[i];
                posts = wordPressBlogReader.GetBlogPosts(blogModel) as List<Post>;

                //Assert
                if (posts.Count == 0) Assert.Inconclusive("No posts found for " + blogLinks[i]);
                Assert.IsTrue(posts[0].Link.Contains(blogLinks[i]));
            }
        }
    }
}
