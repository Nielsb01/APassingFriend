using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string _dataDirPath = string.Empty;
    private string _dataFileName = string.Empty;
    private string _fullPath = string.Empty;

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this._dataDirPath = dataDirPath;
        this._dataFileName = dataFileName;
        _fullPath = Path.Combine(_dataDirPath, _dataFileName);
    }

    public GameData Load()
    {
        GameData gameData = null;
        if (File.Exists(_fullPath))
        {
            try
            {
                var data = string.Empty;

                using (FileStream stream = new FileStream(_fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        data = reader.ReadToEnd();
                    }
                }

                gameData = JsonUtility.FromJson<GameData>(data);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        return gameData;
    }

    public void Save(GameData gameData)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_fullPath));

            var dataToStore = JsonUtility.ToJson(gameData, true);

            using (FileStream stream = new FileStream(_fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}
