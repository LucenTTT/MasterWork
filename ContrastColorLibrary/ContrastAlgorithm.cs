using System.Drawing;
using SkiaSharp;
using Maui = Microsoft.Maui.Graphics;

namespace ContrastColorLibrary;

public abstract class ContrastAlgorithm
{
    private static double CalculateLuminosity(double red, double green, double blue)
    {
        return 0.2126 * red + 0.7152 * green + 0.0722 * blue;
    }

    protected abstract double TransformColor(double value);
    
    protected abstract double CalculateContrastRatioCore(double luminosityA, double luminosityB);

    public double CalculateContrastRatio(SKColor backgroundColor, SKColor foregroundColor)
    {
        var luminosityA = CalculateLuminosity(
            TransformColor(BringTosRgb(backgroundColor.Red)),
            TransformColor(BringTosRgb(backgroundColor.Green)),
            TransformColor(BringTosRgb(backgroundColor.Blue)));
        
        var luminosityB = CalculateLuminosity(
            TransformColor(BringTosRgb(foregroundColor.Red)),
            TransformColor(BringTosRgb(foregroundColor.Green)),
            TransformColor(BringTosRgb(foregroundColor.Blue)));

        return CalculateContrastRatioCore(luminosityA, luminosityB);
    }

    public double CalculateContrastRatio(Color backgroundColor, Color foregroundColor)
    {
        var luminosityA = CalculateLuminosity(
            TransformColor(BringTosRgb(backgroundColor.R)),
            TransformColor(BringTosRgb(backgroundColor.G)),
            TransformColor(BringTosRgb(backgroundColor.B)));
        
        var luminosityB = CalculateLuminosity(
            TransformColor(BringTosRgb(foregroundColor.R)),
            TransformColor(BringTosRgb(foregroundColor.G)),
            TransformColor(BringTosRgb(foregroundColor.B)));

        return CalculateContrastRatioCore(luminosityA, luminosityB);
    }

    public double CalculateContrastRatio(Maui.Color backgroundColor, Maui.Color foregroundColor)
    {
        var luminosityA = CalculateLuminosity(
            TransformColor(backgroundColor.Red),
            TransformColor(backgroundColor.Green),
            TransformColor(backgroundColor.Blue));
        
        var luminosityB = CalculateLuminosity(
            TransformColor(foregroundColor.Red),
            TransformColor(foregroundColor.Green),
            TransformColor(foregroundColor.Blue));

        return CalculateContrastRatioCore(luminosityA, luminosityB);
    }
    
    private double BringTosRgb(double color)
    {
        return color / 255;
    }
}