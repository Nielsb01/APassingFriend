using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 playerLocation;
    public string ItemHeldByPlayer;
    public bool isDay;
    public bool canChargeJump;
    public Vector3 ashaLocation;
    public QuestState questOneState;
    public QuestState questTwoState;
    public string activeCheckpoint;
    public float fogDensity;
    
    public GameData()
    {
        playerLocation = Vector3.zero;
        isDay = true;
        canChargeJump = false;
        ashaLocation = Vector3.zero;
        questOneState = QuestState.Unavailable;
        questTwoState = QuestState.Unavailable;
        activeCheckpoint = "";
    }
}
