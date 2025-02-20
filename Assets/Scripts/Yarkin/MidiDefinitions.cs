using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Yarkin
{

    public enum MidiEventType
    {
        NoteOff = 0x80,           // note - velocity (ignored)
        NoteOn = 0x90,            // note - velocity
        NoteAftertouch = 0xA0,    // note - aftertouch value
        Controller = 0xB0,        // controller number - controller value
        ProgramChange = 0xC0,     // program number
        ChannelAftertouch = 0xD0, // aftertouch value
        PitchBend = 0xE0,         // pitch value (LSB) - pitch value (MSB)
        Meta = 0xF0               // pitch value (LSB) - pitch value (MSB)
    }

    public enum MidiMetaEventType
    {
        SequenceNumber = 0x00,
        TextEvent = 0x01,
        CopyrightNotice = 0x02,
        SequenceOrTrackName = 0x03,
        InstrumentName = 0x04,
        Lyrics = 0x05,
        Marker = 0x06,
        CuePoint = 0x07,
        MIDIChannelPrefix = 0x20,
        EndOfTrack = 0x2F,
        SetTempo = 0x51,
        TimeSignature = 0x54,
        KeySignature = 0x58
    }


    public class MidiNote
    {
        // Starts at C and ends at B
        private static float[] BaseNoteFrequencies =
        {
        16.35f,
        17.32f,
        18.35f,
        19.45f,
        20.6f,
        21.83f,
        23.12f,
        24.5f,
        25.96f,
        27.5f,
        29.14f,
        30.87f
        };

        public override bool Equals(object obj)
        {
            if (obj is MidiNote other)
            {
                return _note == other._note;
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Use a combination of NoteNumber
            return HashCode.Combine(_note, _note);
        }

        public MidiNote(byte note, byte velocity) { _note = note; _velocity = velocity; }

        private byte _note;
        private byte _velocity;
        public double _phase = 0;

        public double GetFrequency()
        {
            return BaseNoteFrequencies[_note % 12] * Math.Pow(2, _note / 12);
        }
    }
    // I'm not using event types that inherit from this because unity serialization
    // would erase their subtype, making it pointless. I could have serialized this
    // via JSON or something else to compensate, but performance would have been worse.
    [Serializable]
    public class MidiEventArgs
    {
        public float _delayBeforeEvent;
        public MidiEventType _type;
        public byte[] _data;
    }

    [Serializable]
    public class MidiSong
    {
        public MidiSong(ushort formatType, ushort numTracks, ushort timeDivision)
        {
            _formatType = formatType;
            _numTracks = numTracks;
            _timeDivision = timeDivision;
            _events = new List<MidiEventArgs>();
        }

        public string _name = "NONE";
        public ushort _formatType = 0;
        public ushort _numTracks = 0;
        public ushort _timeDivision = 0;

        public List<MidiEventArgs> _events;

        public void Clear() { _formatType = 0; _numTracks = 0; _timeDivision = 0; _events.Clear(); }
        public void AddEvent(MidiEventArgs e)
        {
            _events.Add(e);
        }
    }

    public class MidiPlayer
    {
        private MidiSong _midiSong;

        private enum Status
        {
            Stopped,
            Paused,
            Playing
        }

        private Status _status;
        private int _eventIndex = 0;
        private float _eventTimer = 0;
        private float _tempo = 120; // midi standard default bpm

        public delegate void MidiEventHandler_Note(MidiNote midiNote);
        public event MidiEventHandler_Note OnNoteOff;
        public event MidiEventHandler_Note OnNoteOn;

        public delegate void MidiEventHandler_NoteAftertouch(byte note, byte value);
        public event MidiEventHandler_NoteAftertouch OnNoteAftertouch;

        public delegate void MidiEventHandler_Controller(byte controllerNum, byte value);
        public event MidiEventHandler_Controller OnController;

        public delegate void MidiEventHandler_ProgramChange(byte program);
        public event MidiEventHandler_ProgramChange OnProgramChange;

        public delegate void MidiEventHandler_ChannelAftertouch(byte value);
        public event MidiEventHandler_ChannelAftertouch OnChannelAftertouch;

        // Todo[Hamid] probably won't need LSB and MSB here, should be able to consolidate it when i find out how it's supposed to work
        public delegate void MidiEventHandler_PitchBend(byte pitchLSB, byte pitchMSB);
        public event MidiEventHandler_PitchBend OnPitchBend;

        public delegate void MidiEventHandler_Meta(byte[] metaData);
        public event MidiEventHandler_Meta OnMeta;

        public delegate void MidiEventHandler_Basic();
        public event MidiEventHandler_Basic OnFinishedPlaying;

        public MidiPlayer()
        {
            OnMeta += HandleMeta;
            Stop();
        }

        public void Update(float fixedDeltaTime)
        {
            if (_status != Status.Playing)
            {
                return;
            }

            UnityEngine.Debug.Assert(_midiSong != null);
            UnityEngine.Debug.Assert(_midiSong._events.Count != 0);

            for (int i = _eventIndex; i <_midiSong._events.Count; i++)
            {
                MidiEventArgs thisEvent = _midiSong._events[i];
                byte[] eventData = thisEvent._data;

                if (_eventTimer >= thisEvent._delayBeforeEvent)
                {
                    switch (thisEvent._type)
                    {
                        case MidiEventType.NoteOn:
                            {
                                OnNoteOn?.Invoke(new MidiNote(eventData[0], eventData[1])); break;
                            }
                        case MidiEventType.NoteOff:
                            {
                                OnNoteOff?.Invoke(new MidiNote(eventData[0], eventData[1])); break;
                            }
                        case MidiEventType.NoteAftertouch:
                            {
                                OnNoteAftertouch?.Invoke(eventData[0], eventData[1]); break;
                            }
                        case MidiEventType.Controller:
                            {
                                OnController?.Invoke(eventData[0], eventData[1]); break;
                            }
                        case MidiEventType.ProgramChange:
                            {
                                OnProgramChange?.Invoke(eventData[0]); break;
                            }
                        case MidiEventType.ChannelAftertouch:
                            {
                                OnChannelAftertouch?.Invoke(eventData[0]); break;
                            }
                        case MidiEventType.PitchBend:
                            {
                                OnPitchBend?.Invoke(eventData[0], eventData[1]); break;
                            }
                        case MidiEventType.Meta:
                            {
                                OnMeta?.Invoke(eventData); break;
                            }
                        default: UnityEngine.Debug.LogAssertion("Arr: Somehow, we have a midi event of unknown type here."); break;
                    }
                    _eventIndex++;
                    _eventTimer = 0;
                }
                else
                {
                    // we want to fire off every event whose time has come, and as soon as we see ones that we need to wait on, we break out of the loop
                    break;
                }
            }


            if (_eventIndex >= _midiSong._events.Count)
            {
                Stop();
                OnFinishedPlaying?.Invoke();
            }

            _eventTimer += fixedDeltaTime * 4 * _tempo * 1.25f;
        }

        public void Rewind()
        {
            _eventTimer = 0;
            _eventIndex = 0;
        }

        public void Stop()
        {
            _status = Status.Stopped;
            Rewind();
        }

        public void Pause()
        {
            _status = Status.Paused;
        }

        public void Play()
        {
            _status = Status.Playing;
        }

        public void Restart()
        {
            Rewind();
            Play();
        }

        public void SetSong(MidiSong song)
        {
            _midiSong = song;
        }

        private void HandleMeta(byte[] data)
        {
            switch ((MidiMetaEventType)data[0])
            {
                case MidiMetaEventType.SequenceNumber:
                    break;
                case MidiMetaEventType.TextEvent:
                    break;
                case MidiMetaEventType.CopyrightNotice:
                    break;
                case MidiMetaEventType.SequenceOrTrackName:
                    break;
                case MidiMetaEventType.InstrumentName:
                    break;
                case MidiMetaEventType.Lyrics:
                    break;
                case MidiMetaEventType.Marker:
                    break;
                case MidiMetaEventType.CuePoint:
                    break;
                case MidiMetaEventType.MIDIChannelPrefix:
                    break;
                case MidiMetaEventType.EndOfTrack:
                    break;
                case MidiMetaEventType.SetTempo:
                    //byte[] mspqn = new byte[4];
                    //mspqn[3] = 0; mspqn[2] = data[1]; mspqn[1] = data[2]; mspqn[0] = data[3];
                    break;
                case MidiMetaEventType.TimeSignature:
                    break;
                case MidiMetaEventType.KeySignature:
                    break;
                default:
                    break;
            }
        }

    }

}