using System.Drawing;
using ContrastColorLibrary.Models;
using SkiaSharp;
using Maui = Microsoft.Maui.Graphics;
namespace ContrastColorLibrary;

public class ContrastService
{
    private readonly WcagAlgorithm _wcag;
    private readonly ApcaAlgorithm _apca;

    public ContrastService()
    {
        _wcag = new WcagAlgorithm();
        _apca = new ApcaAlgorithm();
    }
    
    public ContrastRatioResult CalculateContrastRatio(SKColor backgroundColor, SKColor foregroundColor)
    {
        return new ContrastRatioResult(_wcag.CalculateContrastRatio(backgroundColor, foregroundColor),
            _apca.CalculateContrastRatio(backgroundColor, foregroundColor));
    }

    public double CalculateContrastRatioApca(SKColor backgroundColor, SKColor foregroundColor)
    {
        return _apca.CalculateContrastRatio(backgroundColor, foregroundColor);
    }

    public double CalculateContrastRatioWcag(SKColor backgroundColor, SKColor foregroundColor)
    {
        return _wcag.CalculateContrastRatio(backgroundColor, foregroundColor);
    }
    
    public ContrastRatioResult CalculateContrastRatio(Color backgroundColor, Color foregroundColor)
    {
        return new ContrastRatioResult(_wcag.CalculateContrastRatio(backgroundColor, foregroundColor),
            _apca.CalculateContrastRatio(backgroundColor, foregroundColor));
    }

    public double CalculateContrastRatioApca(Maui.Color backgroundColor, Maui.Color foregroundColor)
    {
        return _apca.CalculateContrastRatio(backgroundColor, foregroundColor);
    }

    public double CalculateContrastRatioWcag(Maui.Color backgroundColor, Maui.Color foregroundColor)
    {
        return _wcag.CalculateContrastRatio(backgroundColor, foregroundColor);
    }

    public MostContrastColorAlgorithmsResult<SKColor> FindMostContrastColorsFromPicture(SKBitmap picture)
    {
        var distinctColors = picture.Pixels.Distinct().ToList();
        SKColor colorApca1 = SKColor.Empty;
        SKColor colorApca2 = SKColor.Empty;
        SKColor colorWcag1 = SKColor.Empty;
        SKColor colorWcag2 = SKColor.Empty;
        double maxApca = double.NegativeInfinity;
        double maxWcag = double.NegativeInfinity;
        var apcaList = new Dictionary<(SKColor, SKColor), double>();
        var wcagList = new Dictionary<(SKColor, SKColor), double>();
        for (int i = 0; i < distinctColors.Count; i++)
        {
            for (int j = distinctColors.Count - 1; j >= 0; j--)
            {
                var apca = _apca.CalculateContrastRatio(distinctColors[i], distinctColors[j]);
                var wcag = _wcag.CalculateContrastRatio(distinctColors[i], distinctColors[j]);
                apcaList.Add((distinctColors[i], distinctColors[j]), apca);
                wcagList.Add((distinctColors[i], distinctColors[j]), wcag);
                if (maxApca < Math.Abs(apca))
                {
                    maxApca = Math.Abs(apca);
                    colorApca1 = distinctColors[i];
                    colorApca2 = distinctColors[j]; 
                }

                if (maxWcag < wcag)
                {
                    maxWcag = wcag;
                    colorWcag1 = distinctColors[i];
                    colorWcag2 = distinctColors[j];
                }
            }
        }

        var wcagResult = new MostContrastColorsResult<SKColor>(maxWcag, colorWcag1, colorWcag2);
        var apcaResult = new MostContrastColorsResult<SKColor>(maxApca, colorApca1, colorApca2);
        return new MostContrastColorAlgorithmsResult<SKColor>(wcagResult, apcaResult);
    }
    
    public MostContrastColorsResult<SKColor> FindMostContrastColorsFromPictureByWcag(SKBitmap picture)
    {
        var distinctColors = picture.Pixels.Distinct().ToList();
        SKColor colorWcag1 = SKColor.Empty;
        SKColor colorWcag2 = SKColor.Empty;
        double maxWcag = double.NegativeInfinity;
        var wcagList = new Dictionary<(SKColor, SKColor), double>();
        for (int i = 0; i < distinctColors.Count; i++)
        {
            for (int j = distinctColors.Count - 1; j >= 0; j--)
            {
                var wcag = _wcag.CalculateContrastRatio(distinctColors[i], distinctColors[j]);
                wcagList.Add((distinctColors[i], distinctColors[j]), wcag);

                if (maxWcag < wcag)
                {
                    maxWcag = wcag;
                    colorWcag1 = distinctColors[i];
                    colorWcag2 = distinctColors[j];
                }
            }
        }

        return new MostContrastColorsResult<SKColor>(maxWcag, colorWcag1, colorWcag2);
    }
    
