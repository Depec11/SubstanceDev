using Substance.Audio;

namespace Substance.Nodes;

public class AudioSource : Node
{
    public SoundSource? Source { get; set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            if (field is null)
            {
                return;
            }

            field.IsLooping = IsLooping;
        } }
    public bool IsLooping { get; set
        {
            if (field == value)
            {
                return;
            }

            if (Source is null)
            {
                return;
            }

            field = value;

            Source.IsLooping = value;
        } } = false;
    public bool IsPlaying { get; set; } = false;

    public AudioSource()
    {
    }

    public void Play()
    {
        if (Source is null)
        {
            return;
        }

        if (IsPlaying)
        {
            return;
        }

        IsPlaying = true;
        AudioServer.Current.PlaySound(Source.Sid);
    }

    public void Pause()
    {
        if (!IsPlaying)
        {
            return;
        }

        IsPlaying = false;

        if (Source is null)
        {
            return;
        }

        AudioServer.Current.PauseSound(Source.Sid);
    }

    public void Stop()
    {
        if (!IsPlaying)
        {
            return;
        }

        IsPlaying = false;

        if (Source is null)
        {
            return;
        }

        AudioServer.Current.StopSound(Source.Sid);
    }
}