using Microsoft.Extensions.Logging;
using OldandNewClone.Application.Services;
using OldandNewClone.Application;
using OldandNewClone.Infrastructure;

namespace OldandNewClone.MobileDesktop;

public static class MauiProgram
{
    // Base URL of the running Web API server.
    // Android emulator routes 10.0.2.2 → host machine's localhost.
    // All other platforms (Windows, iOS simulator, Mac) use localhost directly.
#if ANDROID
    private const string ApiBaseUrl = "https://10.0.2.2:7005/";
#else
    private const string ApiBaseUrl = "https://localhost:7005/";
#endif

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        // Register HttpClient pointing to the Web API server.
        // In DEBUG builds, bypass SSL certificate validation so the ASP.NET Core
        // dev certificate is accepted on Android emulator and iOS simulator.
        builder.Services.AddScoped(sp =>
        {
#if DEBUG
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            return new HttpClient(handler) { BaseAddress = new Uri(ApiBaseUrl) };
#else
            return new HttpClient { BaseAddress = new Uri(ApiBaseUrl) };
#endif
        });

        // Shared Application services
        builder.Services.AddApplication();
        builder.Services.AddScoped<UserStateService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
