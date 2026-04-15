using OldandNewClone.Application.Interfaces;
using OldandNewClone.Infrastructure.Configuration;
using OldandNewClone.Infrastructure.Persistence;
using OldandNewClone.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace OldandNewClone.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB Configuration
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
        services.AddSingleton<MongoContext>();

        // Repositories
        services.AddScoped<ISongRepository, SongRepository>();
        services.AddScoped<IUserDataRepository, UserDataRepository>();

        return services;
    }
}
