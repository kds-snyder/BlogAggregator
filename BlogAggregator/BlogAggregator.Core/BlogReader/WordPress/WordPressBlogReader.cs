using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace BlogAggregator.Core.BlogReader.WordPress
{
    // Handle Word Press blogs
    public class WordPressBlogReader : IWordPressBlogReader
    {      
        // Retrieve blog description and title from blog Website
        //  according to input blog link, and return in BlogInfo object
        // Return null if unable to get blog info
        public BlogInfo VerifyBlog(string blogLink)
        {            
            // Get the XML of the blog
            string xmlData = getBlogXML(blogLink);

            // If blog XML was obtained, parse the blog information
            if (xmlData != "")
            {
                return parseBlogInfo(xmlData, blogLink);               
            }
            else
            {
                return null;
            }

        }

        // Parse the posts corresponding to blog,
        //  and return a list of the posts       
        public IEnumerable<Post> GetBlogPosts(string blogLink)
        {
            List<Post> blogPosts = new List<Post>();

            // Get the XML of the blog
            string xmlData = getBlogXML(blogLink);

            // If blog XML was obtained, parse the blog information
            // and store in the blog record
            if (xmlData != "")
            {
                blogPosts = parseBlogPosts(xmlData);
            }
            return blogPosts;
        }

        // Get the XML string from the blog RSS feed
        private string getBlogXML(string blogLink)
        {
            string xmlUrl = getXMLUrl(blogLink);
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
        //  and return it as BlogInfo object
        // Return null if unable to get the blog info
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
            catch (Exception e)
            {
                return null;
            }          
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
                           Guid = post.Element("guid").Value,
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
