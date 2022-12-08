using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 PlayerLocation;
    public bool isDay;
    public bool canChargeJump;
    public Vector3 ashaLocation;
    public QuestState questOneState;
    public QuestState questTwoState;
    public string activeCheckpoint;

    public GameData()
    {
        PlayerLocation = Vector3.zero;
        isDay = true;
        canChargeJump = false;
        ashaLocation = Vector3.zero;
        questOneState = QuestState.Unavailable;
        questTwoState = QuestState.Unavailable;
        activeCheckpoint = "";
    }
}
