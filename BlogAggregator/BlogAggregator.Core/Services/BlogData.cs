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

        

        public void Dispose()
        {
           //base.Dispose();
        }
    }
}
