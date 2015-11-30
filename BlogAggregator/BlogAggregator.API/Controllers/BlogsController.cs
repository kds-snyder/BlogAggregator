using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
using AutoMapper;
using BlogAggregator.Core.Models;
using System.Web.Http.Results;
using BlogAggregator.Core.Services;

namespace BlogAggregator.API.Controllers
{
    public class BlogsController : ApiController
    {
        private readonly IBlogAggregatorDbContext db;

        public BlogsController()
        {
            db = new BlogAggregatorDbContext();
        }

        public BlogsController(IBlogAggregatorDbContext context)
        {
            db = context;
        }

        // GET: api/Blogs
        public IEnumerable<BlogModel> GetBlogs()
        {
            return Mapper.Map<IEnumerable<BlogModel>>(db.Blogs);
        }

        // GET: api/Blogs/5
        [ResponseType(typeof(BlogModel))]
        public IHttpActionResult GetBlog(int id)
        {
            Blog dbBlog = db.Blogs.Find(id);
            if (dbBlog == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<BlogModel>(dbBlog));
        }

        // GET: api/blogs/5/posts
        // Get posts belonging to blog corresponding to blog ID
        [Route("api/blogs/{blogID}/posts")]
        public IHttpActionResult GetPostsForBlog(int blogID)
        {
            // Validate request
            if (!BlogExists(blogID))
            {
                return BadRequest();
            }

            // Get list of posts where the blog ID
            //  matches the blog IDs belonging to the blog            
            var dbPosts = db.Posts.Where(p => p.BlogID == blogID);
 
            if (dbPosts.Count() == 0)
            {
                return NotFound();
            }

            // Return the list of PostModel objects            
            return Ok(Mapper.Map<IEnumerable<PostModel>>(dbPosts));
        }

        // PUT: api/Blogs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBlog(int id, BlogModel blog)
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != blog.BlogID)
            {
                return BadRequest();
            }

            if (!BlogExists(id))
            {
                return BadRequest();
            }

            // Get the DB blog and save the approved indicator
            var dbBlog = db.Blogs.Find(id);
            bool approvedBeforeUpdate = dbBlog.Approved;

            // Update the DB blog according to the input BlogModel object,
            //   and then set indicator that DB blog has been modified
            dbBlog.Update(blog);
            db.Entry(dbBlog).State = EntityState.Modified;

            // If approved indicator was changed from true to false, 
            //   remove any posts corresponding to blog
            if (approvedBeforeUpdate && !blog.Approved)
            {
                var dbPosts = db.Posts.Where(p => p.BlogID == id);

                if (dbPosts.Count() > 0)
                {
                    foreach (var dbPost in dbPosts)
                    {
                        db.Posts.Remove(dbPost);
                    }
                }
            }

            // Save database changes
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!BlogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception("Unable to update the blog in the database", e);
                }
            }

            // If approved indicator was changed from false to true, 
            //  parse the blog posts and store them in DB
            if (!approvedBeforeUpdate && blog.Approved)
            {
                    BlogWebDataWP.GetBlogPosts(blog);               
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Blogs
        [ResponseType(typeof(BlogModel))]
        public IHttpActionResult PostBlog(BlogModel blog)
        {           

            // Validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the blog information from the blog website 
            // If unable to get the information, do not create the blog record
 
                bool result = BlogWebDataWP.GetBlogInformation(blog);
                if (!result)
                {
                    return NotFound();
                }                        
                          
            //Set up new Blog object, populated from input blog
            Blog dbBlog = new Blog();
            dbBlog.Update(blog);            

            // Add the new Blog object to the DB
            db.Blogs.Add(dbBlog);

            // Save the changes in the database
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {

                throw new Exception("Unable to add the blog to the database", e);
            }

            // Set blog ID in BlogModel object with the ID 
            //  that was set in the DB blog after db.SaveChanges
            blog.BlogID = dbBlog.BlogID;

            // If approved, parse the blog posts and store them in the DB

            if (blog.Approved)
            {
                    BlogWebDataWP.GetBlogPosts(blog);                
            }            

            // Return the created blog record
            return CreatedAtRoute("DefaultApi", new { id = blog.BlogID }, blog);
        }

        // DELETE: api/Blogs/5
        [ResponseType(typeof(BlogModel))]
        public IHttpActionResult DeleteBlog(int id)
        {
            // Get the DB blog corresponding to the blog ID
            Blog dbBlog = db.Blogs.Find(id);
            if (dbBlog == null)
            {
                return NotFound();
            }

            try
            {
                // Remove any posts corresponding to the blog               
                var dbPosts = db.Posts.Where(p => p.BlogID == id);
 
                if (dbPosts.Count() > 0)
                {
                    foreach (var dbPost in dbPosts)
                    {
                        db.Posts.Remove(dbPost);
                    }
                }

                // Remove the blog
                 db.Blogs.Remove(dbBlog);

                db.SaveChanges();
            }
            catch (Exception e)
            {

                throw new Exception("Unable to delete the blog from the database", e);
            }

            return Ok(Mapper.Map<BlogModel>(dbBlog));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BlogExists(int id)
        {
            return db.Blogs.Count(e => e.BlogID == id) > 0;
        }
    }
}