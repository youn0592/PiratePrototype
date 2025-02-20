using UnityEngine;

namespace Yarkin
{

    [CreateAssetMenu(fileName = "MidiAsset", menuName = "MidiAsset")]
    public class MidiAsset : ScriptableObject
    {
        [SerializeField]
        public MidiSong _midiSong;
    }

}
