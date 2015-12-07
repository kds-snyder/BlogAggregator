using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Models;
using BlogAggregator.Data.Infrastructure;
using BlogAggregator.Data.Repository;
using Microsoft.AspNet.Identity;
using System;
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

        public async Task<IdentityResult> RegisterUser(RegistrationModel registrationModel)
        {
            User user = new User
            {
                UserName = registrationModel.UserName
            };

            var result = await _userManager.CreateAsync(user, registrationModel.Password);

            return result;
        }

        public async Task<User> FindUser(string userName, string password)
        {
            return await _userManager.FindAsync(userName, password);

        }

        public void Dispose()
        {
            _userManager.Dispose();

        }
    }
}
