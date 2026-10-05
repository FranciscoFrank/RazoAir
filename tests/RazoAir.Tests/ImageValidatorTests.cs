using RazoAir.Web.Security;

namespace RazoAir.Tests;

public class ImageValidatorTests
{
    [Fact]
    public void IsValidImageSignature_RecognizesValidPng()
    {
        byte[] validPngHeader = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D];
        using var stream = new MemoryStream(validPngHeader);

        var isValid = ImageValidator.IsValidImageSignature(stream, ".png");

        Assert.True(isValid);
    }

    [Fact]
    public void IsValidImageSignature_RecognizesValidJpeg()
    {
        byte[] validJpegHeader = [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01];
        using var stream = new MemoryStream(validJpegHeader);

        var isValid = ImageValidator.IsValidImageSignature(stream, ".jpg");

        Assert.True(isValid);
    }

    [Fact]
    public void IsValidImageSignature_RecognizesValidGif()
    {
        byte[] validGifHeader = [0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
        using var stream = new MemoryStream(validGifHeader);

        var isValid = ImageValidator.IsValidImageSignature(stream, ".gif");

        Assert.True(isValid);
    }

    [Fact]
    public void IsValidImageSignature_RecognizesValidWebp()
    {
        byte[] validWebpHeader = [
            0x52, 0x49, 0x46, 0x46, // RIFF
            0x24, 0x00, 0x00, 0x00, // Size
            0x57, 0x45, 0x42, 0x50  // WEBP
        ];
        using var stream = new MemoryStream(validWebpHeader);

        var isValid = ImageValidator.IsValidImageSignature(stream, ".webp");

        Assert.True(isValid);
    }

    [Fact]
    public void IsValidImageSignature_RejectsCorruptOrMismatchedContent()
    {
        byte[] fakeTextFile = "MZExecutableDummyPayload12345678"u8.ToArray();
        using var stream = new MemoryStream(fakeTextFile);

        var isPng = ImageValidator.IsValidImageSignature(stream, ".png");
        var isJpg = ImageValidator.IsValidImageSignature(stream, ".jpg");
        var isGif = ImageValidator.IsValidImageSignature(stream, ".gif");
        var isWebp = ImageValidator.IsValidImageSignature(stream, ".webp");

        Assert.False(isPng);
        Assert.False(isJpg);
        Assert.False(isGif);
        Assert.False(isWebp);
    }
}
