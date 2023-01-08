//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.Common.Interfaces;
using Clean.Domain.Common.Model;
using Clean.Domain.ExampleContext.Entities;
using Clean.Domain.ExampleContext.Models.LookupRequests;

namespace Clean.Domain.ExampleContext.Services
{
    public interface IDriversService
    {
        ResultResponse Create(Drivers entity);
        IEnumerable<Drivers> GetAll();
        IEnumerable<Drivers> Get(DriversLookupRequest lookupRequest);
        ResultResponse Update(Drivers entity);
        ResultResponse Delete(Drivers entity);
    }

    public class DriversService : IDriversService
    {
        private readonly IRepository<Drivers> _DriversRepository;

        public DriversService(IRepository<Drivers> DriversRepository)
        {
            _DriversRepository = DriversRepository;
        }

        public ResultResponse Create(Drivers entity)
        {
            ResultResponse result = entity.Validate();

            if (result.Successful)
                result = _DriversRepository.Create(entity);

            return result;
        }

        public IEnumerable<Drivers> GetAll()
        {
            return _DriversRepository.GetAll();
        }

        public IEnumerable<Drivers> Get(DriversLookupRequest lookupRequest)
        {
            var predicate = lookupRequest.BuildPreciate<Drivers>();
            return _DriversRepository.Where(predicate);
        }

        public ResultResponse Update(Drivers entity)
        {
            ResultResponse result = entity.Validate();

            if (result.Successful)
                _DriversRepository.Update(entity);

            return result;
        }

        public ResultResponse Delete(Drivers entity)
        {
            return _DriversRepository.Delete(entity);
        }
    }
}