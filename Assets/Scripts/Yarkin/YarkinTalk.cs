using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class YarkinTalk : MonoBehaviour
{
    [SerializeField]
    YarkinVoiceDefinition _yarkinVoice;

    AudioSource _audioSource;
    int _playedAudioIndex = 0;

    [SerializeField]
    float _volume = 0.25f;

    public bool yarkin;

    private void OnValidate()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        Debug.Assert(_yarkinVoice.GetAudioClipsCount() != 0, "No audio clips found! Did you set up the Yarkin Voice Definition?");
    }

    // Update is called once per frame
    void Update()
    {
        if (yarkin)
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.clip = _yarkinVoice.GetAudioClips()[_playedAudioIndex];
                _audioSource.pitch = Random.Range(_yarkinVoice._pitchMinMax.x, _yarkinVoice._pitchMinMax.y);
                _audioSource.volume = _volume;
                _audioSource.Play();
                _playedAudioIndex = (_playedAudioIndex + 1) % (_yarkinVoice.GetAudioClipsCount());
            }
        }
    }
}
