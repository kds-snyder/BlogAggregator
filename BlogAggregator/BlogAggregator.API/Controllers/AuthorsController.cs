using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Models;
using AutoMapper;

namespace BlogAggregator.API.Controllers
{
    public class AuthorsController : ApiController
    {        
        private readonly IBlogAggregatorDbContext db;

        public AuthorsController()
        {
            db = new BlogAggregatorDbContext();
        }

        public AuthorsController(IBlogAggregatorDbContext context)
        {
            db = context;
        }               

        // GET: api/Authors
        public IEnumerable<AuthorModel> GetAuthors()
        {
            return Mapper.Map<IEnumerable<AuthorModel>>(db.Authors);
        }

        // GET: api/Authors/5
        [ResponseType(typeof(AuthorModel))]
        public IHttpActionResult GetAuthor(int id)
        {
            Author dbAuthor = db.Authors.Find(id);
            if (dbAuthor == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<AuthorModel>(dbAuthor));
        }

        // GET: api/authors/5/blogs
        // Get blogs belonging to author corresponding to author ID
        [Route("api/authors/{authorID}/blogs")]
        public IHttpActionResult GetBlogsForAuthor(int authorID)
        {
            // Validate request
            if (!AuthorExists(authorID))
            {
                return BadRequest();
            }

            // Get list of blogs where the author ID
            //  matches the input author ID
            var dbBlogs = db.Blogs.Where(b => b.AuthorID == authorID);

            if (dbBlogs.Count() == 0)
            {
                return NotFound();
            }

            // Return the list of BlogModel objects            
            return Ok(Mapper.Map<IEnumerable<BlogModel>>(dbBlogs));
        }

        // GET: api/authors/5/posts
        // Get posts belonging to author corresponding to author ID
        [Route("api/authors/{authorID}/posts")]
        public IHttpActionResult GetPostsForAuthor(int authorID)
        {
            // Validate request
            if (!AuthorExists(authorID))
            {
                return BadRequest();
            }

            // Get list of posts where the blog ID
            //  matches the blog IDs belonging to the author            
            var dbPosts = db.Posts.Where(p =>
                          db.Blogs.Any(b => b.AuthorID == authorID &&
                                        b.BlogID == p.BlogID));

            if (dbPosts.Count() == 0)
            {
                return NotFound();
            }

            // Return the list of PostModel objects            
            return Ok(Mapper.Map<IEnumerable<PostModel>>(dbPosts));
        }

        // PUT: api/Authors/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAuthor(int id, AuthorModel author)
        {
            // Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != author.AuthorID)
            {
                return BadRequest();
            }

            if (!AuthorExists(id))
            {
                return BadRequest();
            }

            // Get the DB author, update it according to the input AuthorModel object,           
            //   and then set indicator that DB author has been modified
            var dbAuthor = db.Authors.Find(id);
            dbAuthor.Update(author);
            db.Entry(dbAuthor).State = EntityState.Modified;


            // Save database changes
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!AuthorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception("Unable to update the author in the database", e);
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Authors
        [ResponseType(typeof(AuthorModel))]
        public IHttpActionResult PostAuthor(AuthorModel author)
        {
            // Validate request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Set up new Author object, populated from input author
            Author dbAuthor = new Author();
            dbAuthor.Update(author);

            // Add the new Author object to the DB
            db.Authors.Add(dbAuthor);

            // Save the changes in the database
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {

                throw new Exception("Unable to add the author to the database", e);
            }
         
            // Set author ID in AuthorModel object with the ID 
            //  that was set in the DB author after db.SaveChanges
            author.AuthorID = dbAuthor.AuthorID;
            return CreatedAtRoute("DefaultApi", new { id = author.AuthorID }, author);
        }

        // DELETE: api/Authors/5
        [ResponseType(typeof(AuthorModel))]
        public IHttpActionResult DeleteAuthor(int id)
        {
            // Get the DB author corresponding to the author ID
            Author dbAuthor = db.Authors.Find(id);
            if (dbAuthor == null)
            {
                return NotFound();
            }

            try
            {
                // Remove any posts corresponding to the author
                var dbPosts = db.Posts.Where(p =>
                              db.Blogs.Any(b => b.AuthorID == id &&
                                        b.BlogID == p.BlogID));

                if (dbPosts.Count() > 0)
                {
                    foreach (var dbPost in dbPosts)
                    {
                        db.Posts.Remove(dbPost);
                    }
                }

                // Remove any blogs corresponding to the author
                var dbBlogs = db.Blogs.Where(b => b.AuthorID == id);

                if (dbBlogs.Count() > 0)
                {
                    foreach (var dbBlog in dbBlogs)
                    {
                        db.Blogs.Remove(dbBlog);
                    }
                }

                // Remove the author
                db.Authors.Remove(dbAuthor);
                db.SaveChanges();
            }
            catch (Exception e)
            {

                throw new Exception("Unable to delete the author from the database", e);
            }
           
            return Ok(Mapper.Map<AuthorModel>(dbAuthor));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AuthorExists(int id)
        {
            return db.Authors.Count(e => e.AuthorID == id) > 0;
        }
    }
}