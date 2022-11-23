using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    [SerializeField] private string _filename;
    public static DataPersistenceManager instance { get; private set; }

    private GameData _gameData;
    private List<IDataPersistence> _dataPersistenceOpjects;
    private FileDataHandler _fileDataHandler;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one DPM found in scene");
        }
        instance = this;
    }

    private void Start()
    {
        this._fileDataHandler = new FileDataHandler("D:\\", _filename);
        this._dataPersistenceOpjects = GetAllDataPersistenceObjects();
        LoadGame();
    }

    private List<IDataPersistence> GetAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> result = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(result);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void NewGame()
    {
        this._gameData = new GameData();
    }

    public void SaveGame()
    {
        foreach (var obj in _dataPersistenceOpjects)
        {
            obj.SaveData(ref _gameData);
        }

        _fileDataHandler.Save(_gameData);
    }

    public void LoadGame()
    {
        this._gameData = _fileDataHandler.Load();

        if (_gameData == null)
        {
            NewGame();
        }

        foreach (var obj in _dataPersistenceOpjects)
        {
            obj.LoadData(_gameData);
        }
    }
}
