namespace RazoAir.Web.Security;

public static class ImageValidator
{
    public static bool IsValidImageSignature(Stream stream, string extension)
    {
        Span<byte> header = stackalloc byte[12];
        var bytesRead = stream.Read(header);
        if (bytesRead < 4) return false;

        return extension switch
        {
            ".png" => bytesRead >= 8 &&
                      header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 &&
                      header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A,
            ".jpg" or ".jpeg" => header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            ".gif" => bytesRead >= 6 &&
                      header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38 &&
                      (header[4] == 0x37 || header[4] == 0x39) && header[5] == 0x61,
            ".webp" => bytesRead >= 12 &&
                       header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
                       header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50,
            _ => false
        };
    }
}
