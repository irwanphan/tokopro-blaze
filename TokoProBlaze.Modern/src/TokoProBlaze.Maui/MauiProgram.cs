using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TokoProBlaze.Maui.Configuration;
using TokoProBlaze.Maui.Services.Adapters;
using TokoProBlaze.Maui.Services.Ports;

namespace TokoProBlaze.Maui;

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

		var assembly = typeof(MauiProgram).Assembly;
		using (var stream = assembly.GetManifestResourceStream("TokoProBlaze.Maui.appsettings.json"))
		{
			if (stream is not null)
			{
				builder.Configuration.AddJsonStream(stream);
			}
		}

		builder.Services.AddHttpClient<ICustomerDirectoryReader, LegacyApiCustomerDirectoryReader>((sp, client) =>
		{
			var configuration = sp.GetRequiredService<IConfiguration>();
			client.BaseAddress = ApiBaseUriResolver.Resolve(configuration);
			client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
		});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