    public MostContrastColorsResult<SKColor> FindMostContrastColorsFromPictureByApca(SKBitmap picture)
    {
        var distinctColors = picture.Pixels.Distinct().ToList();
        SKColor colorApca1 = SKColor.Empty;
        SKColor colorApca2 = SKColor.Empty;
        double maxApca = double.NegativeInfinity;
        double maxWcag = double.NegativeInfinity;
        var apcaList = new Dictionary<(SKColor, SKColor), double>();
        for (int i = 0; i < distinctColors.Count; i++)
        {
            for (int j = distinctColors.Count - 1; j >= 0; j--)
            {
                var apca = _apca.CalculateContrastRatio(distinctColors[i], distinctColors[j]);
                apcaList.Add((distinctColors[i], distinctColors[j]), apca);
                if (maxApca < Math.Abs(apca))
                {
                    maxApca = Math.Abs(apca);
                    colorApca1 = distinctColors[i];
                    colorApca2 = distinctColors[j]; 
                }
            }
        }

        return new MostContrastColorsResult<SKColor>(maxApca, colorApca1, colorApca2);
    }

    public MostContrastColorAlgorithmsResult<SKColor> FindMostContrastColorFromList(SKColor color, params SKColor[] vsColors)
    {
        var apcaMax = double.NegativeInfinity;
        var wcagMax = double.NegativeInfinity;
        SKColor apcaColor = SKColor.Empty;
        SKColor wcagColor = SKColor.Empty;
        
        foreach (var skColor in vsColors)
        {
            var apca = _apca.CalculateContrastRatio(color, skColor);
            var wcag = _apca.CalculateContrastRatio(color, skColor);

            if (Math.Abs(apcaMax) < Math.Abs(apca))
            {
                apcaColor = skColor;
                apcaMax = apca;
            }

            if (wcagMax < wcag)
            {
                wcagColor = skColor;
                wcagMax = wcag;
            }
        }

        var wcagResult = new MostContrastColorsResult<SKColor>(wcagMax, color, wcagColor);
        var apcaResult = new MostContrastColorsResult<SKColor>(apcaMax, color, apcaColor);

        return new MostContrastColorAlgorithmsResult<SKColor>(wcagResult, apcaResult);
    }
    
    public MostContrastColorsResult<SKColor> FindMostContrastColorFromListByApca(SKColor color, params SKColor[] vsColors)
    {
        var apcaMax = double.NegativeInfinity;
        SKColor apcaColor = SKColor.Empty;
        
        foreach (var skColor in vsColors)
        {
            var apca = _apca.CalculateContrastRatio(color, skColor);
            var wcag = _apca.CalculateContrastRatio(color, skColor);

            if (Math.Abs(apcaMax) < Math.Abs(apca))
            {
                apcaColor = skColor;
                apcaMax = apca;
            }
        }
        
        return new MostContrastColorsResult<SKColor>(apcaMax, color, apcaColor);
    }
    
    public MostContrastColorsResult<SKColor> FindMostContrastColorFromListByWcag(SKColor color, params SKColor[] vsColors)
    {
        var wcagMax = double.NegativeInfinity;
        SKColor wcagColor = SKColor.Empty;
        
        foreach (var skColor in vsColors)
        {
            var wcag = _apca.CalculateContrastRatio(color, skColor);

            if (wcagMax < wcag)
            {
                wcagColor = skColor;
                wcagMax = wcag;
            }
        }

        return new MostContrastColorsResult<SKColor>(wcagMax, color, wcagColor);
    }
    
