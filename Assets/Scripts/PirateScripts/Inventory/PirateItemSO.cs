using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PirateItemSO", menuName = "ScriptableObjects/PirateItemSO", order = 1)]
public class PirateItemSO : ScriptableObject
{
    [Header("Info")]
    [SerializeField] string itemDisplayName;
    [SerializeField] Sprite uiDisplayIcon;
    [SerializeField] GameObject PrefabObj;


}
