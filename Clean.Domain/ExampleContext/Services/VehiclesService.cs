//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.Common.Interfaces;
using Clean.Domain.Common.Model;
using Clean.Domain.ExampleContext.Entities;
using Clean.Domain.ExampleContext.Models.LookupRequests;

namespace Clean.Domain.ExampleContext.Services
{
    public interface IVehiclesService
    {
        ResultResponse Create(Vehicles entity);
        IEnumerable<Vehicles> GetAll();
        IEnumerable<Vehicles> Get(VehiclesLookupRequest lookupRequest);
        ResultResponse Update(Vehicles entity);
        ResultResponse Delete(Vehicles entity);
    }

    public class VehiclesService : IVehiclesService
    {
        private readonly IRepository<Vehicles> _VehiclesRepository;

        public VehiclesService(IRepository<Vehicles> VehiclesRepository)
        {
            _VehiclesRepository = VehiclesRepository;
        }

        public ResultResponse Create(Vehicles entity)
        {
            ResultResponse result = entity.Validate();

            if (result.Successful)
                result = _VehiclesRepository.Create(entity);

            return result;
        }

        public IEnumerable<Vehicles> GetAll()
        {
            return _VehiclesRepository.GetAll();
        }

        public IEnumerable<Vehicles> Get(VehiclesLookupRequest lookupRequest)
        {
            var predicate = lookupRequest.BuildPreciate<Vehicles>();
            return _VehiclesRepository.Where(predicate);
        }

        public ResultResponse Update(Vehicles entity)
        {
            ResultResponse result = entity.Validate();

            if (result.Successful)
                _VehiclesRepository.Update(entity);

            return result;
        }

        public ResultResponse Delete(Vehicles entity)
        {
            return _VehiclesRepository.Delete(entity);
        }
    }
}