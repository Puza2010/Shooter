using UnityEngine;

public class SoundEffectsCheck : MonoBehaviour
{
    public AudioClip[] audioClips;

    public void Play()
    {
        _audioSource.PlayOneShot(audioClips[_index]);

        Debug.Log(audioClips[_index].name);

        _index++;
        if (_index >= audioClips.Length) _index = 0;
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    AudioSource _audioSource;
    int _index;
}
