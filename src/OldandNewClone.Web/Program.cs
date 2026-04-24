using OldandNewClone.Web.Components;
using OldandNewClone.Application;
using OldandNewClone.Application.Configuration;
using OldandNewClone.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Components;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.Secret)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Add HttpContextAccessor (required by SignInManager)
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient for Blazor Server components — uses the app's own base URI
builder.Services.AddScoped(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
});

// Add Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add API Controllers for backward compatibility
builder.Services.AddControllers();

// Add MongoDB user checker for diagnostics
builder.Services.AddScoped<OldandNewClone.Web.Services.MongoDbUserChecker>();
builder.Services.AddScoped<OldandNewClone.Web.Services.PasswordFieldMigration>();

// Add UserStateService — shared between Web and MAUI (lives in Application layer)
builder.Services.AddScoped<OldandNewClone.Application.Services.UserStateService>();

var app = builder.Build();

// Migrate password fields from Node.js format to .NET format
using (var scope = app.Services.CreateScope())
{
    var startupLogger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("Startup");

    try
    {
        var passwordMigration = scope.ServiceProvider.GetRequiredService<OldandNewClone.Web.Services.PasswordFieldMigration>();
        var migrationResult = await passwordMigration.MigratePasswordFieldsAsync();

        startupLogger.LogInformation(
            "Password migration startup run complete. Migrated: {MigratedUsers}, Failed: {FailedUsers}, Remaining: {UsersNeedingMigration}",
            migrationResult.MigratedUsers,
            migrationResult.FailedUsers,
            migrationResult.Status.UsersNeedingMigration);
    }
    catch (Exception ex)
    {
        startupLogger.LogError(ex, "Password migration failed during startup");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowSpecificOrigins");

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map API Controllers
app.MapControllers();

app.Run();
