﻿using BlogAggregator.Core.Models;
using System;
using System.Collections.Generic;

namespace BlogAggregator.Core.Domain
{
    // Blog table
    public class Blog
    {       
        public int BlogID { get; set; }

        public BlogTypes BlogType { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool Approved { get; set; }

        public string AuthorEmail { get; set; }

        public string AuthorName { get; set; }

        public string Description { get; set; }
                
        public string Link { get; set; }        

        public string Title { get; set; }

        // Blog can have many posts
        public virtual ICollection<Post> Posts { get; set; }
      
        public void Update(BlogModel blog)
        {
            if (blog.BlogID == 0)
            {
                CreatedDate = DateTime.Now;
            }           
            Approved = blog.Approved;
            AuthorName = blog.AuthorName;
            AuthorEmail = blog.AuthorEmail;
            BlogType = blog.BlogType;
            Description = blog.Description;
            Link = blog.Link;
            Title = blog.Title;
        }
    }
}
