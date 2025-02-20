using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Using this post as parse guide
// https://github.com/colxi/midi-parser-js/wiki/MIDI-File-Format-Specifications

namespace Yarkin
{

public class MidiParser
{
    class Utils
    {
        public static uint BigEndianReadFourBytes(BinaryReader reader)
        {
            byte[] readBytes = reader.ReadBytes(4);

            if (readBytes.Length < 4)
                throw new EndOfStreamException("Arr: Not enough bytes to read a UInt32.");

            return BinaryPrimitives.ReadUInt32BigEndian(readBytes);
        }

        public static ushort BigEndianReadTwoBytes(BinaryReader reader)
        {
            byte[] readBytes = reader.ReadBytes(2);

            if (readBytes.Length < 2)
                throw new EndOfStreamException("Arr: Not enough bytes to read a UInt16.");

            return BinaryPrimitives.ReadUInt16BigEndian(readBytes);
        }

        public static int ReadDeltaTime(BinaryReader reader)
        {
            int deltaTime = 0;
            byte readByte = 0;

            do
            {
                readByte = reader.ReadByte();
                deltaTime = (deltaTime << 7) | readByte & 0x7F;
            } while ((readByte & 0x80) != 0); // the leftmost bit is 1 if there's more bytes to read and tally up

            return deltaTime;
        }

        public static bool IsAValidMidiEvent(byte midiEvent)
        {
            switch ((MidiEventType)midiEvent)
            {
                case MidiEventType.NoteOff:
                case MidiEventType.NoteOn:
                case MidiEventType.NoteAftertouch:
                case MidiEventType.Controller:
                case MidiEventType.ProgramChange:
                case MidiEventType.ChannelAftertouch:
                case MidiEventType.PitchBend:
                case MidiEventType.Meta:
                    return true;
                default: return false;
            };
        }
    };

    private enum TimeDivisionType
    {
        TicksPerBeat,   // from around 48 to 960 - number of 'clock ticks' or 'track delta positions' in every quarter note of music
        FramesPerSecond // has a frame per second and a tick-per-frame number encoded. avoiding this if possible.
    }

    public static bool TryParse(byte[] midiData, ref MidiSong midiSong)
    {
        using var memStream = new MemoryStream(midiData, false);
        var reader = new BinaryReader(memStream);

        // Read the 4-byte header, should always be "MThd"
        char[] midiHeader = reader.ReadChars(4);

        if (midiHeader[0] != 'M' || midiHeader[1] != 'T' || midiHeader[2] != 'h' || midiHeader[3] != 'd' || Utils.BigEndianReadFourBytes(reader) != 6) // if the header is NOT MThd or its length is more than 6 - the file is corrupt or invalid.
        {
            Debug.LogError("Arr: the midi file has an unexpected header.");
            return false;
        }

        ushort formatType = Utils.BigEndianReadTwoBytes(reader);
        ushort numTracks = Utils.BigEndianReadTwoBytes(reader);
        ushort timeDivision = Utils.BigEndianReadTwoBytes(reader);

        TimeDivisionType timeDivisionType = (timeDivision & 0x8000) == 0 ? TimeDivisionType.TicksPerBeat : TimeDivisionType.FramesPerSecond;

        if (timeDivisionType == TimeDivisionType.FramesPerSecond)
        {
            Debug.LogError("Arr: What the heck is even that");
            return false;
        }

        char[] trackChunkHeader = reader.ReadChars(4);
        if (trackChunkHeader[0] != 'M' || trackChunkHeader[1] != 'T' || trackChunkHeader[2] != 'r' || trackChunkHeader[3] != 'k') // if the track header is NOT MTrk something's gone wrong.
        {
            Debug.LogError("Arr: the midi file has an unexpected track header.");
            return false;
        }

        uint trackChunkSize = Utils.BigEndianReadFourBytes(reader);

        MidiSong outsong = new MidiSong(formatType, numTracks, timeDivision);

        byte lastEventType = 0;

        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            int dt = Utils.ReadDeltaTime(reader);

            Debug.Log("wait " + dt);

            byte eventType = reader.ReadByte();

            // Apparently midi files can omit event type if it's the same event. so if the read value is smaller than 0x80 we know it's omitted the event type.
            if (eventType < 0x80)
            {
                // use previous event type
                reader.BaseStream.Position--; // Step back for simplicity
                eventType = lastEventType;
            }
            else
            {
                lastEventType = eventType; // Store this event type in case it's omitted
            }

            byte midiEventUpperNibble = (byte)(eventType & 0xF0);
            if (!Utils.IsAValidMidiEvent(midiEventUpperNibble))
            {
                Debug.LogError("Arr, something went wrong while deciphering midi events.");
                return false;
            }

            MidiEventType midiEvent = (MidiEventType)(midiEventUpperNibble);
            Debug.Log("Arr: Event - " + midiEvent);

            switch (midiEvent)
            {
                case MidiEventType.NoteOn:
                    {
                        // note - velocity
                        outsong.AddEvent(new MidiEventArgs { _type = MidiEventType.NoteOn, _data = reader.ReadBytes(2), _delayBeforeEvent = dt });
                        break;
                    }

                case MidiEventType.NoteOff:
                    {
                        // note - velocity
                        outsong.AddEvent(new MidiEventArgs { _type = MidiEventType.NoteOff, _data = reader.ReadBytes(2), _delayBeforeEvent = dt });
                        break;
                    }

                case MidiEventType.NoteAftertouch:
                    {
                        // note - value
                        outsong.AddEvent(new MidiEventArgs { _type = MidiEventType.NoteAftertouch, _data = reader.ReadBytes(2), _delayBeforeEvent = dt });
                        break;
                    }

                case MidiEventType.Controller:
                    {
                        // num - value
                        outsong.AddEvent(new MidiEventArgs { _type = MidiEventType.Controller, _data = reader.ReadBytes(2), _delayBeforeEvent = dt });
                        break;
                    }

                case MidiEventType.ProgramChange:
                    {
                        // program
                        outsong.AddEvent(new MidiEventArgs { _type = MidiEventType.ProgramChange, _data = reader.ReadBytes(1), _delayBeforeEvent = dt });
                        break;
                    }

                case MidiEventType.ChannelAftertouch:
                    {
                        // value
                        outsong.AddEvent(new MidiEventArgs { _type = MidiEventType.ChannelAftertouch, _data = reader.ReadBytes(1), _delayBeforeEvent = dt });
                        break;
                    }

                case MidiEventType.PitchBend:
                    {
                        // pitchLSB - pitchMSB
                        outsong.AddEvent(new MidiEventArgs { _type = MidiEventType.Controller, _data = reader.ReadBytes(2), _delayBeforeEvent = dt });
                        break;
                    }

                case MidiEventType.Meta:
                    {
                        Debug.Log("Arr: What the heck is a Meta event. Never Meta the guy.");
                        List<byte> readData = new List<byte>();

                        // metaType (one byte) - data (varying number of bytes)
                        readData.Add(reader.ReadByte());
                        byte metaLength = reader.ReadByte();
                        byte[] metaData = reader.ReadBytes(metaLength);
                        readData.AddRange(metaData);

                        outsong.AddEvent(new MidiEventArgs { _type = MidiEventType.Meta, _data = readData.ToArray(), _delayBeforeEvent = dt });
                        break;
                    }
            }
        }

        midiSong = outsong;
        return true;
    }
}

}