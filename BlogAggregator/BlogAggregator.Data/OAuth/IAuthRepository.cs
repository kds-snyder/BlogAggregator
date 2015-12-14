using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Data.OAuth
{
    public interface IAuthRepository : IDisposable
    {       
        Task<IdentityResult> AddLoginAsync(int userId, UserLoginInfo login);

        Task<IdentityResult> CreateAsync(User user);

        Task<User> FindAsync(UserLoginInfo loginInfo);

        Task<User> FindUser(string userName, string password);

        Task<IdentityResult> RegisterUser(RegistrationModel userModel);

    }
}
