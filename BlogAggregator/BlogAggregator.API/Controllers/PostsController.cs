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

namespace BlogAggregator.API.Controllers
{
    public class PostsController : ApiController
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PostsController(IBlogRepository blogRepository, IPostRepository postRepository, IUnitOfWork unitOfWork)
        {
            _blogRepository = blogRepository;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
        }        

        // GET: api/Posts
        public IQueryable<PostModel> GetPosts()
        {
            return _postRepository.GetAll().ProjectTo<PostModel>();
        }

        // GET: api/Posts/5
        [ResponseType(typeof(PostModel))]
        public IHttpActionResult GetPost(int id)
        {
            Post dbPost = _postRepository.GetByID(id);
            if (dbPost == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<PostModel>(dbPost));
        }

        // PUT: api/Posts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPost(int id, PostModel post)
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != post.PostID)
            {
                return BadRequest();
            }

            // Get the DB post, update it according to the input PostModel object,           
            //   and then update the DB post in the database
            var dbPost = _postRepository.GetByID(id);
            dbPost.Update(post);
            _postRepository.Update(dbPost);

            // Save database changes
            try
            {
                _unitOfWork.Commit();
            }
            catch (DBConcurrencyException e)            
            { 
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception("Unable to update the post in the database", e);
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Posts
        [ResponseType(typeof(PostModel))]
        public IHttpActionResult PostPost(PostModel post)
        {
            // Validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check that the corresponding blog exists
            if (!_blogRepository.Any(b => b.BlogID == post.BlogID))
            {
                throw new Exception("Unable to add the post to the database, as it does not correspond to a blog");
            }

            //Set up new Post object, populated from input post
            Post dbPost = new Post();
            dbPost.Update(post);

            // Add the new Post object to the DB
            _postRepository.Add(dbPost);

            // Save the changes in the database
            try
            {
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                throw new Exception("Unable to add the post to the database", e);
            }

            // Set post ID in PostModel object with the ID 
            //  that was set in the DB post after db.SaveChanges
            post.PostID = dbPost.PostID;
            return CreatedAtRoute("DefaultApi", new { id = post.PostID }, post);
        }

        // DELETE: api/Posts/5
        [ResponseType(typeof(PostModel))]
        public IHttpActionResult DeletePost(int id)
        {
            // Get the DB post corresponding to the post ID
            Post dbPost = _postRepository.GetByID(id);
            if (dbPost == null)
            {
                return NotFound();
            }

            // Remove the post
            _postRepository.Delete(dbPost);

            try
            {
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {

                throw new Exception("Unable to delete the post from the database", e);
            }

            return Ok(Mapper.Map<PostModel>(dbPost));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool PostExists(int id)
        {
            return _postRepository.Count(e => e.PostID == id) > 0;
        }
    }
}