using Clean.Domain.Common.Interfaces;
using Clean.Domain.ExampleContext.Entities;
using Clean.Domain.ExampleContext.Models.LookupRequests;

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
        private readonly IRepository<Sample> _SampleRepository;

        public SampleService(IRepository<Sample> SampleRepository)
        {
            _SampleRepository = SampleRepository;
        }

        public void Create(Sample entity)
        {
            _SampleRepository.Create(entity);
        }

        public IEnumerable<Sample> Get(SampleLookupRequest lookupRequest)
        {
            var predicate = lookupRequest.BuildPreciate<Sample>();
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

            if (entity != null)
                _SampleRepository.Delete(entity);
        }
    }
}
