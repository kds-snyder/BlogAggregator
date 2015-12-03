using BlogAggregator.Core.Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Data.Infrastructure
{
    public class UserStore : IUserStore<User, int>,
                             IUserLoginStore<User, int>,
                             IUserPasswordStore<User, int>,
                             IUserSecurityStampStore<User, int>
    {
        private readonly IDatabaseFactory _databaseFactory;

        private BlogAggregatorDbContext _dataContext;
        protected BlogAggregatorDbContext DataContext
        {
            get
            {
                return _dataContext ?? (_dataContext = _databaseFactory.GetDataContext());
            }
        }

        public UserStore(IDatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        #region IUserStore
        public virtual Task CreateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.Factory.StartNew(() =>
            {
                DataContext.Users.Add(user);
                DataContext.SaveChanges();
            });
        }       

        public Task DeleteAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.Factory.StartNew(() =>
            {
                DataContext.Users.Remove(user);
                DataContext.SaveChanges();
            });
        }

        public Task<User> FindByIdAsync(int userId)
        {
            return Task.Factory.StartNew(() => DataContext.Users.Find(userId));
        }

        public Task<User> FindByNameAsync(string userName)
        {
            if (userName == null)
            {
                throw new ArgumentNullException(nameof(userName));
            }

            return Task.Factory.StartNew(() =>
                        DataContext.Users.FirstOrDefault
                            (u => u.UserName.ToLower() == userName.ToLower()));
        }

        public Task UpdateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.Factory.StartNew(() =>
            {
                DataContext.Users.Attach(user);
                DataContext.Entry(user).State = EntityState.Modified;
                DataContext.SaveChanges();
            });
        }

        public void Dispose()
        {
           
        }

        #endregion

        #region IUserLoginStore

        public Task AddLoginAsync(User user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            return Task.Factory.StartNew(() =>
            {
                DataContext.ExternalLogins.Add(new ExternalLogin
                {
                    UserID = user.Id,
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey
                });
                DataContext.SaveChanges();
            });
        }

        public Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            return Task.Factory.StartNew(() =>
            {
                var externalLogin = DataContext.ExternalLogins.FirstOrDefault
                                        (el => el.UserID == user.Id && el.LoginProvider == login.LoginProvider);
                DataContext.ExternalLogins.Remove(externalLogin);
                DataContext.SaveChanges();
            });
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.Factory.StartNew(() =>
                        (IList<UserLoginInfo>)DataContext.ExternalLogins.Where
                            (el => el.UserID == user.Id).
                            Select(el => new UserLoginInfo(el.LoginProvider, el.ProviderKey)).ToList());
        }

        public Task<User> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            return Task.Factory.StartNew(() =>
                        DataContext.Users.FirstOrDefault
                            (u => u.ExternalLogins
                            .Any(el => el.LoginProvider == login.LoginProvider &&
                                 el.ProviderKey == login.ProviderKey)));                           
        }
               
        #endregion

        #region IUserPasswordStore
       
        public Task<string> GetPasswordHashAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (passwordHash == null)
            {
                throw new ArgumentNullException(nameof(passwordHash));
            }

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);

        }
        #endregion

        #region IUserSecurityStampStore

        public Task<string> GetSecurityStampAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetSecurityStampAsync(User user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (stamp == null)
            {
                throw new ArgumentNullException(nameof(stamp));
            }

            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }
       
        #endregion
    }
}
