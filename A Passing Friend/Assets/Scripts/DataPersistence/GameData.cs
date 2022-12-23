using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 playerLocation;
    public string ItemHeldByPlayer;
    public bool isDay;
    public bool canChargeJump;
    public bool ashaIsAtHouse;
    public QuestState questOneState;
    public QuestState questTwoState;
    public string activeCheckpoint;
    public float fogDensity;
    public bool hasBeenEagle;

    public GameData()
    {
        playerLocation = Vector3.zero;
        isDay = true;
        canChargeJump = false;
        ashaIsAtHouse = false;
        questOneState = QuestState.Unavailable;
        questTwoState = QuestState.Unavailable;
        activeCheckpoint = "";
    }
}
