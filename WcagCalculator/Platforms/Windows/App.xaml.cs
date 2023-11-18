using System.Diagnostics;
using Windows.Foundation.Collections;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WcagCalculator.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
        ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompatOnOnActivated;
    }

    private void ToastNotificationManagerCompatOnOnActivated(ToastNotificationActivatedEventArgsCompat e)
    {
        ToastArguments args = ToastArguments.Parse(e.Argument);

        // Obtain any user input (text boxes, menu selections) from the notification
        ValueSet userInput = e.UserInput;

        // Need to dispatch to UI thread if performing UI operations
        Process.Start("explorer", args.Get("ResultPath"));
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}