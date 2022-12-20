using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum SoundState : ushort
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

    [Header("Sound Objects")]
    [SerializeField] private List<GameObject> _environmentObjects;

    // Member variables for keeping track of the game state
    private bool _isDay = true;
    private SoundState _state = SoundState.UNDEFINED;


    public void Awake()
    {
        StartCoroutine(OnUpdateBackgroundMusic());
    }


    public void LoadData(GameData gameData)
    {
        StopAllSounds();

        // Update all member variables
        _state = SoundState.UNDEFINED;
        _isDay = gameData.isDay;
    }

    public void SaveData(ref GameData gameData)
    {
    }

    private void StopAllSounds()
    {
        foreach (var obj in _environmentObjects)
        {
            obj.SetActive(false);
        }

        var mainBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        mainBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private IEnumerator OnUpdateBackgroundMusic()
    {
        SoundState newState = GetNewState();

        if (_state != newState)
        {
            StopAllSounds();

            // Start the background music
            LoadBackgroundMusic(newState);

            // Enable all other sound objects
            foreach (var obj in _environmentObjects)
            {
                obj.SetActive(true);
            }

            _state = newState;
        }

        const float delay = 0.1f;

        yield return new WaitForSeconds(delay);

        StartCoroutine(OnUpdateBackgroundMusic());

        yield return null;
    }

    private SoundState GetNewState()
    {
        SoundState newState = SoundState.UNDEFINED;

        if (_forrestBoundaries.bounds.Contains(_player.transform.position))
        {
            // Player in forrest
            newState = SoundState.FORREST;
        }
        else if (_villageBoundaries.bounds.Contains(_player.transform.position))
        {
            // Player in village
            if (_isDay)
            {
                // Day
                newState = SoundState.VILLAGE_DAY;
            }
            else
            {
                // Night
                newState = SoundState.VILLAGE_NIGHT;
            }
        }

        return newState;
    }

    private void LoadBackgroundMusic(SoundState newState)
    {
        switch (newState)
        {
            case SoundState.FORREST:
                {
                    FMODUnity.RuntimeManager.PlayOneShot(_forrestMusicEventPath);
                    break;
                }
            case SoundState.VILLAGE_DAY:
                {
                    FMODUnity.RuntimeManager.PlayOneShot(_villageDayEventPath);
                    break;
                }
            case SoundState.VILLAGE_NIGHT:
                {
                    FMODUnity.RuntimeManager.PlayOneShot(_villageNightEventPath);
                    break;
                }
        }
    }

}
