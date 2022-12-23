using Clean.Application.Common;
using Clean.Application.Common.Helpers;
using Clean.Application.Models.LookupRequests;
using Clean.Domain.ExampleContext.Entities;
using System.Linq.Expressions;
using System.Reflection;

namespace Clean.Domain.ExampleContext.Services
{
    public interface ISampleService
    {
        void Create(Sample entity);
        IEnumerable<Sample> Get(SampleLookupRequest lookupRequest);
        void Update(Sample entity);
        void Delete(Sample entity);
        void Delete(int id);
    }

    public class SampleService : ISampleService
    {
        private readonly IRepository<ExampleDbContext, Sample> _SampleRepository;

        public SampleService(IRepository<ExampleDbContext, Sample> SampleRepository) 
        { 
            _SampleRepository = SampleRepository;
        }

        public void Create(Sample entity)
        {
            _SampleRepository.Create(entity);
        }

        public IEnumerable<Sample> Get(SampleLookupRequest lookupRequest)
        {
            var predicate = FilterSamplesByLookupRequest(lookupRequest);
            return _SampleRepository.Where(predicate);
        }

        public void Update(Sample entity)
        {
            _SampleRepository.Update(entity);
        }

        public void Delete(Sample entity)
        {
            _SampleRepository.Delete(entity);
        }

        public void Delete(int id)
        {
            var entity = _SampleRepository.FirstOrDefault(entity => entity.Id == id);

            if(entity != null)
                _SampleRepository.Delete(entity);
        }

        private Expression<Func<Sample, bool>> FilterSamplesByLookupRequest(SampleLookupRequest lookupRequest)
        {
            Expression<Func<Sample, bool>> predicate = PredicateBuilder.True<Sample>();
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
