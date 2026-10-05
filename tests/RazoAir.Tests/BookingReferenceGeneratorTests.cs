using RazoAir.Web.Data;

namespace RazoAir.Tests;

public class BookingReferenceGeneratorTests
{
    [Fact]
    public void Generate_ReturnsExpectedLength()
    {
        var reference = BookingReferenceGenerator.Generate();

        Assert.Equal(BookingReferenceGenerator.ReferenceLength, reference.Length);
    }

    [Fact]
    public void Generate_UsesOnlyAllowedAlphabetCharacters()
    {
        for (var i = 0; i < 100; i++)
        {
            var reference = BookingReferenceGenerator.Generate();
            foreach (var ch in reference)
            {
                Assert.Contains(ch, BookingReferenceGenerator.Alphabet);
            }
        }
    }

    [Fact]
    public void Generate_ProducesHighEntropyUniqueValues()
    {
        const int count = 1000;
        var generated = new HashSet<string>(StringComparer.Ordinal);

        for (var i = 0; i < count; i++)
        {
            var reference = BookingReferenceGenerator.Generate();
            generated.Add(reference);
        }

        // In 1000 random 6-character selections from 32-character alphabet (1,073,741,824 space),
        // collision probability is ~ 0.00046. All 1000 should be unique.
        Assert.Equal(count, generated.Count);
    }
}
