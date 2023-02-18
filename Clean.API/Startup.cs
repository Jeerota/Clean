//using Clean.Domain.ExampleContext.Extensions;
//using Clean.Infrastructure.ExampleContext;
using Clean.Infrastructure.SQL;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(MyNamespace.Startup))]

namespace MyNamespace
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.SQLInfrastructure();
            //builder.Services.ExampleInfrastructure();
            //builder.Services.ExampleDomain();
        }
    }
}