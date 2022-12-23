using Clean.Application.Common;
using Clean.Application.Common.Helpers;
using Clean.Application.Models.LookupRequests;
using Clean.Domain.ExampleContext.Entities;
using System.Linq.Expressions;
using System.Reflection;

namespace Clean.Domain.ExampleContext.Services
{
    public interface IExampleService
    {
        void Create(Example entity);
        IEnumerable<Example> Get(ExampleLookupRequest lookupRequest);
        void Update(Example entity);
        void Delete(Example entity);
        void Delete(int id);
    }

    public class ExampleService : IExampleService
    {
        private readonly IRepository<ExampleDbContext, Example> _ExampleRepository;

        public ExampleService(IRepository<ExampleDbContext, Example> exampleRepository) 
        {
            _ExampleRepository = exampleRepository;
        }

        public void Create(Example entity)
        {
            _ExampleRepository.Create(entity);
        }

        public IEnumerable<Example> Get(ExampleLookupRequest lookupRequest)
        {
            var predicate = FilterExamplesByLookupRequest(lookupRequest);
            return _ExampleRepository.Where(predicate);
        }

        public void Update(Example entity)
        {
            _ExampleRepository.Update(entity);
        }

        public void Delete(Example entity)
        {
            _ExampleRepository.Delete(entity);
        }

        public void Delete(int id)
        {
            var entity = _ExampleRepository.FirstOrDefault(entity => entity.Id == id);

            if(entity != null)
                _ExampleRepository.Delete(entity);
        }

        private Expression<Func<Example, bool>> FilterExamplesByLookupRequest(ExampleLookupRequest lookupRequest)
        {
            Expression<Func<Example, bool>> predicate = PredicateBuilder.True<Example>();
            var properties = lookupRequest.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                var lookupPropertyValue = property.GetValue(lookupRequest, null);

                if(lookupPropertyValue != null)
                    predicate = predicate.And(entity => property.GetValue(entity, null) == lookupPropertyValue);
            }

            return predicate;
        }
    }
}
