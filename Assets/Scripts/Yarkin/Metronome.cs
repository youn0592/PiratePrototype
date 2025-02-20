using UnityEngine;

// The code example shows how to implement a metronome that procedurally
// generates the click sounds via the OnAudioFilterRead callback.
// While the game is paused or suspended, this time will not be updated and sounds
// playing will be paused. Therefore developers of music scheduling routines do not have
// to do any rescheduling after the app is unpaused

[RequireComponent(typeof(AudioSource))]
public class Metronome : MonoBehaviour
{
    public double _bpm = 140.0F;
    public float _gain = 0.5F;
    public int _signatureHi = 4;
    public int _signatureLo = 4;

    private double _nextTick = 0.0F;
    private float _amp = 0.0F;
    private float _phase = 0.0F;
    private double _sampleRate = 0.0F;
    private int _accent;
    private bool _running = false;

    void Start()
    {
        _accent = _signatureHi;
        double startTick = AudioSettings.dspTime;
        _sampleRate = AudioSettings.outputSampleRate;
        _nextTick = startTick * _sampleRate;
        _running = true;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!_running)
            return;

        double samplesPerTick = _sampleRate * 60.0F / _bpm * 4.0F / _signatureLo;
        double sample = AudioSettings.dspTime * _sampleRate;
        int dataLen = data.Length / channels;

        int n = 0;
        while (n < dataLen)
        {
            float x = _gain * _amp * Mathf.Sin(_phase);
            int i = 0;
            while (i < channels)
            {
                data[n * channels + i] += x;
                i++;
            }
            while (sample + n >= _nextTick)
            {
                _nextTick += samplesPerTick;
                _amp = 1.0F;
                if (++_accent > _signatureHi)
                {
                    _accent = 1;
                    _amp *= 2.0F;
                }
                Debug.Log("Tick: " + _accent + "/" + _signatureHi);
            }
            _phase += _amp * 0.3F;
            _amp *= 0.993F;
            n++;
        }
    }
}
