using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 PlayerLocation;

    public GameData()
    {
        PlayerLocation = new Vector3(0,0,10);
    }
}
