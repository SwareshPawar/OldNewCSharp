using OldandNewClone.Application.Services;
using OldandNewClone.Domain.Common;
using Xunit;

namespace OldandNewClone.UnitTests;

public class TransposeServiceTests
{
    private readonly TransposeService _transposeService;

    public TransposeServiceTests()
    {
        _transposeService = new TransposeService();
    }

    [Theory]
    [InlineData("C", "D", 2)]
    [InlineData("C", "G", 7)]
    [InlineData("Am", "Dm", 5)]
    [InlineData("G", "C", 5)]
    public void CalculateSemitones_ShouldReturnCorrectDistance(string fromKey, string toKey, int expectedSemitones)
    {
        // Act
        var result = _transposeService.CalculateSemitones(fromKey, toKey);

        // Assert
        Assert.Equal(expectedSemitones, result);
    }

    [Theory]
    [InlineData("C", 2, "D")]
    [InlineData("Am", 5, "Dm")]
    [InlineData("G7", 2, "A7")]
    [InlineData("Cmaj7", 3, "D#maj7")]
    public void TransposeChord_ShouldTransposeCorrectly(string chord, int semitones, string expected)
    {
        // Act
        var result = _transposeService.TransposeChord(chord, semitones);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TransposeLyrics_ShouldTransposeAllChords()
    {
        // Arrange
        var lyrics = "C G Am F\nVerse line\nC G";
        var expected = "D A Bm G\nVerse line\nD A";

        // Act
        var result = _transposeService.TransposeLyrics(lyrics, 2);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TransposeLyrics_WithZeroSemitones_ShouldReturnOriginal()
    {
        // Arrange
        var lyrics = "C G Am F";

        // Act
        var result = _transposeService.TransposeLyrics(lyrics, 0);

        // Assert
        Assert.Equal(lyrics, result);
    }
}
