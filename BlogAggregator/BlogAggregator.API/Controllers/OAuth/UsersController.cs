using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Models;
using BlogAggregator.Core.Repository;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Data;
using System.Web.Http.OData;

namespace BlogAggregator.API.Controllers.OAuth
{
    public class UsersController : ApiController
    {       
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {            
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        // GET: api/Users  
        [EnableQuery]
        public IQueryable<UserModel> GetUsers()
        {
            return _userRepository.GetAll().ProjectTo<UserModel>();
        }

        // GET: api/Users/5
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult GetUser(int id)
        {
            User dbUser = _userRepository.GetByID(id);
            if (dbUser == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<UserModel>(dbUser));
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(string id, UserModel user)
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            // Get the DB user, update it according to the input UserModel object,           
            //   and then update the DB user in the database
            var dbUser = _userRepository.GetByID(id);
            if (dbUser == null)
            {
                return NotFound();
            }
            dbUser.Update(user);
            _userRepository.Update(dbUser);

            // Save database changes
            try
            {
                _unitOfWork.Commit();
            }
            catch (DBConcurrencyException e)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception("Unable to update the user in the database", e);
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult PostUser(UserModel user)
        {
            // Validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }           

            //Set up new User object, populated from input user
            User dbUser = new User();
            dbUser.Update(user);

            // Add the new User object to the DB
            _userRepository.Add(dbUser);

            // Save the changes in the database
            try
            {
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                throw new Exception("Unable to add the user to the database", e);
            }

            // Set user ID in UserModel object with the ID 
            //  that was set in the DB user after db.SaveChanges
            user.Id = dbUser.Id;
            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult DeleteUser(int id)
        {
            // Get the DB user corresponding to the user ID
            User dbUser = _userRepository.GetByID(id);
            if (dbUser == null)
            {
                return NotFound();
            }

            // Remove the user
            _userRepository.Delete(dbUser);

            try
            {
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {

                throw new Exception("Unable to delete the user from the database", e);
            }

            return Ok(Mapper.Map<UserModel>(dbUser));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool UserExists(string id)
        {
            return _userRepository.Count(u => u.Id == id) > 0;
        }
    }
}