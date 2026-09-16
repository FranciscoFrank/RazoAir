namespace RazoAir.Web.Data;

public static class BookingReferenceGenerator
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public static string Generate()
    {
        Span<char> buffer = stackalloc char[6];
        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = Alphabet[Random.Shared.Next(Alphabet.Length)];
        }
        return new string(buffer);
    }
}
