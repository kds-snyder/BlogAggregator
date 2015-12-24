using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Models;
using BlogAggregator.Core.Repository;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Data;
using System.Web.Http.OData;
using System.Web.Http.Description;

namespace BlogAggregator.API.Controllers.OAuth
{   
    public class ExternalLoginsController : ApiController
    {
        private readonly IExternalLoginRepository _externalLoginRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExternalLoginsController(IExternalLoginRepository externalLoginRepository,
                                            IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _externalLoginRepository = externalLoginRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        // GET: api/ExternalLogins  
        [EnableQuery]
        public IQueryable<ExternalLoginModel> GetExternalLogins()
        {
            return _externalLoginRepository.GetAll().ProjectTo<ExternalLoginModel>();
        }

        // GET: api/ExternalLogins/5
        [ResponseType(typeof(ExternalLoginModel))]
        public IHttpActionResult GetExternalLogin(int id)
        {
            ExternalLogin dbExternalLogin = _externalLoginRepository.GetByID(id);
            if (dbExternalLogin == null)
            {
                return NotFound();
            }
            return Ok(Mapper.Map<ExternalLoginModel>(dbExternalLogin));
        }

        // PUT: api/ExternalLogins/5
        [Authorize]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutExternalLogin(int id, ExternalLoginModel externalLogin)
        {
            // Allow only for authorized user
            var userToCheck = _userRepository.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (!userToCheck.Authorized)
            {
                return Unauthorized();
            }

            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != externalLogin.ExternalLoginID)
            {
                return BadRequest();
            }

            // Get the DB externalLogin, update it according to the input ExternalLoginModel object,           
            //   and then update the DB externalLogin in the database
            var dbExternalLogin = _externalLoginRepository.GetByID(id);
            if (dbExternalLogin == null)
            {
                return NotFound();
            }
            dbExternalLogin.Update(externalLogin);
            _externalLoginRepository.Update(dbExternalLogin);

            // Save database changes
            try
            {
                _unitOfWork.Commit();
            }
            catch (DBConcurrencyException e)
            {
                if (!ExternalLoginExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception("Unable to update the externalLogin in the database", e);
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ExternalLogins
        [Authorize]
        [ResponseType(typeof(ExternalLoginModel))]
        public IHttpActionResult PostExternalLogin(ExternalLoginModel externalLogin)
        {
            // Validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check that the corresponding user exists
            if (!_userRepository.Any(u => u.Id == externalLogin.UserID))
            {
                throw new Exception("Unable to add the external login to the database, as it does not correspond to a user");
            }

            //Set up new ExternalLogin object, populated from input externalLogin
            ExternalLogin dbExternalLogin = new ExternalLogin();
            dbExternalLogin.Update(externalLogin);

            // Add the new ExternalLogin object to the DB
            _externalLoginRepository.Add(dbExternalLogin);

            // Save the changes in the database
            try
            {
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                throw new Exception("Unable to add the externalLogin to the database", e);
            }

            // Set externalLogin ID in ExternalLoginModel object with the ID 
            //  that was set in the DB externalLogin after db.SaveChanges
            externalLogin.ExternalLoginID = dbExternalLogin.ExternalLoginID;
            return CreatedAtRoute("DefaultApi", new { id = externalLogin.ExternalLoginID }, externalLogin);
        }

        // DELETE: api/ExternalLogins/5
        [Authorize]
        [ResponseType(typeof(ExternalLoginModel))]
        public IHttpActionResult DeleteExternalLogin(int id)
        {
            // Allow only for authorized user
            var userToCheck = _userRepository.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (!userToCheck.Authorized)
            {
                return Unauthorized();
            }

            // Get the DB externalLogin corresponding to the externalLogin ID
            ExternalLogin dbExternalLogin = _externalLoginRepository.GetByID(id);
            if (dbExternalLogin == null)
            {
                return NotFound();
            }

            // Remove the externalLogin
            _externalLoginRepository.Delete(dbExternalLogin);

            try
            {
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {

                throw new Exception("Unable to delete the externalLogin from the database", e);
            }

            return Ok(Mapper.Map<ExternalLoginModel>(dbExternalLogin));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool ExternalLoginExists(int id)
        {
            return _externalLoginRepository.Count(u => u.ExternalLoginID == id) > 0;
        }


    }
}
