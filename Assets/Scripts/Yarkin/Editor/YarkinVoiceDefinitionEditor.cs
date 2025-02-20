using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(YarkinVoiceDefinition))]
public class YarkinVoiceDefinitionEditor : Editor
{
    const string _voiceDirectoryRootPath = "Assets\\";
    string _subPath = "Assets\\Audio\\Yarkin\\Media\\";
    SerializedProperty _audioClipArrayProperty;

    public override void OnInspectorGUI()
    {
        YarkinVoiceDefinition targetScriptableObject = (YarkinVoiceDefinition)target;

        // Draw the pitch range
        {
            GUILayout.Label("Mix/Max Random Pitch Range: " + targetScriptableObject._pitchMinMax.x + " - " + targetScriptableObject._pitchMinMax.y);
            EditorGUILayout.MinMaxSlider(/*min*/ ref targetScriptableObject._pitchMinMax.x, /*max*/ ref targetScriptableObject._pitchMinMax.y, /*minLimit*/ 0.25f, /*maxLimit*/2.5f);
        }

        // Draw the clip array
        {
            serializedObject.Update();
            _audioClipArrayProperty = serializedObject.FindProperty("_audioClips");
            EditorGUILayout.PropertyField(_audioClipArrayProperty, new GUIContent("Yarkin Clips"), true);
        }

        // Automatic populating of the clips from a directory
        {
            // Draw the directory label/text box
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(_voiceDirectoryRootPath, GUILayout.ExpandWidth(false));
                _subPath = GUILayout.TextField(_subPath);
                GUILayout.EndHorizontal();
            }

            // Load button
            if (GUILayout.Button("Load From Directory"))
            {
                string fullPath = _voiceDirectoryRootPath + _subPath;
                if (Directory.Exists(fullPath))
                {
                    string[] audioAssetPaths = Directory.GetFiles(fullPath);
                    List<string> cleanedAudioAssetPaths = new List<string>();

                    foreach (string path in audioAssetPaths)
                    {
                        if (Path.GetExtension(path) != ".meta")
                        {
                            cleanedAudioAssetPaths.Add(path);
                        }
                    }

                    if (cleanedAudioAssetPaths.Count == 0)
                    {
                        Debug.LogError("Requested directory doesn't contain any assets! (" + fullPath + ")");
                    }
                    else
                    {
                        targetScriptableObject.SetAudioClips(cleanedAudioAssetPaths.ToArray());
                        EditorUtility.SetDirty(targetScriptableObject);
                    }
                }
                else
                {
                    Debug.LogError("Requested directory doesn't exist! (" + fullPath + ")");
                }
            }
        }
    }
}