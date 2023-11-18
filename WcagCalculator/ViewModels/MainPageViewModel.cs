using CommunityToolkit.Mvvm.ComponentModel;

namespace WcagCalculator.ViewModels;

public class MainPageViewModel : ObservableObject
{
    public Color BackgroundColor
    {
        get => _backgroundColor;
        set => SetProperty(ref _backgroundColor, value);
    }

    public Color ForegroundColor
    {
        get => _foregroundColor;
        set => SetProperty(ref _foregroundColor, value);
    }

    private Color _backgroundColor;
    private Color _foregroundColor;
    
}