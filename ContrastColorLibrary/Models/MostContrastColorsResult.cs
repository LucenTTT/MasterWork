using SkiaSharp;

namespace ContrastColorLibrary.Models;

public class MostContrastColorsResult<T>
{
    public MostContrastColorsResult(double contrastRatio, T firstColor, T secondColor)
    {
        ContrastRatio = contrastRatio;
        FirstColor = firstColor;
        SecondColor = secondColor;
    }

    public double ContrastRatio { get; }
    public T FirstColor { get; }
    public T SecondColor { get; }
}