namespace ContrastColorLibrary;

public class WcagAlgorithm : ContrastAlgorithm
{
    protected override double TransformColor(double value)
    {
        if (value <= 0.03928)
        {
            return value / 12.92;
        }
        else
        {
            return Math.Pow((value + 0.055) / 1.055, 2.4);
        }
    }

    protected override double CalculateContrastRatioCore(double luminosityA, double luminosityB)
    {
        var luminosity1 = Math.Max(luminosityA, luminosityB);
        var luminosity2 = Math.Min(luminosityA, luminosityB);
        var contrastRatio = (luminosity1 + 0.05) / (luminosity2 + 0.05);
        return contrastRatio;
    }
}