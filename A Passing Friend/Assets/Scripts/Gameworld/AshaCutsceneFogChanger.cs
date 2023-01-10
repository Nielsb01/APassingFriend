using Cinemachine;
using UnityEngine;
using CameraState = Camera.CameraState;

public class AshaCutsceneFogChanger : MonoBehaviour
{
    [SerializeField] private HealthController _player;
    private float _fogDensity;
    private bool _active;
    private CinemachineVirtualCamera _cam;

    private void Start()
    {
        _cam = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (_active)
        {
            if (_cam.Priority == (int)CameraState.Inactive)
            {
                ExecuteTrigger();
                _player.SetPlayerInvisible(false);
                Destroy(this);
            }
        }
        else if (_cam.Priority == (int)CameraState.Active)
        {
            _active = true;
            ExecuteTrigger();
        }
    }

    private void ExecuteTrigger()
    {
        if (RenderSettings.fogDensity != 0)
        {
            _fogDensity = RenderSettings.fogDensity;
            RenderSettings.fogDensity = 0;
        }
        else
        {
            RenderSettings.fogDensity = _fogDensity;
        }
    }
}