//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//
//Last Generated: 12/29/2022 03:37//

using Clean.Domain.Common.Interfaces;
using Clean.Domain.Common.Model;
using Clean.Domain.ExampleContext.Entities;
using Clean.Domain.ExampleContext.Models.LookupRequests;

namespace Clean.Domain.ExampleContext.Services
{
    public interface IExamplesService
    {
        ResultResponse Create(Examples entity);
        IEnumerable<Examples> Get(ExamplesLookupRequest lookupRequest);
        ResultResponse Update(Examples entity);
        ResultResponse Delete(Examples entity);
    }

    public class ExamplesService : IExamplesService
    {
        private readonly IRepository<Examples> _ExamplesRepository;

        public ExamplesService(IRepository<Examples> ExamplesRepository)
        {
            _ExamplesRepository = ExamplesRepository;
        }

        public ResultResponse Create(Examples entity)
        {
            ResultResponse result = entity.Validate();

            if (result.Successful)
                result = _ExamplesRepository.Create(entity);

            return result;
        }

        public IEnumerable<Examples> Get(ExamplesLookupRequest lookupRequest)
        {
            var predicate = lookupRequest.BuildPreciate<Examples>();
            return _ExamplesRepository.Where(predicate);
        }

        public ResultResponse Update(Examples entity)
        {
            ResultResponse result = entity.Validate();

            if (result.Successful)
                _ExamplesRepository.Update(entity);

            return result;
        }

        public ResultResponse Delete(Examples entity)
        {
            return _ExamplesRepository.Delete(entity);
        }
    }
}