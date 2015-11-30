using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Models
{
    public class BlogModel
    {
        public int BlogID { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool Approved { get; set; }

        public string AuthorEmail { get; set; }

        public string AuthorName { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        public string Title { get; set; }
    }
}
