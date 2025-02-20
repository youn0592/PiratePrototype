using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "YarkinVoiceDefinition", menuName = "YarkinVoiceDefinition")]
public class YarkinVoiceDefinition : ScriptableObject
{
    [SerializeField]
    public float2 _pitchMinMax = new float2(0.9f, 1.1f);

    [SerializeReference]
    AudioClip[] _audioClips;

    public int GetAudioClipsCount()
    {
        return _audioClips.Length;
    }
    public AudioClip[] GetAudioClips()
    {
        return _audioClips;
    }
    public void SetAudioClips(string[] paths)
    {
        List<AudioClip> loadedAudioClips = new List<AudioClip>();
        for (int i = 0; i < paths.Length; i++)
        {
            AudioClip loadedClip = (AudioClip)AssetDatabase.LoadAssetAtPath(paths[i], typeof(AudioClip));
            if (loadedClip != null)
            {
                loadedAudioClips.Add(loadedClip);
            }
            else
            {
                Debug.LogError("File is not a valid AudioClip! (" + paths[i] + ")");
            }
        }

        _audioClips = loadedAudioClips.ToArray();
    }
}
