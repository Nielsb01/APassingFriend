using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour, IDataPersistence
{
    [SerializeField] private CheckpointGameDataSO _gameData;
    [SerializeField] private string _checkpointName;
    private bool _isActive;

    public void Start()
    {
        _isActive = false;

        if (_gameData == null)
        {
            Debug.LogError(this.name + ": Must have a CheckpointGameDataScriptableObject, it can't be null.");
        }
        if (_checkpointName == null)
        {
            Debug.LogError(this.name + ": Must have a unique Name set, it can't be null.");
        }
    }

    public void SetIsActiveTrue()
    {
        _isActive = true;
    }

    public string GetCheckpointName()
    {
        return _checkpointName;
    }

    public void LoadData(GameData gameData) {}

    public void SaveData(ref GameData gameData)
    {
        if (!_isActive) return;

        gameData.activeCheckpoint = _checkpointName;
        gameData.playerLocation = transform.position;
        gameData.isDay = _gameData.isDay;
        gameData.canChargeJump = _gameData.canChargeJump;
        gameData.ashaLocation = _gameData.ashaLocation;
        gameData.questOneState = _gameData.questOneState;
        gameData.questTwoState = _gameData.questTwoState;
        _isActive = false;
    }
}
