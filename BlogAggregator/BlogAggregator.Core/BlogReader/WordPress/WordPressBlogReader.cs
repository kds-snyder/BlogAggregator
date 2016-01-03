using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Services;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace BlogAggregator.Core.BlogReader.WordPress
{
    // Handle Word Press blogs
    public class WordPressBlogReader : IWordPressBlogReader
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();

        // Retrieve blog description and title from blog Website
        //  according to input blog link, and return in BlogInfo object
        // Return null if unable to get blog info
        public BlogInfo VerifyBlog(string blogLink)
        {           

            // Get the blog feed
            string feedData = getBlogFeed(blogLink);

            // If blog feed was obtained, parse the blog information
            if (feedData != "")
            {
                return parseBlogInfo(feedData, blogLink);
            }
            else
            {
                return null;
            }
        }

        // Parse the blog posts,
        //  and return a list of the posts       
        public IEnumerable<Post> GetBlogPosts(string blogLink)
        {
            List<Post> blogPosts = new List<Post>();
            bool done = false;
            HttpStatusCode HTTPresult;
            int postPage = 1;

            while (!done)
            {
                // Get blog feed
                string feedData = getBlogFeed(blogLink, out HTTPresult, postPage);

                if (HTTPresult != HttpStatusCode.OK)
                {
                    if (HTTPresult == HttpStatusCode.NotFound)
                    {
                        done = true;
                    }
                    else
                    {
                        _logger.Error("Unable to get blog feed from {0}, postPage: {1}, HTTP status code: {2}",
                                        blogLink, postPage, HTTPresult);
                        throw new Exception("Unable to get feed from blog at " + blogLink +
                                                ", postPage: " + postPage +
                                                ", HTTP status code: " + HTTPresult);
                    }
                }
                else
                {
                    // Parse the posts and append them to blogPosts
                    // Increment post page counter                    
                    blogPosts.AddRange(parseBlogPosts(blogLink, feedData));
                    ++postPage;
                }
            }

            return blogPosts;
        }

        // Get the blog feed and return the HTTP status code
        // input postPage: 1 indicates get blog information or the recent posts (page 1); 
        //   value greater than 1 indicates earlier posts (page 2, 3,...)
        private string getBlogFeed(string blogLink, out HttpStatusCode HTTPresult, int postPage = 1)
        {
            string feedUrl = getFeedUrl(blogLink, postPage);
            string feedData = WebData.Instance.GetWebData(feedUrl, out HTTPresult);

            return feedData;
        }

        // Overload for getBlogFeed if HTTP status code is not needed
        private string getBlogFeed(string blogLink, int postPage = 1)
        {
            HttpStatusCode HTTPresult;
            return getBlogFeed(blogLink, out HTTPresult, postPage);
        }

        // Get the URL of the blog RSS feed from the blog link
        // The blog feed URL is <blogLink>/feed for the most recent posts,
        //  and for earlier posts the feed URL is <blogLink>/feed/?paged=2, <blogLink>/feed/?paged=3, etc.
        private string getFeedUrl(string blogLink, int postPage)
        {
            string feedUrl;

            // Append 'feed' or '/feed', depending on whether input ends with '/'
            if (blogLink[blogLink.Length - 1] == '/')
            {
                feedUrl = blogLink + "feed";
            }
            else
            {
                feedUrl = blogLink + "/feed";
            }

            if (postPage > 1)
            {
                feedUrl = feedUrl + "/?paged=" + postPage;
            }

            return feedUrl;
        }

        // Get the blog information from the input XML data,
        //  and return it as BlogInfo object
        private BlogInfo parseBlogInfo(string xmlData, string blogLink)
        {
            try
            {
                using (var xmlStream = xmlData.ToStream())
                {
                    XDocument xmlDoc = XDocument.Load(xmlStream);

                    // Get the blog information XML node
                    var blogInfoXML = xmlDoc.Element("rss").Element("channel");

                    // Get the blog description and title 
                    var blogInfo = new BlogInfo();
                    blogInfo.Description = blogInfoXML.Element("description").Value.ScrubHtml();
                    blogInfo.Title = blogInfoXML.Element("title").Value.ScrubHtml();
                    return blogInfo;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to parse blog info from {0}\nException: {1}\nStackTrace: {2}",
                                        blogLink, ex.Message, ex.StackTrace);
                return null;
            }
        }

        // Get the blog posts from the input feed XML data,
        //  and return as list of Post objects        
        private List<Post> parseBlogPosts(string feedLink, string xmlData)
        {
            var posts = new List<Post>();

            try
            {
                using (var xmlStream = xmlData.ToStream())
                {
                    XDocument xmlDoc = XDocument.Load(xmlStream);

                    // Get the namespace to use for the content tag
                    XNamespace contentNameSpace = getContentNameSpace(xmlDoc);

                    // Get the blog posts from the XML document
                    // They are in <item> tags, which are under <channel> under <rss> 
                    posts =
                       xmlDoc.Element("rss").Element("channel").Descendants("item").Select(post => new Post
                       {
                           Content = post.Element(contentNameSpace + "encoded").Value.ScrubHtml(),
                           Description = post.Element("description").Value.ScrubHtml().AdjustContent(),
                           Guid = post.Element("guid").Value,
                           Link = post.Element("link").Value,
                           PublicationDate = Convert.ToDateTime(post.Element("pubDate").Value),
                           Title = post.Element("title").Value.ScrubHtml()
                       }).ToList();
                }
                return posts;
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to parse blog posts from {0}\nException: {1}\nStackTrace: {2}",
                                        feedLink, ex.Message, ex.StackTrace);
                throw new Exception("Unable to parse blog posts from " + feedLink);
            }
        }

        // Get the content namespace specified in the input XML document,
        // from the <rss> tag, attribute xmlns:content
        // If xmlns:content not found, use default value
        private XNamespace getContentNameSpace(XDocument xmlDoc)
        {
            XNamespace contentNameSpace = "http://purl.org/rss/1.0/modules/content/";

            var contentAttrib = xmlDoc.Element("rss").Attributes()
                                 .Where(attrib => attrib.IsNamespaceDeclaration &&
                                   attrib.Name.LocalName == "content").FirstOrDefault();

            if (contentAttrib != null) contentNameSpace = contentAttrib.Value;

            return contentNameSpace;
        }

    }
}
