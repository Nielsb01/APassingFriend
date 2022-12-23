using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour, IDataPersistence
{
    [SerializeField] private CheckpointGameDataSO _gameData;
    private bool _isActive;

    public void Start()
    {
        _isActive = false;

        if (_gameData == null)
        {
            Debug.LogError(this.name + ": Must have a CheckpointGameDataScriptableObject, it can't be null.");
        }
        if (_gameData.name == null)
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
        return _gameData.name;
    }

    public void LoadData(GameData gameData) {}

    public void SaveData(ref GameData gameData)
    {
        if (!_isActive) return;

        gameData.activeCheckpoint = _gameData.name;
        gameData.playerLocation = transform.position;
        gameData.isDay = _gameData.isDay;
        gameData.canChargeJump = _gameData.canChargeJump;
        gameData.ashaIsAtHouse = _gameData.ashaIsAtHouse;
        gameData.questOneState = _gameData.questOneState;
        gameData.questTwoState = _gameData.questTwoState;
        _isActive = false;
    }
}
