using Clean.Domain.Common.Interfaces;
using Clean.Domain.Common.Model;
using Clean.Domain.ExampleContext.Entities;
using Clean.Domain.ExampleContext.Models.LookupRequests;

namespace Clean.Domain.ExampleContext.Services
{
    public interface IExampleService
    {
        ResultResponse Create(Example entity);
        IEnumerable<Example> Get(ExampleLookupRequest lookupRequest);
        ResultResponse Update(Example entity);
        ResultResponse Delete(Example entity);
    }

    public class ExampleService : IExampleService
    {
        private readonly IRepository<Example> _ExampleRepository;

        public ExampleService(IRepository<Example> exampleRepository)
        {
            _ExampleRepository = exampleRepository;
        }

        public ResultResponse Create(Example entity)
        {
            ResultResponse result = entity.Validate();

            if (result.Successful)
                result = _ExampleRepository.Create(entity);

            return result;
        }

        public IEnumerable<Example> Get(ExampleLookupRequest lookupRequest)
        {
            var predicate = lookupRequest.BuildPreciate<Example>();
            return _ExampleRepository.Where(predicate);
        }

        public ResultResponse Update(Example entity)
        {
            ResultResponse result = entity.Validate();

            if (result.Successful)
                _ExampleRepository.Update(entity);

            return result;
        }

        public ResultResponse Delete(Example entity)
        {
            return _ExampleRepository.Delete(entity);
        }
    }
}