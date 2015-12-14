using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public bool Authorized { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string UserName { get; set; }
    }
}
