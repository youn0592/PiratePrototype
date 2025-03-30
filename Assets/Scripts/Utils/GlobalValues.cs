using UnityEngine;

public class GlobalValues : MonoBehaviour
{

    public static GlobalValues instance { get; private set; }

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

}
