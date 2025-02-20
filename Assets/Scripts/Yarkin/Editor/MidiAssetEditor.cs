using UnityEditor;
using UnityEngine;

namespace Yarkin
{
    [CustomEditor(typeof(MidiAsset))]
    public class MidiAssetEditor : Editor
    {
        DefaultAsset _TargetMidiFile;

        public override void OnInspectorGUI()
        {
            MidiAsset targetScriptableObject = (MidiAsset)target;

            {
                string loadedFileName = "NONE";
                if (targetScriptableObject != null && targetScriptableObject._midiSong != null && targetScriptableObject._midiSong._name != null && targetScriptableObject._midiSong._name.Length != 0)
                {
                    loadedFileName = targetScriptableObject._midiSong._name;
                }

                EditorGUILayout.LabelField("Loaded File: " + loadedFileName, EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                _TargetMidiFile = (DefaultAsset)EditorGUILayout.ObjectField("Midi File To Process", _TargetMidiFile, typeof(DefaultAsset), false);
                if (GUILayout.Button("Load"))
                {
                    TryReadAssetAsMidi(_TargetMidiFile);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        private bool TryReadAssetAsMidi(DefaultAsset asset)
        {
            if (asset == null)
            {
                Debug.LogError("Yarr: No Midi file specified to be read!");
                return false;
            }

            string assetPath = AssetDatabase.GetAssetPath(asset);
            if (!assetPath.EndsWith(".mid"))
            {
                Debug.LogError("Yarr: Specified file isn't .mid format!");
                return false;
            }

            MidiAsset targetScriptableObject = (MidiAsset)target;

            bool success = TryLoadMidiData(assetPath, ref targetScriptableObject._midiSong);

            if (success)
            {
                targetScriptableObject._midiSong._name = asset.name;
                EditorUtility.SetDirty(targetScriptableObject);
            }

            return success;
        }

        private bool TryLoadMidiData(string assetPath, ref MidiSong midiSong)
        {
            byte[] midiData = System.IO.File.ReadAllBytes(assetPath);
            return MidiParser.TryParse(midiData, ref midiSong);
        }
    }
}
