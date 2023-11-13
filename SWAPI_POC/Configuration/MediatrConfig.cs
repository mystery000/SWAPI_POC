using MediatR;
using SWAPI_POC.Behaviours;
using System.Reflection;

namespace SWAPI_POC.Configuration
{
    public static class MediatrConfig
    {
        public static void SetupMediatr(this IServiceCollection services)
        {

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        }
    }
}
