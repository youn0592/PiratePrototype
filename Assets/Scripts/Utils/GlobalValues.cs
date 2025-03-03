using UnityEngine;

public class GlobalValues : MonoBehaviour
{

    GlobalValues instance;

    public enum currentPlayer
    {
        Pirate = 0,
        Ship = 1,
        UI = 2
    }


    void Start()
    {
        if (instance == null)
            instance = this;
    }


    void Update()
    {
        
    }
}
