using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using portfolio_admin.Services;

namespace GenericAdminApp
{
    public static class MauiProgram
    {
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
            builder.Services.AddSingleton<GitService>();

            // appsettings.json betöltése
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // saját service regisztrálása
            builder.Services.AddSingleton<JsonRepository>();


#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
