using OldandNewClone.Application.Interfaces;
using OldandNewClone.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace OldandNewClone.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Application Services
        services.AddScoped<ISongService, SongService>();
        services.AddScoped<IUserDataService, UserDataService>();
        services.AddScoped<ITransposeService, TransposeService>();

        return services;
    }
}
