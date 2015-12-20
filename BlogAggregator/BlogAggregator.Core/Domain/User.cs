using BlogAggregator.Core.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;

namespace BlogAggregator.Core.Domain
{
    public class User : IUser<int>
    {
        public int Id { get; set; }
        public bool Authorized { get; set; }        
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string UserName { get; set; }

        public virtual ICollection<ExternalLogin> ExternalLogins { get; set; }

        public void Update(UserModel user)
        {
            Authorized = user.Authorized;
            PasswordHash = user.PasswordHash;
            SecurityStamp = user.SecurityStamp;
            UserName = user.UserName;
        }
    }
}
