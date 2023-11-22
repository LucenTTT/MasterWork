using CommunityToolkit.Mvvm.Messaging;
using ContrastColorLibrary;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using WcagCalculator.ViewModels;

namespace WcagCalculator;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .RegisterServices()
            .RegisterViewModels()
            .RegisterViews()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("OpenSans-Light.ttf", "OpenSansLight");
                fonts.AddFont("OpenSans-Medium.ttf", "OpenSansMedium");
                fonts.AddFont("OpenSans-Bold.ttf", "OpenSansBold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IMessenger, WeakReferenceMessenger>();
        builder.Services.AddSingleton<ContrastService>();
        
        return builder;
    }
    
    private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<MainPageViewModel>();

        return builder;
    }
    
    private static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddTransient<MainPage>();

        return mauiAppBuilder;
    }
}