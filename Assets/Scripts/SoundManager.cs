using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource _generalAudioSource;
    [SerializeField] AudioSource _pitchedAudioSource;
    [SerializeField] float _pitchIncrement = 0.1f;

    [SerializeField] AudioClip _perfectAlignmentClip;
    [SerializeField] List<AudioClip> _stackCutClips;

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            _generalAudioSource.PlayOneShot(clip);
        }
    }

    public void PlayPerfectAlignmentSound(int perfectMatchCount)
    {
        _pitchedAudioSource.pitch = 1 + (perfectMatchCount * _pitchIncrement);
        _pitchedAudioSource.PlayOneShot(_perfectAlignmentClip);
    }

    public void PlayCutStackSoundRandomly()
    {
        var clip = _stackCutClips[Random.Range(0, _stackCutClips.Count)];
        PlaySound(clip);
    }
}
