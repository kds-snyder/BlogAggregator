
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlogAggregator.Core.BlogReader.WordPress;
using BlogAggregator.Core.Domain;
using System.Collections.Generic;
using BlogAggregator.Core.BlogReader;

namespace BlogAggregator.Core.Test.BlogReader
{
    [TestClass]
    public class WordPressBlogReaderTests
    {
        // Real blog links
        string[] _blogLinks =
            {
                "bracken.design",
                "jeffwarddevelopment.com/index.php",
                "kdssnyder.wordpress.com",
                "zakdietzen.com"
            };

        [TestMethod]
        public void GetRealBlogPostsReturnsPosts()
        {
            //Arrange
            var wordPressBlogReader = new WordPressBlogReader();
            var posts = new List<Post>();

            for (int i = 0; i < _blogLinks.Length; i++)
            {
                //Act
                posts = wordPressBlogReader.GetBlogPosts(_blogLinks[i]) as List<Post>;

                //Assert
                if (posts.Count == 0) Assert.Inconclusive("No posts found for " + _blogLinks[i]);
                Assert.IsTrue(posts[0].Link.Contains(_blogLinks[i]));
            }
        }

        [TestMethod]
        public void VerifyRealBlogReturnsNotNull()
        {
            //Arrange
            var wordPressBlogReader = new WordPressBlogReader();
            BlogInfo blogInfo;

            for (int i = 0; i < _blogLinks.Length; i++)
            {
                //Act
                blogInfo = wordPressBlogReader.VerifyBlog(_blogLinks[i]);

                //Assert
                Assert.IsNotNull(blogInfo);
            }
        }

        [TestMethod]
        public void VerifyNonExistentBlogReturnsNull()
        {
            //Arrange
            var wordPressBlogReader = new WordPressBlogReader();

            //Act
            BlogInfo blogInfo = wordPressBlogReader.VerifyBlog("someblogXXX");

            //Assert
            Assert.IsNull(blogInfo);
        }
    }
}