    public MostContrastColorAlgorithmsResult<Maui.Color> FindMostContrastColorFromList(Maui.Color color, params Maui.Color[] vsColors)
    {
        var apcaMax = double.NegativeInfinity;
        var wcagMax = double.NegativeInfinity;
        Maui.Color apcaColor = null;
        Maui.Color wcagColor = null;
        
        foreach (var skColor in vsColors)
        {
            var apca = _apca.CalculateContrastRatio(color, skColor);
            var wcag = _apca.CalculateContrastRatio(color, skColor);

            if (Math.Abs(apcaMax) < Math.Abs(apca))
            {
                apcaColor = skColor;
                apcaMax = apca;
            }

            if (wcagMax < wcag)
            {
                wcagColor = skColor;
                wcagMax = wcag;
            }
        }

        var wcagResult = new MostContrastColorsResult<Maui.Color>(wcagMax, color, wcagColor);
        var apcaResult = new MostContrastColorsResult<Maui.Color>(apcaMax, color, apcaColor);

        return new MostContrastColorAlgorithmsResult<Maui.Color>(wcagResult, apcaResult);
    }
    
    public MostContrastColorsResult<Maui.Color> FindMostContrastColorFromListByApca(Maui.Color color, params Maui.Color[] vsColors)
    {
        var apcaMax = double.NegativeInfinity;
        Maui.Color apcaColor = null;
        
        foreach (var skColor in vsColors)
        {
            var apca = _apca.CalculateContrastRatio(color, skColor);
            var wcag = _apca.CalculateContrastRatio(color, skColor);

            if (Math.Abs(apcaMax) < Math.Abs(apca))
            {
                apcaColor = skColor;
                apcaMax = apca;
            }
        }
        
        return new MostContrastColorsResult<Maui.Color>(apcaMax, color, apcaColor);
    }
    
    public MostContrastColorsResult<Maui.Color> FindMostContrastColorFromListByWcag(Maui.Color color, params Maui.Color[] vsColors)
    {
        var wcagMax = double.NegativeInfinity;
        Maui.Color wcagColor = null;
        
        foreach (var skColor in vsColors)
        {
            var wcag = _apca.CalculateContrastRatio(color, skColor);

            if (wcagMax < wcag)
            {
                wcagColor = skColor;
                wcagMax = wcag;
            }
        }

        return new MostContrastColorsResult<Maui.Color>(wcagMax, color, wcagColor);
    }
    
    public MostContrastColorAlgorithmsResult<Color> FindMostContrastColorFromList(Color color, params Color[] vsColors)
    {
        var apcaMax = double.NegativeInfinity;
        var wcagMax = double.NegativeInfinity;
        Color apcaColor = Color.Empty;
        Color wcagColor = Color.Empty;
        
        foreach (var skColor in vsColors)
        {
            var apca = _apca.CalculateContrastRatio(color, skColor);
            var wcag = _apca.CalculateContrastRatio(color, skColor);

            if (Math.Abs(apcaMax) < Math.Abs(apca))
            {
                apcaColor = skColor;
                apcaMax = apca;
            }

            if (wcagMax < wcag)
            {
                wcagColor = skColor;
                wcagMax = wcag;
            }
        }

        var wcagResult = new MostContrastColorsResult<Color>(wcagMax, color, wcagColor);
        var apcaResult = new MostContrastColorsResult<Color>(apcaMax, color, apcaColor);

        return new MostContrastColorAlgorithmsResult<Color>(wcagResult, apcaResult);
    }
    
    public MostContrastColorsResult<Color> FindMostContrastColorFromListByApca(Color color, params Color[] vsColors)
    {
        var apcaMax = double.NegativeInfinity;
        Color apcaColor = Color.Empty;
        
        foreach (var skColor in vsColors)
        {
            var apca = _apca.CalculateContrastRatio(color, skColor);
            var wcag = _apca.CalculateContrastRatio(color, skColor);

            if (Math.Abs(apcaMax) < Math.Abs(apca))
            {
                apcaColor = skColor;
                apcaMax = apca;
            }
        }
        
        return new MostContrastColorsResult<Color>(apcaMax, color, apcaColor);
    }
    
    public MostContrastColorsResult<Color> FindMostContrastColorFromListByWcag(Color color, params Color[] vsColors)
    {
        var wcagMax = double.NegativeInfinity;
        Color wcagColor = Color.Empty;
        
        foreach (var skColor in vsColors)
        {
            var wcag = _apca.CalculateContrastRatio(color, skColor);

            if (wcagMax < wcag)
            {
                wcagColor = skColor;
                wcagMax = wcag;
            }
        }

        return new MostContrastColorsResult<Color>(wcagMax, color, wcagColor);
    }
}