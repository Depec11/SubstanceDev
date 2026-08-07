namespace Substance.Audio;

public record class AudioData(int SampleRate, int Channels, int BitsPerSample, byte[] Data)
{
    public static readonly AudioData Empty = new(0, 0, 0, []);
}