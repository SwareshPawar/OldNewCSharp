using OldandNewClone.Application.Interfaces;
using OldandNewClone.Application.Configuration;
using OldandNewClone.Infrastructure.Configuration;
using OldandNewClone.Infrastructure.Persistence;
using OldandNewClone.Infrastructure.Repositories;
using OldandNewClone.Infrastructure.Services;
using OldandNewClone.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using MongoDB.Bson;
using MongoDB.Driver;

namespace OldandNewClone.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB Configuration
        var mongoDbSettings = configuration.GetSection("MongoDbSettings").Get<Configuration.MongoDbSettings>();
        services.Configure<Configuration.MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
        services.AddSingleton<MongoContext>();

        // JWT Configuration
        services.Configure<OldandNewClone.Application.Configuration.JwtSettings>(configuration.GetSection("JwtSettings"));

        // MongoDB Identity Configuration
        var mongoDbIdentityConfiguration = new MongoDbIdentityConfiguration
        {
            MongoDbSettings = new AspNetCore.Identity.MongoDbCore.Infrastructure.MongoDbSettings
            {
                ConnectionString = mongoDbSettings!.ConnectionString,
                DatabaseName = mongoDbSettings.DatabaseName
            },
            IdentityOptionsAction = options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            }
        };

        services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, string>(mongoDbIdentityConfiguration);

        // Replace default password hasher with BCrypt
        services.AddScoped<IPasswordHasher<ApplicationUser>, BCryptPasswordHasher<ApplicationUser>>();

        // Explicitly add SignInManager (required by AuthService)
        services.AddScoped<SignInManager<ApplicationUser>>();

        // Services
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Provide IMongoDatabase for services that need direct MongoDB access
        services.AddScoped(sp =>
        {
            var mongoContext = sp.GetRequiredService<MongoContext>();
            return mongoContext.Database;
        });

        // Repositories
        services.AddScoped<ISongRepository, SongRepository>();
        services.AddScoped<IUserDataRepository, UserDataRepository>();
        services.AddScoped<ISetlistRepository, SetlistRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
