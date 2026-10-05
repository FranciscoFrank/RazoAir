using System.Security.Cryptography;

namespace RazoAir.Web.Data;

public static class BookingReferenceGenerator
{
    public const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    public const int ReferenceLength = 6;

    public static string Generate() =>
        RandomNumberGenerator.GetString(Alphabet, ReferenceLength);
}
