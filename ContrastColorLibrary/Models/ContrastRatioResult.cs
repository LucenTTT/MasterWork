namespace ContrastColorLibrary.Models;

public class ContrastRatioResult
{
    public ContrastRatioResult(double wcagContastRatio, double apcaContrastRatio)
    {
        WcagContastRatio = wcagContastRatio;
        ApcaContrastRatio = apcaContrastRatio;
    }

    public double WcagContastRatio { get; }
    public double ApcaContrastRatio { get; }
}