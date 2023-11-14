namespace SWAPI_POC.Configuration
{
    public static class CacheConfig
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
        }
    }
}