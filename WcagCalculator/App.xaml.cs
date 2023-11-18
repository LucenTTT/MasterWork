using System.Diagnostics;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;

namespace WcagCalculator;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        
        MainPage = new AppShell();
    }

    private void ToastNotificationManagerCompatOnOnActivated(ToastNotificationActivatedEventArgsCompat e)
    {
        ToastArguments args = ToastArguments.Parse(e.Argument);

        // Obtain any user input (text boxes, menu selections) from the notification
        ValueSet userInput = e.UserInput;

        // Need to dispatch to UI thread if performing UI operations
        using Process fileopener = new Process();

        fileopener.StartInfo.FileName = "explorer";
        fileopener.StartInfo.Arguments = "\\" + args.Get("ResultPath") + "\\";
        
        Current.Quit();
    }
}