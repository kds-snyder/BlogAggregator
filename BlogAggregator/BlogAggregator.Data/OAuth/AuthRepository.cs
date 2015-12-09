using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Models;
using BlogAggregator.Data.Infrastructure;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAggregator.Data.OAuth
{
    public class AuthRepository : IAuthRepository, IDisposable
    {
        private readonly IDatabaseFactory _databaseFactory;
        private UserManager<User, int> _userManager;
        private readonly IUserStore<User, int> _userStore;

        private BlogAggregatorDbContext _dataContext;
        protected BlogAggregatorDbContext DataContext
        {
            get
            {
                return _dataContext ?? (_dataContext = _databaseFactory.GetDataContext());
            }
        }

        public AuthRepository(IDatabaseFactory databaseFactory, IUserStore<User, int> userStore)
        {
            _userStore = userStore;
            _databaseFactory = databaseFactory;
            _userManager = new UserManager<User, int>(userStore);
        }

        public async Task<IdentityResult> AddLoginAsync(int userId, UserLoginInfo login)
        {
            var result = await _userManager.AddLoginAsync(userId, login);

            return result;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var existingToken = _dataContext.RefreshTokens.SingleOrDefault(r => r.Subject == token.Subject &&
                                                                        r.ClientId == token.ClientId);
            
            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            _dataContext.RefreshTokens.Add(token);

            return await _dataContext.SaveChangesAsync() > 0;
        }

        public async Task<IdentityResult> CreateAsync(User user)
        {
            var result = await _userManager.CreateAsync(user);

            return result;
        }

        public async Task<User> FindAsync(UserLoginInfo loginInfo)
        {
            User user = await _userManager.FindAsync(loginInfo);

            return user;
        }

        public Client FindClient(string clientId)
        {
            var client = _dataContext.Clients.Find(clientId);

            return client;
        }

        public async Task<User> FindUser(string userName, string password)
        {
            return await _userManager.FindAsync(userName, password);

        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            return await Task.Factory.StartNew(() =>
            {
                var refreshToken = _dataContext.RefreshTokens.Find(refreshTokenId);

                return refreshToken;
            });
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return _dataContext.RefreshTokens.ToList();
        }

        public async Task<IdentityResult> RegisterUser(RegistrationModel registrationModel)
        {
            User user = new User
            {
                UserName = registrationModel.UserName
            };

            var result = await _userManager.CreateAsync(user, registrationModel.Password);

            return result;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = _dataContext.RefreshTokens.Find(refreshTokenId);

            if (refreshToken != null)
            {
                return await RemoveRefreshToken(refreshToken);
            }

            return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            _dataContext.RefreshTokens.Remove(refreshToken);
            return await _dataContext.SaveChangesAsync() > 0;
        }       

        public void Dispose()
        {
            _userManager.Dispose();

        }
    }
}
