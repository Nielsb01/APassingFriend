using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Day night shizzle")]
    [SerializeField] private DayNightToggler _dayNightToggler;


    public void LoadData(GameData gameData)
    {
        LoadBackgroundMusic();
    }

    public void SaveData(ref GameData gameData)
    {
        // throw new System.NotImplementedException();
    }

    private void LoadBackgroundMusic()
    {
        if (_forrestBoundaries.bounds.Contains(_player.transform.position))
        {
            // Player in forrest
            FMODUnity.RuntimeManager.PlayOneShot(_forrestMusicEventPath);
        }
        else if (_villageBoundaries.bounds.Contains(_player.transform.position))
        {          
            // Player in village
            if (_dayNightToggler.IsDay)
            {
                // Day
                FMODUnity.RuntimeManager.PlayOneShot(_villageDayEventPath);
            }
            else
            {
                // Night
                FMODUnity.RuntimeManager.PlayOneShot(_villageNightEventPath);
            }

        }
    }

}
