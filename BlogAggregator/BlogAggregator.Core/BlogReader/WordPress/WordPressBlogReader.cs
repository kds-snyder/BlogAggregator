using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Models;
using BlogAggregator.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlogAggregator.Core.BlogReader.WordPress
{
    // Handle Word Press blogs
    public class WordPressBlogReader : IWordPressBlogReader
    {
        // Retrieve blog description and title from blog Website
        //  according to blog link, and store in the blog record
        // Return true if able to get the info, otherwise return false
        public bool VerifyBlog(BlogModel blog)
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
        //  and return a list of the posts       
        public IEnumerable<Post> GetBlogPosts(BlogModel blog)
        {
            List<Post> blogPosts = new List<Post>();

            // Get the XML of the blog
            string xmlData = getBlogXML(blog);

            // If blog XML was obtained, parse the blog information
            // and store in the blog record
            if (xmlData != "")
            {
                blogPosts = parseBlogPosts(xmlData);
            }

            return blogPosts;
        }

        // Get the XML string from the blog RSS feed
        private string getBlogXML(BlogModel blog)
        {
            string xmlUrl = getXMLUrl(blog.Link);
            string xmlData = WebData.Instance.GetWebDataFixUrl(xmlUrl);
            return xmlData;
        }

        // Get the URL of the blog RSS feed from the blog link
        private string getXMLUrl(string inputUrl)
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
        private bool parseBlogInfo(string xmlData, BlogModel blog)
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
                    blog.Description = blogInfoXML.Element("description").Value.ScrubHtml();
                    blog.Link = blogInfoXML.Element("link").Value;
                    blog.Title = blogInfoXML.Element("title").Value.ScrubHtml();
                }
            }
            catch (Exception e)
            {
                result = false;
            }

            return result;
        }

        // Get the blog posts from the input XML data,
        //  and return as list of Post objects        
        private List<Post> parseBlogPosts(string xmlData)
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
                           Link = post.Element("link").Value,
                           PublicationDate = Convert.ToDateTime(post.Element("pubDate").Value),
                           Title = post.Element("title").Value.ScrubHtml()
                       }).ToList();                                           
                }
            }
            catch (Exception e)
            {
                // If error occurred, ensure that null object is returned
                if (posts != null)
                {
                    posts = new List<Post>();
                }
            }           

            return posts;
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
