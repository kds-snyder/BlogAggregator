
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
            var blogModel = new BlogModel
            {
                BlogID = blogIDForTest,
                BlogType = BlogTypes.WordPress,
                AuthorEmail = "a@b.com",
                AuthorName = "Testy Testerson",
                Approved = true,
                CreatedDate = DateTime.Now,
                Description = "Great Blog",
                Link = "bracken.design",  // Must be real blog link
                Title = "Stupendous Blog"
            };

            //Act
            posts = wordPressBlogReader.GetBlogPosts(blogModel) as List<Post>;

            //Assert
            Assert.IsTrue(posts.Count > 0);
            Assert.IsTrue(posts[0].Link.Contains("bracken.design"));
        }
    }
}
