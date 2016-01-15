using BlogAggregator.Core.Domain;
using System;

namespace BlogAggregator.Core.Models
{
    public class BlogModel
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

        public string FixedLink
        {
            get
            {
                return Link.FixWebUrl();
            }
        }
    }
}
