using Microsoft.Azure.WebJobs.Host;
using SimpleInjector;

namespace BlogAggregator.WebJob
{
    class BlogAggregatorJobActivator : IJobActivator
    {
        private Container container;

        public BlogAggregatorJobActivator(Container container)
        {
            this.container = container;
        }

        public T CreateInstance<T>()
        {
            return (T)container.GetInstance(typeof(T));
        }
    }
}
