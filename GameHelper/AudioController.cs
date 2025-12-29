using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace ReFMGame.GameHelper;
public class AudioController : IDisposable
{
    private readonly List<SoundEffectInstance> _activeSoundEffectInstances;
    private float _previousSoundEffectVolume;
    public bool IsMuted { get; private set; }
    public float Volume
    {
        get
        {
            if (IsMuted)
            {
                return 0.0f;
            }

            return SoundEffect.MasterVolume;
        }
        set
        {
            if (IsMuted)
            {
                return;
            }

            SoundEffect.MasterVolume = Math.Clamp(value, 0.0f, 1.0f);
        }
    }
    public bool IsDisposed { get; private set; }

    public AudioController()
    {
        _activeSoundEffectInstances = new List<SoundEffectInstance>();
    }

    ~AudioController() => Dispose(false);

    public void Update()
    {
        for (int i = _activeSoundEffectInstances.Count - 1; i >= 0; i--)
        {
            SoundEffectInstance instance = _activeSoundEffectInstances[i];

            if (instance.State == SoundState.Stopped)
            {
                if (!instance.IsDisposed)
                {
                    instance.Dispose();
                }
                _activeSoundEffectInstances.RemoveAt(i);
            }
        }
    }

    public SoundEffectInstance Play(SoundEffect soundEffect, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f, bool isLooped = false)
    {
        SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

        soundEffectInstance.Volume = volume;
        soundEffectInstance.Pitch = pitch;
        soundEffectInstance.Pan = pan;
        soundEffectInstance.IsLooped = isLooped;

        soundEffectInstance.Play();

        _activeSoundEffectInstances.Add(soundEffectInstance);

        return soundEffectInstance;
    }

    public void StopAll()
    {
        foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances)
        {
            soundEffectInstance.Stop();
        }
    }

    public void PauseAll()
    {
        foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances)
        {
            soundEffectInstance.Pause();
        }
    }

    public void ResumeAll()
    {
        foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances)
        {
            soundEffectInstance.Resume();
        }
    }

    public void MuteAudio()
    {
        _previousSoundEffectVolume = SoundEffect.MasterVolume;

        SoundEffect.MasterVolume = 0.0f;

        IsMuted = true;
    }

    public void UnmuteAudio()
    {
        SoundEffect.MasterVolume = _previousSoundEffectVolume;

        IsMuted = false;
    }

    public void ToggleMute()
    {
        if (IsMuted)
        {
            UnmuteAudio();
        }
        else
        {
            MuteAudio();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposing)
        {
            foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances)
            {
                soundEffectInstance.Dispose();
            }
            _activeSoundEffectInstances.Clear();
        }

        IsDisposed = true;
    }
}
