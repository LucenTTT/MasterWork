using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ContrastColorLibrary;

namespace WcagCalculator.ViewModels;

public class MainPageViewModel : ObservableObject
{
    private readonly ContrastService _contrastService;

    private Color _backgroundColor;
    private Color _foregroundColor;
    private string _backgroundColorHex;
    private string _foregroundColorHex;
    private double _wcagResult;
    private double _apcaResult;

    public string BackgroundColorHex
    {
        get => BackgroundColor != null ? BackgroundColor.ToArgbHex() : string.Empty;
        set
        {
            Regex argbRegex = new Regex("^#[0-9a-fA-F]{6}$");
            if (argbRegex.IsMatch(value))
            {
                BackgroundColor = Color.FromArgb(value);
            }
        }
    }

    public string ForegroundColorHex
    {
        get => ForegroundColor != null ? ForegroundColor.ToArgbHex() : string.Empty;
        set
        {
            Regex argbRegex = new Regex("^#[0-9a-fA-F]{6}$");
            if (argbRegex.IsMatch(value))
            {
                ForegroundColor = Color.FromArgb(value);
            }
        }
    }

    public Color BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            SetProperty(ref _backgroundColor, value); 
            OnPropertyChanged(nameof(BackgroundColorHex));
            CalculateContrast();
        }
    }

    public Color ForegroundColor
    {
        get => _foregroundColor;
        set
        {
            SetProperty(ref _foregroundColor, value); 
            OnPropertyChanged(nameof(ForegroundColorHex));
            CalculateContrast();
        }
    }
    
    public double WcagResult
    {
        get => _wcagResult;
        set => SetProperty(ref _wcagResult, value);
    }

    public double ApcaResult
    {
        get => _apcaResult;
        set => SetProperty(ref _apcaResult, value);
    }

    public RelayCommand SwapCommand { get; }

    public MainPageViewModel(ContrastService contrastService)
    {
        _contrastService = contrastService;
        SwapCommand = new RelayCommand(OnSwapping);
        var currentTheme = Application.Current.RequestedTheme;
        if (currentTheme == AppTheme.Dark)
        {
            BackgroundColor = Colors.Black;
            ForegroundColor = Colors.White;
        }
        else
        {
            BackgroundColor = Colors.White;
            ForegroundColor = Colors.Black;
        }
    }

    private void OnSwapping()
    {
        (BackgroundColor, ForegroundColor) = (ForegroundColor, BackgroundColor);
    }
    
    
    private void CalculateContrast()
    {
        if (BackgroundColor == null || ForegroundColor == null)
        {
            return;
        }
        var result = _contrastService.CalculateContrastRatio(BackgroundColor, ForegroundColor);
        WcagResult = result.WcagContastRatio;
        ApcaResult = result.ApcaContrastRatio;
    }
}