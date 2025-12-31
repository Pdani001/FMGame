using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReFMGame.GameHelper;
public class AudioController : IDisposable
{
    private readonly List<SoundEffectInstance> _activeSoundEffectInstances;
    private readonly Dictionary<string, SoundEffectInstance> _uniqueSoundEffectInstances;
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
        _activeSoundEffectInstances = [];
        _uniqueSoundEffectInstances = [];
    }

    ~AudioController() => Dispose(false);

    public void Update()
    {
        foreach (var kvp in _uniqueSoundEffectInstances)
        {
            SoundEffectInstance instance = kvp.Value;
            if (instance.State == SoundState.Stopped)
            {
                if (!instance.IsDisposed)
                {
                    instance.Dispose();
                }
                _uniqueSoundEffectInstances.Remove(kvp.Key);
            }
        }
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

    public SoundEffectInstance Play(SoundEffect soundEffect, float volume = 1.0f, float pitch = 0.0f, float pan = 0.0f, bool isLooped = false, bool unique = false)
    {
        SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

        soundEffectInstance.Volume = volume;
        soundEffectInstance.Pitch = pitch;
        soundEffectInstance.Pan = pan;
        soundEffectInstance.IsLooped = isLooped;

        soundEffectInstance.Play();

        if(unique)
        {
            if (_uniqueSoundEffectInstances.ContainsKey(soundEffect.Name))
            {
                SoundEffectInstance existingInstance = _uniqueSoundEffectInstances[soundEffect.Name];
                existingInstance.Stop();
                existingInstance.Dispose();
                _uniqueSoundEffectInstances.Remove(soundEffect.Name);
            }
            _uniqueSoundEffectInstances.Add(soundEffect.Name, soundEffectInstance);
            return soundEffectInstance;
        }
        _activeSoundEffectInstances.Add(soundEffectInstance);

        return soundEffectInstance;
    }

    public bool IsPlaying(string soundEffectName)
    {
        if (_uniqueSoundEffectInstances.TryGetValue(soundEffectName, out SoundEffectInstance instance))
        {
            return instance.State == SoundState.Playing;
        }
        return false;
    }

    public void Stop(string soundEffectName)
    {
        if (_uniqueSoundEffectInstances.TryGetValue(soundEffectName, out SoundEffectInstance instance))
        {
            instance.Stop();
        }
    }

    public void StopAll(Predicate<string> matchUnique = null)
    {
        matchUnique ??= _ => true;
        foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances)
        {
            soundEffectInstance.Stop();
        }
        foreach (var kvp in _uniqueSoundEffectInstances.Where(e=>matchUnique.Invoke(e.Key)))
        {
            kvp.Value.Stop();
        }
    }

    public void PauseAll()
    {
        foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances)
        {
            soundEffectInstance.Pause();
        }
        foreach (var kvp in _uniqueSoundEffectInstances)
        {
            kvp.Value.Pause();
        }
    }

    public void ResumeAll()
    {
        foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances)
        {
            soundEffectInstance.Resume();
        }
        foreach (var kvp in _uniqueSoundEffectInstances)
        {
            kvp.Value.Resume();
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
            foreach (var kvp in _uniqueSoundEffectInstances)
            {
                kvp.Value.Dispose();
            }
            _activeSoundEffectInstances.Clear();
            _uniqueSoundEffectInstances.Clear();
        }

        IsDisposed = true;
    }
}
