using SWAPI_POC_Core.Configuration;

namespace SWAPI_POC.Configuration
{
    public static class OptionsConfig
    {
        public static void SetupOptions(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<SWAPISettings>(config.GetSection(SWAPISettings.Section));
        }
    }
}
