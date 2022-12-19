using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BackgroundMusicState : ushort
{
    UNDEFINED = 0,
    FORREST = 1,
    VILLAGE_DAY = 2,
    VILLAGE_NIGHT = 3
}

public class SoundController : MonoBehaviour, IDataPersistence
{
    [Header("Geopositional Settings")]
    [SerializeField] private GameObject _player;
    [SerializeField] private Collider _forrestBoundaries;
    [SerializeField] private Collider _villageBoundaries;
    
    [Header("Sound Events")]
    [SerializeField] private FMODUnity.EventReference _forrestMusicEventPath;
    [SerializeField] private FMODUnity.EventReference _villageDayEventPath;
    [SerializeField] private FMODUnity.EventReference _villageNightEventPath;

    private bool _isDay;


    private BackgroundMusicState _state;

    public void Awake()
    {
        OnUpdateBackgroundMusic();
    }


    public void LoadData(GameData gameData)
    {
        _isDay = gameData.isDay;
        StopAllSounds();
        OnUpdateBackgroundMusic();

        StartCoroutine(OnUpdateBackgroundMusic());
    }

    public void SaveData(ref GameData gameData)
    {
    }

    private void StopAllSounds()
    {
        var mainBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        mainBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private IEnumerator OnUpdateBackgroundMusic()
    {
        BackgroundMusicState newState = GetNewState();

        if (_state != newState)
        {
            StopAllSounds();
            LoadBackgroundMusic(newState);
            _state = newState;
        }


        float delay = 0.1f;
        yield return new WaitForSeconds(delay);

        StartCoroutine(OnUpdateBackgroundMusic());

        yield return null;
    }

    private BackgroundMusicState GetNewState()
    {
        BackgroundMusicState newState = BackgroundMusicState.UNDEFINED;
        if (_forrestBoundaries.bounds.Contains(_player.transform.position))
        {
            // Player in forrest
            newState = BackgroundMusicState.FORREST;
        }
        else if (_villageBoundaries.bounds.Contains(_player.transform.position))
        {
            // Player in village
            if (_isDay)
            {
                // Day
                newState = BackgroundMusicState.VILLAGE_DAY;
            }
            else
            {
                // Night
                newState = BackgroundMusicState.VILLAGE_NIGHT;
            }
        }

        return newState;
    }

    private void LoadBackgroundMusic(BackgroundMusicState newState)
    {
        switch (newState)
        {
            case BackgroundMusicState.FORREST:
                {
                    FMODUnity.RuntimeManager.PlayOneShot(_forrestMusicEventPath);
                    break;
                }
            case BackgroundMusicState.VILLAGE_DAY:
                {
                    FMODUnity.RuntimeManager.PlayOneShot(_villageDayEventPath);
                    break;
                }
            case BackgroundMusicState.VILLAGE_NIGHT:
                {
                    FMODUnity.RuntimeManager.PlayOneShot(_villageNightEventPath);
                    break;
                }
        }
    }

}
