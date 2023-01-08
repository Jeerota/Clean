//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.Common.Interfaces;
using Clean.Domain.Common.Model;
using Clean.Domain.ExampleContext.Entities;
using Clean.Domain.ExampleContext.Models.LookupRequests;

namespace Clean.Domain.ExampleContext.Services
{
    public interface ISamplesService
    {
        ResultResponse Create(Samples entity);
        IEnumerable<Samples> Get(SamplesLookupRequest lookupRequest);
        ResultResponse Update(Samples entity);
        ResultResponse Delete(Samples entity);
    }

    public class SamplesService : ISamplesService
    {
        private readonly IRepository<Samples> _SamplesRepository;

        public SamplesService(IRepository<Samples> SamplesRepository)
        {
            _SamplesRepository = SamplesRepository;
        }

        public ResultResponse Create(Samples entity)
        {
            ResultResponse result = entity.Validate();

            if (result.Successful)
                result = _SamplesRepository.Create(entity);

            return result;
        }

        public IEnumerable<Samples> Get(SamplesLookupRequest lookupRequest)
        {
            var predicate = lookupRequest.BuildPreciate<Samples>();
            return _SamplesRepository.Where(predicate);
        }

        public ResultResponse Update(Samples entity)
        {
            ResultResponse result = entity.Validate();

            if (result.Successful)
                _SamplesRepository.Update(entity);

            return result;
        }

        public ResultResponse Delete(Samples entity)
        {
            return _SamplesRepository.Delete(entity);
        }
    }
}