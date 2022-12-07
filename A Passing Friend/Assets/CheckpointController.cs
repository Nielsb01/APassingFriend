using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour, IDataPersistence
{
    [SerializeField] private CheckpointGameDataSO _gameData;
    [SerializeField] private string _checkpointName;
    private bool _isActive = false;

    public void SetIsActiveTrue()
    {
        _isActive = true;
    }

    public void LoadData(GameData gameData) 
    {
        if (gameData.activeCheckpoint.Equals(_checkpointName))
        {
            _isActive = true;
        }
        else
        {
            _isActive = false;
        }
    }

    public void SaveData(ref GameData gameData)
    {
        if (!_isActive) return;

        gameData.activeCheckpoint = _checkpointName;
        gameData.PlayerLocation = transform.position;
        gameData.isDay = _gameData.isDay;
        gameData.canChargeJump = _gameData.canChargeJump;
        gameData.ashaLocation = _gameData.ashaLocation;
        gameData.questOneState = _gameData.questOneState;
        gameData.questTwoState = _gameData.questTwoState;
    }
}
