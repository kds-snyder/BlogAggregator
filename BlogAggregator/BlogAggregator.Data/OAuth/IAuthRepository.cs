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
        Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login);

        Task<bool> AddRefreshToken(RefreshToken token);

        Task<IdentityResult> CreateAsync(User user);

        Task<User> FindAsync(UserLoginInfo loginInfo);

        Client FindClient(string clientId);

        Task<RefreshToken> FindRefreshToken(string refreshTokenId);

        Task<User> FindUser(string userName, string password);

        //List<RefreshToken> GetAllRefreshTokens();
        IEnumerable<RefreshToken> GetAllRefreshTokens();

        Task<IdentityResult> RegisterUser(RegistrationModel userModel);

        Task<bool> RemoveRefreshToken(string refreshTokenId);

        Task<bool> RemoveRefreshToken(RefreshToken refreshToken);
    }
}
