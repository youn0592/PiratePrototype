using System;
using System.Collections.Generic;
using UnityEngine;
using Yarkin;

[RequireComponent(typeof(AudioSource))]
public class YarkinShanty : MonoBehaviour
{
    [SerializeField]
    YarkinVoiceDefinition _yarkinVoice;

    // AudioSource _audioSource;

    [SerializeField]
    public MidiAsset _midiAsset;

    [SerializeField]
    float _volume = 0.05f;

    private MidiPlayer _midiPlayer;
    private HashSet<MidiNote> _activeNotes;
    private double _sampling_frequency;

    void Awake()
    {
        SetupMidiPlayer();
        _activeNotes = new HashSet<MidiNote>();
        Debug.Assert(_yarkinVoice.GetAudioClipsCount() != 0, "No audio clips found! Did you set up the Yarkin Voice Definition?");
        // _audioSource = GetComponent<AudioSource>();
        _sampling_frequency = AudioSettings.outputSampleRate;
        //_audioSource.clip = _yarkinVoice.GetAudioClips()[1];
    }

    public void UpdatePlayer(float dt)
    {
        _midiPlayer.Update(dt);
    }

    void SetupMidiPlayer()
    {
        _midiPlayer = new MidiPlayer();
        _midiPlayer.SetSong(_midiAsset._midiSong);
        _midiPlayer.OnNoteOn += PlayNote;
        _midiPlayer.OnNoteOff += StopNote;
    }

    public void PlayShantyOneShot()
    {
        _midiPlayer.Restart();
    }

    private void PlayNote(MidiNote midiNote)
    {
        if (!_activeNotes.Contains(midiNote))
        {
            _activeNotes.Add(midiNote);
        }
        //_audioSource.pitch = note * 0.05f - 2f;
        //_audioSource.Play();
    }

    private void StopNote(MidiNote midiNote)
    {
        if (_activeNotes.Contains(midiNote))
        {
            _activeNotes.Remove(midiNote);
        }
        //_audioSource.pitch = note * 0.05f - 2f;
        //_audioSource.Play();
    }


    void OnAudioFilterRead(float[] data, int channels)
    {
        for (var i = 0; i < data.Length; i = i + channels)
        {
            double k = 0;

            foreach (MidiNote note in _activeNotes)
            {
                note._phase += note.GetFrequency() * 2 * Math.PI / _sampling_frequency;
                k += Math.Sin(note._phase);
                if (note._phase > 2 * Math.PI) note._phase -= 2 * Math.PI;
            }
            // this is where we copy audio data to make them “available” to Unity
            data[i] = (float)(_volume * k);
            // if we have stereo, we copy the mono data to each channel
            if (channels == 2) data[i + 1] = data[i];
        }
    }
}
