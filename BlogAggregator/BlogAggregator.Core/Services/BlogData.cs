using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Infrastructure;
using BlogAggregator.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogAggregator.Core.Services
{
    public class BlogData : IDisposable
    {
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BlogData(IPostRepository postRepository, IUnitOfWork unitOfWork)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
        }

        // Write the posts in the input posts to the database
        // Return true if successful, otherwise return false
        public bool SavePosts(List<Post> posts, int blogID)
        {
            bool result = true;

            foreach (var post in posts)
            {
                _postRepository.Add(new Post
                {
                    BlogID = blogID,
                    Content = post.Content,
                    Description = post.Description,
                    Link = post.Link,
                    PublicationDate = post.PublicationDate,
                    Title = post.Title
                });
            }

            try
            {
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {

                result = false;
            }

            return result;
        }

        public void Dispose()
        {
           //base.Dispose();
        }
    }
}
