using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum SoundState : ushort
{
    UNDEFINED = 0,
    FORREST = 1,
    VILLAGE_DAY = 2,
    VILLAGE_NIGHT = 3,
    TREE_VIEW = 4
}

public class SoundController : MonoBehaviour, IDataPersistence
{
    [Header("Geopositional Settings")] [SerializeField]
    private GameObject _player;

    [SerializeField] private Collider _forrestBoundaries;
    [SerializeField] private Collider _villageBoundaries;
    [SerializeField] private Collider _treeBoundaries;

    [Header("Sound Events")] [SerializeField]
    private FMODUnity.EventReference _forrestMusicEventPath;

    [SerializeField] private FMODUnity.EventReference _villageDayEventPath;
    [SerializeField] private FMODUnity.EventReference _villageNightEventPath;
    [SerializeField] private FMODUnity.EventReference _treeEventPath;

    [Header("Sound Objects")] [SerializeField]
    private List<GameObject> _environmentObjects;

    [SerializeField] private List<GameObject> _environmentDayObjects;
    [SerializeField] private List<GameObject> _environmentNightObjects;

    // Member variables for keeping track of the game state
    private bool _isDay = true;
    private SoundState _state = SoundState.UNDEFINED;
    private bool _playerEnteredTreeViewPlatform = false;
    private bool _treeMusicPlayed = false;
    private bool _playerInTree = false;


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

        foreach (var obj in _environmentDayObjects)
        {
            obj.SetActive(false);
        }

        foreach (var obj in _environmentNightObjects)
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

            if (newState == SoundState.VILLAGE_NIGHT)
            {
                foreach (var obj in _environmentNightObjects)
                {
                    obj.SetActive(true);
                }
            }
            else
            {
                foreach (var obj in _environmentDayObjects)
                {
                    obj.SetActive(true);
                }
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

            if ((_treeMusicPlayed == false) && _playerInTree)
            {
                // Player in tree
                newState = SoundState.TREE_VIEW;
                _playerEnteredTreeViewPlatform = true;
            }
            else if ((_treeMusicPlayed == false) && (_playerInTree == false) && (_playerEnteredTreeViewPlatform))
            {
                // Wait for player to leave the tree view spot before changing music
                _treeMusicPlayed = true;
            }
        }

        return newState;
    }

    public void SetPlayerInTree(bool state)
    {
        _playerInTree = state;
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
            case SoundState.TREE_VIEW:
            {
                FMODUnity.RuntimeManager.PlayOneShot(_treeEventPath);
                break;
            }
        }
    }
}