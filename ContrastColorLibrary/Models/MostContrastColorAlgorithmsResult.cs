namespace ContrastColorLibrary.Models;

public class MostContrastColorAlgorithmsResult<T>
{
    public MostContrastColorAlgorithmsResult(MostContrastColorsResult<T> wcag, MostContrastColorsResult<T> apca)
    {
        Wcag = wcag;
        Apca = apca;
    }

    public MostContrastColorsResult<T> Wcag { get; }
    public MostContrastColorsResult<T> Apca { get; }
}