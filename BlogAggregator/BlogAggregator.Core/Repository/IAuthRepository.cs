using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Repository
{
    public interface IAuthRepository : IDisposable
    {
        Task<IdentityResult> RegisterUser(RegistrationModel userModel);
        Task<User> FindUser(string userName, string password);
    }
}
