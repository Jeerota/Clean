//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.Common.Interfaces;
using Clean.Domain.Common.Model;
using Clean.Domain.ExampleContext.Entities;
using Clean.Domain.ExampleContext.Models.LookupRequests;

namespace Clean.Domain.ExampleContext.Services
{
    public interface ILocationsService
    {
        ResultResponse Create(Locations entity);
        IEnumerable<Locations> GetAll();
        IEnumerable<Locations> Get(LocationsLookupRequest lookupRequest);
        ResultResponse Update(Locations entity);
        ResultResponse Delete(Locations entity);
    }

    public class LocationsService : ILocationsService
    {
        private readonly IRepository<Locations> _LocationsRepository;

        public LocationsService(IRepository<Locations> LocationsRepository)
        {
            _LocationsRepository = LocationsRepository;
        }

        public ResultResponse Create(Locations entity)
        {
            ResultResponse result = entity.Validate();

            if (result.Successful)
                result = _LocationsRepository.Create(entity);

            return result;
        }

        public IEnumerable<Locations> GetAll()
        {
            return _LocationsRepository.GetAll();
        }

        public IEnumerable<Locations> Get(LocationsLookupRequest lookupRequest)
        {
            var predicate = lookupRequest.BuildPreciate<Locations>();
            return _LocationsRepository.Where(predicate);
        }

        public ResultResponse Update(Locations entity)
        {
            ResultResponse result = entity.Validate();

            if (result.Successful)
                _LocationsRepository.Update(entity);

            return result;
        }

        public ResultResponse Delete(Locations entity)
        {
            return _LocationsRepository.Delete(entity);
        }
    }
}