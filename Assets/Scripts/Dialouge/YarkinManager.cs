using System.Collections.Generic;
using Ink.Parsed;
using UnityEngine;

public class YarkinManager : MonoBehaviour
{
    public static YarkinManager instance { get; private set; }


    [Header("VoiceDefiniation")]
    [SerializeField] List<YarkinVoiceDefinition> pirateVoices;

    Dictionary<string, YarkinVoiceDefinition> yarkinMap;

    private void Awake()
    {
        if (instance != null) Debug.LogError("Multiple instances of YarkinManager Exist");
        instance = this;

        yarkinMap = CreateYarkinMap();
    }

    private Dictionary<string, YarkinVoiceDefinition> CreateYarkinMap()
    {
        Dictionary<string, YarkinVoiceDefinition> idToYarkMap = new Dictionary<string, YarkinVoiceDefinition>();
        foreach (YarkinVoiceDefinition voice in pirateVoices)
        {
            if(idToYarkMap.ContainsKey(voice.name))
            {
                Debug.LogWarning("Duplicate ID found when making map: " + voice.name);
            }
            idToYarkMap.Add(voice.name, voice);
        }

        return idToYarkMap;
    }

    public YarkinVoiceDefinition GetVoiceDefinition(string yarkinKey)
    {
        YarkinVoiceDefinition yarkin = yarkinMap[yarkinKey];

        if(yarkin == null)
        {
            Debug.LogError("ID not found in the Yarkin Map: " + yarkinKey);
        }

        return yarkin;
    }

}
