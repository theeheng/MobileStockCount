using DomainConstant;

namespace DomainInterface
{
    public interface IBarcodeResult
    {
        string Text { get; set; }
        BarcodeFormat Format { get; set; }
    }
}