namespace ContrastColorLibrary;

public class ApcaAlgorithm : ContrastAlgorithm
{
    protected override double TransformColor(double value)
    {
        return Math.Pow(value, 2.4);
    }

    protected override double CalculateContrastRatioCore(double luminosityA, double luminosityB)
    {
        var luminosity1 = TransformLuminosity(luminosityA);
        var luminosity2 = TransformLuminosity(luminosityB);
        
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

    private double TransformLuminosity(double y)
    {
        if (y < 0.022) {
            y += Math.Pow(0.022 - y, 1.414);
        }
        return y;
    }
}