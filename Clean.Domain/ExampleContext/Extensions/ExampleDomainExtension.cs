using Clean.Domain.ExampleContext.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Clean.Domain.ExampleContext.Extensions
{
    public static class ExampleDomainExtension
    {
        public static void ExampleDomain(this IServiceCollection services)
        {
            services.AddScoped<IExampleService, ExampleService>();
            services.AddScoped<ISampleService, SampleService>();
        }
    }
}