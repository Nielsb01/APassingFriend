using UnityEngine;

[CreateAssetMenu(fileName = "CheckpointGameData", menuName = "ScriptableObjects/CheckpointGameDataSO", order = 1)]
public class CheckpointGameDataSO : ScriptableObject
{
    public bool isDay;
    public bool canChargeJump;
    public Vector3 ashaLocation;
    public QuestState questOneState;
    public QuestState questTwoState;
}
