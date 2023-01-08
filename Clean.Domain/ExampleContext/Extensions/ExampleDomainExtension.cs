//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using Clean.Domain.ExampleContext.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Clean.Domain.ExampleContext.Extensions
{
    public static class ExampleDomainExtension
    {
        public static void ExampleDomain(this IServiceCollection services)
        {
			services.AddScoped<IDriversService, DriversService>();
			services.AddScoped<ILocationsService, LocationsService>();
			services.AddScoped<IVehiclesService, VehiclesService>();

        }
    }
}