using BlogAggregator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Domain
{
    public class ExternalLogin
    {
        public int ExternalLoginID { get; set; }

        public int UserID {get; set;}

        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public virtual User User { get; set; }

        public void Update(ExternalLoginModel externalLogin)
        {
            UserID = externalLogin.UserID;
            LoginProvider = externalLogin.LoginProvider;
            ProviderKey = externalLogin.ProviderKey;
        }
    }
}
