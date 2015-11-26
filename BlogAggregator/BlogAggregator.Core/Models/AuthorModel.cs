using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Models
{
    public class AuthorModel
    {
        public int AuthorID { get; set; }

        public int BlogID { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
    }
}
