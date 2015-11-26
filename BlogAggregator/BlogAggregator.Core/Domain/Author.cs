using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogAggregator.Core.Models;

namespace BlogAggregator.Core.Domain
{
    // Blog author table
    public class Author
    {
        public int AuthorID { get; set; }

        public int BlogID { get; set; }

        public DateTime CreatedDate { get; set; }
       
        public string Email { get; set; }

        public string Name { get; set; }

        // Author can have many blogs
        public virtual ICollection<Blog> Blogs { get; set; }

        public void Update(AuthorModel author)
        {
            if (author.AuthorID == 0)
            {
                CreatedDate = DateTime.Now;
            }

            BlogID = author.BlogID;
            Email = author.Email;
            Name = author.Name;
        }
    }
}
