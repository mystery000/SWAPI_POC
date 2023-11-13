using SWAPI_POC_Core.Infrastructure.ApiClients.SWAPI;
using SWAPI_POC_Core.Interfaces;
using SWAPI_POC_Core.Repository;

namespace SWAPI_POC.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void SetupDependencyInjection(this IServiceCollection services)
        {
            services.AddTransient<ISwapiRepository, SwapiRepository>();
            services.AddTransient<ISwapiService, SWAPIService>();
        }
    }
}
