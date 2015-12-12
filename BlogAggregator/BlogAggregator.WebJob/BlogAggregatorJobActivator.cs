using Microsoft.Azure.WebJobs.Host;
using SimpleInjector;

namespace BlogAggregator.WebJob
{
    class BlogAggregatorJobActivator : IJobActivator
    {
        private Container _container;

        public BlogAggregatorJobActivator(Container container)
        {
            _container = container;
        }

        public T CreateInstance<T>()
        {
            return (T)_container.GetInstance(typeof(T));
        }
    }
}
