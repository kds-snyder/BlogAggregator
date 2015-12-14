using BlogAggregator.Data.OAuth;
using System;
using System.Web.Http;

namespace BlogAggregator.API.Controllers.OAuth
{
    [RoutePrefix("api/RefreshTokens")]
    public class RefreshTokensController : ApiController
    {
        //private Func<IAuthRepository> _authRepositoryFactory;
        //private IAuthRepository _authRepository
        //{
        //    get
        //    {
        //        return _authRepositoryFactory.Invoke();
        //    }
        //}
        private readonly IAuthRepository _authRepository;


        //public RefreshTokensController(Func<IAuthRepository> authRepositoryFactory)
        //{
        //    _authRepositoryFactory = authRepositoryFactory;
        //}
        public RefreshTokensController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        //[Authorize(Users = "Admin")]
        [AllowAnonymous]
        [Route("")]
        public async System.Threading.Tasks.Task<IHttpActionResult> Delete(string tokenId)
        {
            var result = await _authRepository.RemoveRefreshToken(tokenId);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Token Id does not exist");

        }

        //[Authorize(Users = "Admin")]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_authRepository.GetAllRefreshTokens());
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
