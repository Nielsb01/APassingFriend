using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    [SerializeField] private string _filename;
    [SerializeField] private string _dirPath = "C:\\temp";
    [SerializeField] private List<CheckpointController> _checkpoints;
    public static DataPersistenceManager instance { get; private set; }

    private GameData _gameData;
    private List<IDataPersistence> _dataPersistenceObjects;
    private FileDataHandler _fileDataHandler;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError(this.name + "More than one DPM found in scene");
        }
        instance = this;
    }

    private void Start()
    {
        _fileDataHandler = new FileDataHandler(_dirPath, _filename);
        _dataPersistenceObjects = GetAllDataPersistenceObjects();
        LoadGame();
    }

    private List<IDataPersistence> GetAllDataPersistenceObjects()
    {
        var result = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(result);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void NewGame()
    {
        _gameData = new GameData();
        if (_checkpoints.Count < 1 || _checkpoints[0] == null)
        {
            Debug.LogError(this.name + ": Checkpoints list must have at least one CheckPointFlag assigned to use as spawn point.");
        }
        else
        {
            SetCheckpoint(0);
        }
    }

    public void SaveGame()
    {
        foreach (var obj in _dataPersistenceObjects)
        {
            obj.SaveData(ref _gameData);
        }

        _fileDataHandler.Save(_gameData);
    }

    public void LoadGame()
    {
        _gameData = _fileDataHandler.Load();

        if (_gameData == null)
        {
            NewGame();
        }

        foreach (var obj in _dataPersistenceObjects)
        {
            obj.LoadData(_gameData);
        }
    }

    public void NextCheckpoint(int nextCheckpoint)
    {
        if (nextCheckpoint <= 0)
        {
            Debug.LogError("The chosen index needs to be higher than 0, nextCheckpointIndex cannot be 0.");
            return;
        }

        var nameActive = _gameData.activeCheckpoint;

        if (_checkpoints[nextCheckpoint-1].GetComponent<CheckpointController>().GetCheckpointName().Equals(nameActive))
        {
            SetCheckpoint(nextCheckpoint);
        }
    }

    private void SetCheckpoint(int index)
    {
        var checkpoint = _checkpoints[index].GetComponent<CheckpointController>();
        checkpoint.SetIsActiveTrue();
        SaveGame();
        LoadGame();
    }
}
