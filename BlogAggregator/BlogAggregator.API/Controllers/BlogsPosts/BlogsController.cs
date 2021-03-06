﻿using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using BlogAggregator.Core.Domain;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BlogAggregator.Core.Models;
using BlogAggregator.Core.Services;
using BlogAggregator.Core.Repository;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.BlogReader.WordPress;
using System.Web.Http.OData;
using BlogAggregator.Core.BlogReader;
using System.Collections.Generic;

namespace BlogAggregator.API.Controllers
{
    public class BlogsController : ApiController
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlogService _blogService;
        private readonly IWordPressBlogReader _wordPressBlogReader;

        public BlogsController(IBlogRepository blogRepository, IPostRepository postRepository,
                                    IUserRepository userRepository,
                                    IUnitOfWork unitOfWork, IBlogService blogService, 
                                    IWordPressBlogReader wordPressBlogReader)
        {
            _blogRepository = blogRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _blogService = blogService;
            _wordPressBlogReader = wordPressBlogReader;
        }

        // GET: api/Blogs
        [EnableQuery]
        public IQueryable<BlogModel> GetBlogs()      
        {
            return _blogRepository.GetAll().ProjectTo<BlogModel>();
        }

        // GET: api/Blogs/5
        [ResponseType(typeof(BlogModel))]
        public IHttpActionResult GetBlog(int id)
        {
            Blog dbBlog = _blogRepository.GetByID(id);
            if (dbBlog == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<BlogModel>(dbBlog));
        }

        // PUT: api/Blogs/5
        [Authorize]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBlog(int id, BlogModel blog)
        {
            //Allow only for authorized user
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

            if (id != blog.BlogID)
            {
                return BadRequest();
            }

            // Get the DB blog and save the approved indicator
            var dbBlog = _blogRepository.GetByID(id);
            if (dbBlog == null)
            {
                return NotFound();
            }
            bool approvedBeforeUpdate = dbBlog.Approved;

            // Update the DB blog according to the input BlogModel object,
            //   and then update the DB blog in the database
            dbBlog.Update(blog);
            _blogRepository.Update(dbBlog);

            // If approved indicator was changed from true to false, 
            //   remove any posts corresponding to blog
            if (approvedBeforeUpdate && !dbBlog.Approved)
            {
                deleteBlogPosts(id);
            }

            // Save database changes
            try
            {
                _unitOfWork.Commit();
            }

            catch (DBConcurrencyException ex)
            {
                if (!BlogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw new Exception("Unable to update the blog in the database", ex);
                }
            }

            // If approved indicator was changed from false to true, 
            //  parse the blog posts and store them in DB
            if (!approvedBeforeUpdate && dbBlog.Approved)
            {
                _blogService.ExtractAndSaveBlogPosts(dbBlog);
 
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
            BlogInfo blogInfo = _wordPressBlogReader.VerifyBlog(blog.Link);
            if (blogInfo == null)
            {
                return NotFound();
            }

            //Set the blog description and title that was obtained from the blog website
            blog.Description = blogInfo.Description;
            blog.Title = blogInfo.Title;

            // Set blog as not approved
            blog.Approved = false;

            //Set up new Blog object, populated from input blog
            Blog dbBlog = new Blog();
            dbBlog.Update(blog);

            // Add the new Blog object to the DB
            _blogRepository.Add(dbBlog);

            // Save the changes in the database
            try
            {
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                throw new Exception("Unable to add the blog to the database", e);
            }

            // Set blog ID in BlogModel object with the ID 
            //  that was placed in the DB blog after it was added to DB
            blog.BlogID = dbBlog.BlogID;           

            // Return the created blog record
            return CreatedAtRoute("DefaultApi", new { id = blog.BlogID }, blog);
        }

        // DELETE: api/Blogs/5
        [Authorize]
        [ResponseType(typeof(BlogModel))]
        public IHttpActionResult DeleteBlog(int id)
        {
            //Allow only for authorized user
            var userToCheck = _userRepository.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (!userToCheck.Authorized)
            {
                return Unauthorized();
            }

            // Get the DB blog corresponding to the blog ID
            Blog dbBlog = _blogRepository.GetByID(id);
            if (dbBlog == null)
            {
                return NotFound();
            }

            try
            {
                deleteBlogPosts(id);

                // Remove the blog
                _blogRepository.Delete(dbBlog);

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {

                throw new Exception("Unable to delete the blog from the database", e);
            }

            return Ok(Mapper.Map<BlogModel>(dbBlog));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private bool BlogExists(int id)
        {
            return _blogRepository.Count(b => b.BlogID == id) > 0;
        }

        // Remove posts corresponding to blog
        private void deleteBlogPosts(int blogID)
        {
            var dbPosts = _postRepository.Where(p => p.BlogID == blogID);

            if (dbPosts.Count() > 0)
            {
                foreach (var dbPost in dbPosts)
                {
                    _postRepository.Delete(dbPost);
                }
            }
        }
    }
}