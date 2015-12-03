using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Models;
using AutoMapper;
using BlogAggregator.Core.Repository;

namespace BlogAggregator.API.Controllers
{
    public class PostsController : ApiController
    {        
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PostsController(IPostRepository postRepository, IUnitOfWork unitOfWork)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
        }        

        // GET: api/Posts
        public IEnumerable<PostModel> GetPosts()
        {
            return Mapper.Map<IEnumerable<PostModel>>(db.Posts);
        }

        // GET: api/Posts/5
        [ResponseType(typeof(PostModel))]
        public IHttpActionResult GetPost(int id)
        {
            Post dbPost = db.Posts.Find(id);
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

            if (!PostExists(id))
            {
                return BadRequest();
            }

            // Get the DB post, update it according to the input PostModel object,           
            //   and then set indicator that DB post has been modified
            var dbPost = db.Posts.Find(id);
            dbPost.Update(post);
            db.Entry(dbPost).State = EntityState.Modified;

            // Save database changes
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
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

            //Set up new Post object, populated from input post
            Post dbPost = new Post();
            dbPost.Update(post);

            // Add the new Post object to the DB
            db.Posts.Add(dbPost);

            // Save the changes in the database
            try
            {
                db.SaveChanges();
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
            Post dbPost = db.Posts.Find(id);
            if (dbPost == null)
            {
                return NotFound();
            }

            // Remove the post
            db.Posts.Remove(dbPost);

            try
            {                
                db.SaveChanges();
            }
            catch (Exception e)
            {

                throw new Exception("Unable to delete the post from the database", e);
            }

            return Ok(Mapper.Map<PostModel>(dbPost));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PostExists(int id)
        {
            return db.Posts.Count(e => e.PostID == id) > 0;
        }
    }
}