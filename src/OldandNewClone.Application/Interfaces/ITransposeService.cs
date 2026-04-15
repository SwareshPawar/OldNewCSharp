namespace OldandNewClone.Application.Interfaces;

public interface ITransposeService
{
    string TransposeLyrics(string lyrics, int semitones);
    string TransposeChord(string chord, int semitones);
    int CalculateSemitones(string fromKey, string toKey);
}
