using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Domain
{
    // Blog post table
    public class Post
    {
        public int PostID { get; set; }

        public int BlogID { get; set; }

        public string Content { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        public DateTime PublicationDate { get; set; }       

        public string Title { get; set; }

        // Post corresponds to just one blog
        public virtual Blog Blog { get; set; }
    }
}
