using OldandNewClone.Application.Interfaces;
using OldandNewClone.Domain.Common;
using System.Text.RegularExpressions;

namespace OldandNewClone.Application.Services;

public class TransposeService : ITransposeService
{
    private static readonly string[] ChromaticScale = new[]
    {
        "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"
    };

    private static readonly Dictionary<string, string> FlatToSharp = new()
    {
        { "Db", "C#" }, { "Eb", "D#" }, { "Gb", "F#" }, { "Ab", "G#" }, { "Bb", "A#" }
    };

    public string TransposeLyrics(string lyrics, int semitones)
    {
        if (string.IsNullOrEmpty(lyrics) || semitones == 0) return lyrics;

        // Match chord patterns (e.g., C, Cm, C#, Dm7, F#m, etc.)
        var chordPattern = @"\b([A-G][#b]?m?[0-9]?)\b";

        return Regex.Replace(lyrics, chordPattern, match =>
        {
            var chord = match.Groups[1].Value;
            return TransposeChord(chord, semitones);
        });
    }

    public string TransposeChord(string chord, int semitones)
    {
        if (string.IsNullOrEmpty(chord)) return chord;

        // Extract root note, modifier (# or b), quality (m, maj, etc.), and extensions (7, 9, etc.)
        var match = Regex.Match(chord, @"^([A-G])([#b]?)(m?.*)?$");
        if (!match.Success) return chord;

        var root = match.Groups[1].Value;
        var accidental = match.Groups[2].Value;
        var suffix = match.Groups[3].Value;

        // Combine root and accidental
        var fullRoot = root + accidental;

        // Normalize flats to sharps
        if (FlatToSharp.ContainsKey(fullRoot))
        {
            fullRoot = FlatToSharp[fullRoot];
        }

        // Find current position in chromatic scale
        var currentIndex = Array.IndexOf(ChromaticScale, fullRoot);
        if (currentIndex == -1) return chord;

        // Calculate new position
        var newIndex = (currentIndex + semitones + 12) % 12;
        var newRoot = ChromaticScale[newIndex];

        return newRoot + suffix;
    }

    public int CalculateSemitones(string fromKey, string toKey)
    {
        return MusicKey.GetSemitoneDistance(fromKey, toKey);
    }
}
