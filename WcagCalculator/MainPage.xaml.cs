using Windows.ApplicationModel.DataTransfer;
using CommunityToolkit.Mvvm.Messaging;
using ContrastColorLibrary;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using WcagCalculator.ViewModels;
using Clipboard = Windows.ApplicationModel.DataTransfer.Clipboard;

namespace WcagCalculator;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _viewModel;
    private readonly IMessenger _messenger;
    private readonly ContrastService _contrastService;

    public MainPage(MainPageViewModel viewModel, IMessenger messenger, ContrastService contrastService)
    {
        _viewModel = viewModel;
        BindingContext = viewModel;
        _messenger = messenger;
        _contrastService = contrastService;
        
        InitializeComponent();
        Clipboard.ContentChanged += ClipboardOnContentChanged;
    }

    private async void ClipboardOnContentChanged(object sender, object e)
    {
        var content = Clipboard.GetContent();
        if (content.Contains(StandardDataFormats.Bitmap))
        {
            var stream = await content.GetBitmapAsync();
            var st = await stream.OpenReadAsync();
            var picture = SKBitmap.Decode(st.AsStreamForRead());
            var result = _contrastService.FindMostContrastColorsFromPicture(picture);
            using var borderTextPaint = new SKPaint { TextSize = 14, Color = SKColors.Black};
            using var surface = SKSurface.Create(new SKImageInfo(300,300));
            var currentTheme = Application.Current.RequestedTheme;
            if (currentTheme == AppTheme.Dark)
            {
                surface.Canvas.Clear(new SKColor(32,32,32,255));
                borderTextPaint.Color = new SKColor(224,224,224,255);
                borderTextPaint.Style = SKPaintStyle.Stroke;
                borderTextPaint.StrokeWidth = 6;
                surface.Canvas.DrawRect(10,10,90,90, borderTextPaint);
                surface.Canvas.DrawRect(10,195,90,90, borderTextPaint);
                surface.Canvas.DrawRect(200,10,90,90, borderTextPaint);
                surface.Canvas.DrawRect(200,195,90,90, borderTextPaint);
                borderTextPaint.Style = SKPaintStyle.Fill;
            }
            else
            {
                surface.Canvas.Clear(new SKColor(224,224,224,255));
                borderTextPaint.Color = new SKColor(32,32,32,255);
                borderTextPaint.Style = SKPaintStyle.Stroke;
                borderTextPaint.StrokeWidth = 6;
                surface.Canvas.DrawRect(10,10,90,90, borderTextPaint);
                surface.Canvas.DrawRect(10,195,90,90, borderTextPaint);
                surface.Canvas.DrawRect(200,10,90,90, borderTextPaint);
                surface.Canvas.DrawRect(200,195,90,90, borderTextPaint);
                borderTextPaint.Style = SKPaintStyle.Fill;
            }
            
            surface.Canvas.DrawRect(25,25, 60, 60, new SKPaint() {Color = result.Wcag.FirstColor});
            surface.Canvas.DrawRect(215,25, 60, 60, new SKPaint() {Color = result.Wcag.SecondColor});
            surface.Canvas.DrawText($"WCAG: {Math.Round(result.Wcag.ContrastRatio,0)}:1", 120,50, borderTextPaint);
            using var lowerTextPaint = borderTextPaint.Clone();
            lowerTextPaint.TextSize = 10;
            surface.Canvas.DrawText($"{result.Wcag.FirstColor.ToString()} vs {result.Wcag.SecondColor.ToString()}", 105,60, lowerTextPaint);
            
            surface.Canvas.DrawRect(25,210, 60, 60, new SKPaint() {Color = result.Wcag.FirstColor});
            surface.Canvas.DrawRect(215,210, 60, 60, new SKPaint() {Color = result.Wcag.SecondColor});
            surface.Canvas.DrawText($"APCA: {Math.Round(result.Apca.ContrastRatio,0)}", 120,240, borderTextPaint);
            surface.Canvas.DrawText($"{result.Apca.FirstColor.ToString()} vs {result.Apca.SecondColor.ToString()}", 105,250, lowerTextPaint);
            surface.Canvas.Flush();
            
            using var image = surface.Snapshot();
            
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            Directory.CreateDirectory(Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "ContrastCalculation"));
            var path = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "ContrastCalculation", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".jpg");
            await using var fileStream = File.Create(path);
            data.SaveTo(fileStream);
            
            new ToastContentBuilder()
                .AddArgument("ResultPath", path)
                .AddInlineImage(new Uri(path))
                .AddText("Screenshot was taken and there are the most contrast colors in this picture!")
                .AddText($"WCAG: {Math.Round(result.Wcag.ContrastRatio)}:1; {result.Wcag.FirstColor.ToString()} vs {result.Wcag.SecondColor.ToString()}")
                .AddText($"APCA: {Math.Round(result.Apca.ContrastRatio,0)}; {result.Apca.FirstColor.ToString()} vs {result.Apca.SecondColor.ToString()}")
                .Show();
        }
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        // var wcagContrast = _co.CalculateContrastRatio(SKColors.White, SKColors.Black);
        // var apcaContrast = _apcaAlgorithm.CalculateContrastRatio(SKColors.White, SKColors.Black);
    }

    private double picture()
    {
        var bitmap = SKBitmap.Decode("C:\\Users\\kotmo\\Downloads\\29390c4e756d42e1b23c68682db3e216.jpg");
        var colors = bitmap.Pixels.Distinct().ToList();
        SKColor color1;
        SKColor color2;
        double max = double.NegativeInfinity;
        for (int i = 0; i < colors.Count; i++)
        {
            for (int j = colors.Count - 1; j >= 0; j--)
            {
                var apca = APCA(colors[i], colors[j]);
                if (max < Math.Abs(apca))
                {
                    max = Math.Abs(apca);
                    color1 = colors[i];
                    color2 = colors[j]; 
                }
            }
        }

        return max;
    }
    
    public async Task<Stream> ConvertImageSourceToStreamAsync(ImageSource imageSource)
    {
        using var stream = await ((StreamImageSource)imageSource).Stream(CancellationToken.None);
        return stream;
    }
    
    private double TransformColor(double color)
    {
        if (color <= 0.03928)
        {
            return color / 12.92;
        }
        else
        {
            return Math.Pow((color + 0.055) / 1.055, 2.4);
        }
    }

    private double WcagLuminosityCalculation(double red, double green, double blue)
    {
        return 0.2126 * red + 0.7152 * green + 0.0722 * blue;
    }

    private double WCAG(SKColor fg, SKColor bg)
    {
        var luminosityA = WcagLuminosityCalculation(TransformColor(fg.Red / 255),
            TransformColor(fg.Green / 255),
            TransformColor(fg.Blue / 255));
        
        var luminosityB = WcagLuminosityCalculation(TransformColor(bg.Red / 255),
            TransformColor(bg.Green / 255),
            TransformColor(bg.Blue / 255));
        
        var luminosity1 = Math.Max(luminosityA, luminosityB);
        var luminosity2 = Math.Min(luminosityA, luminosityB);
        var contrastRatio = (luminosity1 + 0.05) / (luminosity2 + 0.05);
        return contrastRatio;
    }

    private double APCA(SKColor fg, SKColor bg)
    {
        var luminosity1 = sRGBToY(fg);
        var luminosity2 = sRGBToY(bg);
        var c = 1.14d;

        if (luminosity2 > luminosity1)
        {
            c *= Math.Pow(luminosity2, 0.56) - Math.Pow(luminosity1, 0.57);
        }
        else
        {
            c *= Math.Pow(luminosity2, 0.65) - Math.Pow(luminosity1, 0.62);
        }

        if (Math.Abs(c) < 0.1)
        {
            return 0;
        }
        else if (c > 0)
        {
            c -= 0.027;
        }
        else
        {
            c += 0.027;
        }

        return c * 100;
    }

    private double sRGBToY(SKColor color)
    {
        var r = Math.Pow(color.Red / 255, 2.4);
        var g = Math.Pow(color.Green / 255, 2.4);
        var b = Math.Pow(color.Blue / 255, 2.4);
        var y = 0.2126729 * r + 0.7151522 * g + 0.0721750 * b;

        if (y < 0.022) {
            y += Math.Pow(0.022 - y, 1.414);
        }
        return y;
    }
}