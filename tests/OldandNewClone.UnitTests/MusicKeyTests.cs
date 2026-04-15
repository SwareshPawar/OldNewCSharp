using Xunit;
using OldandNewClone.Domain.Common;

namespace OldandNewClone.UnitTests;

public class MusicKeyTests
{
    [Theory]
    [InlineData("C", "D", 2)]
    [InlineData("C", "C", 0)]
    [InlineData("C", "B", 11)]
    [InlineData("G", "C", 5)]
    public void GetSemitoneDistance_ShouldCalculateCorrectly(string from, string to, int expected)
    {
        // Act
        var result = MusicKey.GetSemitoneDistance(from, to);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("C", "C")]
    [InlineData("Db", "C#")]
    [InlineData("Cm", "C")]
    [InlineData("F#m", "F#")]
    public void GetRoot_ShouldNormalizeCorrectly(string key, string expectedRoot)
    {
        // Act
        var result = MusicKey.GetRoot(key);

        // Assert
        Assert.Equal(expectedRoot, result);
    }

    [Theory]
    [InlineData("Cm", true)]
    [InlineData("C", false)]
    [InlineData("F#m", true)]
    [InlineData("G", false)]
    public void IsMinor_ShouldIdentifyCorrectly(string key, bool expectedIsMinor)
    {
        // Act
        var result = MusicKey.IsMinor(key);

        // Assert
        Assert.Equal(expectedIsMinor, result);
    }
}
