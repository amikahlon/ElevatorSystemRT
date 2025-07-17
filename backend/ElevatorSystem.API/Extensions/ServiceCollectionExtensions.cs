using ElevatorSystem.API.Repositories;
using ElevatorSystem.API.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorSystem.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
                    services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
