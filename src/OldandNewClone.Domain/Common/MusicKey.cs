namespace OldandNewClone.Domain.Common;

public static class MusicKey
{
    public static readonly string[] AllKeys = new[]
    {
        "C", "C#", "Db", "D", "D#", "Eb", "E", "F", "F#", "Gb", "G", "G#", "Ab", "A", "A#", "Bb", "B",
        "Cm", "C#m", "Dbm", "Dm", "D#m", "Ebm", "Em", "Fm", "F#m", "Gbm", "Gm", "G#m", "Abm", "Am", "A#m", "Bbm", "Bm"
    };

    public static readonly string[] MajorKeys = new[]
    {
        "C", "C#", "Db", "D", "D#", "Eb", "E", "F", "F#", "Gb", "G", "G#", "Ab", "A", "A#", "Bb", "B"
    };

    public static readonly string[] MinorKeys = new[]
    {
        "Cm", "C#m", "Dbm", "Dm", "D#m", "Ebm", "Em", "Fm", "F#m", "Gbm", "Gm", "G#m", "Abm", "Am", "A#m", "Bbm", "Bm"
    };

    private static readonly string[] ChromaticScale = new[]
    {
        "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"
    };

    public static int GetSemitoneDistance(string fromKey, string toKey)
    {
        var fromRoot = GetRoot(fromKey);
        var toRoot = GetRoot(toKey);

        var fromIndex = Array.IndexOf(ChromaticScale, fromRoot);
        var toIndex = Array.IndexOf(ChromaticScale, toRoot);

        if (fromIndex == -1 || toIndex == -1) return 0;

        return (toIndex - fromIndex + 12) % 12;
    }

    public static string GetRoot(string key)
    {
        if (string.IsNullOrEmpty(key)) return "C";

        // Remove 'm' for minor keys
        var root = key.Replace("m", "");

        // Normalize flats to sharps
        root = root switch
        {
            "Db" => "C#",
            "Eb" => "D#",
            "Gb" => "F#",
            "Ab" => "G#",
            "Bb" => "A#",
            _ => root
        };

        return root;
    }

    public static bool IsMinor(string key)
    {
        return key.EndsWith("m");
    }
}
