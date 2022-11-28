using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 PlayerLocation;

    public GameData()
    {
        PlayerLocation = Vector3.zero;
    }
}
