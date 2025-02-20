using System;
using UnityEngine;

public class Sinus : MonoBehaviour
{
    public double _frequency = 440;
    public double _gain = 0.05;
    private double _increment = 0;
    private double _phase = 0;
    private double _sampling_frequency = 48000;

    private void Start()
    {
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        // update increment in case frequency has changed
        _increment = _frequency * 2 * Math.PI / _sampling_frequency;
        for (var i = 0; i < data.Length; i = i + channels)
        {
            _phase = _phase + _increment;
            // this is where we copy audio data to make them “available” to Unity
            data[i] = (float)(_gain * Math.Sin(_phase));
            // if we have stereo, we copy the mono data to each channel
            if (channels == 2) data[i + 1] = data[i];
            if (_phase > 2 * Math.PI) _phase = 0;
        }
    }

}