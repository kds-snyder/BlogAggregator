using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlogAggregator.Core.Services
{
    // Handle Word Press blogs
    public static class BlogWebDataWP 
    {

        // Retrieve blog description and title from blog Website
        //  according to blog link, and store in the blog record
        // Return true if able to get the info, otherwise return false
        public static bool GetBlogInformation(BlogModel blog)
        {
            bool result = false;

            // Get the XML of the blog
            string xmlData = getBlogXML(blog);

            // If blog XML was obtained, parse the blog information
            // and store in the blog record
            if (xmlData != "")
            {
                result = parseBlogInfo(xmlData, blog);
            }

            return result;
        }

        // Parse the posts corresponding to blog,
        //  and store them in the DB
        public static bool GetBlogPosts(BlogModel blog)
        {
            bool result = false;

            // Get the XML of the blog
            string xmlData = getBlogXML(blog);

            // If blog XML was obtained, parse the blog information
            // and store in the blog record
            if (xmlData != "")
            {
                result = parseBlogPosts(xmlData, blog.BlogID);
            }

            return result;
        }

        // Get the XML string from the blog RSS feed
        private static string getBlogXML(BlogModel blog)
        {
            string xmlUrl = getXMLUrl(blog.Link);
            string xmlData = WebData.GetWebData(xmlUrl);
            return xmlData;
        }

        // Get the URL of the blog RSS feed from the blog link
        private static string getXMLUrl(string inputUrl)
        {
            string XMLUrl;

            // Append 'feed' or '/feed', depending on whether input ends with '/'
            if (inputUrl[inputUrl.Length - 1] == '/')
            {
                XMLUrl = inputUrl + "feed";
            }
            else
            {
                XMLUrl = inputUrl + "/feed";
            }

            return XMLUrl;
        }

        // Get the blog information from the input XML data,
        //  and set it in the blog record
        // Return true if no error occurs, otherwise return false
        private static bool parseBlogInfo(string xmlData, BlogModel blog)
        {
            bool result = true;

            try
            {
                using (var xmlStream = xmlData.ToStream())
                {
                    XDocument xmlDoc = XDocument.Load(xmlStream);

                    // Get the blog information XML node
                    var blogInfoXML = xmlDoc.Element("rss").Element("channel");

                    // Get the blog information and store it in the blog record  
                    blog.Description = blogInfoXML.Element("description").Value;
                    blog.Title = blogInfoXML.Element("title").Value;
                }
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;
        }

        // Get the blog posts from the input XML data,
        //  and write them in the database
        // Return true if no error occurs, otherwise return false
        private static bool parseBlogPosts(string xmlData, int blogID)
        {
            bool result = true;
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
                           Content = post.Element(contentNameSpace + "encoded").Value,
                           Description = post.Element("description").Value,
                           Link = post.Element("link").Value,
                           PublicationDate = Convert.ToDateTime(post.Element("pubDate").Value),
                           Title = post.Element("title").Value
                       }).ToList();

                    // Write the posts to the DB
                    using (var blogData = new BlogData())
                    {
                        result = blogData.SavePosts(posts, blogID);
                    }                   
                }
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;
        }

        // Get the content namespace specified in the input XML document,
        // from the <rss> tag, attribute xmlns:content
        // If xmlns:content not found, use default value
        private static XNamespace getContentNameSpace(XDocument xmlDoc)
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
