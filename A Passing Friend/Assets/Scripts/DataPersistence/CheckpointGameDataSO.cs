using UnityEngine;

[CreateAssetMenu(fileName = "CheckpointGameData", menuName = "ScriptableObjects/CheckpointGameDataSO", order = 1)]
public class CheckpointGameDataSO : ScriptableObject
{
    public string checkpointName;
    public bool isDay;
    public bool canChargeJump;
    public bool ashaCutsceneComplete;
    public QuestState questOneState;
    public QuestState questTwoState;
    public bool hasBeenEagle;
}
