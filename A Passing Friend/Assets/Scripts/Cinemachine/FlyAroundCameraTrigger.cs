using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

namespace Camera
{
    public class FlyAroundCameraTrigger : MonoBehaviour, IDataPersistence
    {
        private bool _cannotBeTriggered;
        [SerializeField] private float _movementLockTime = 1;
        private const int NEXT_CHECKPOINT = 2;

        public void OnTriggerEnter(Collider collisionCollider)
        {
            if (_cannotBeTriggered) return;
            _cannotBeTriggered = true;

            GetComponent<PlayableDirector>().Play();
            StartCoroutine(LockPlayerControlsForTime(_movementLockTime, collisionCollider));
        }

        private IEnumerator LockPlayerControlsForTime(float time, Collider collisionCollider)
        {
            var soundController = FindObjectOfType<SoundController>();
            var lightCheckScript = FindObjectOfType<LightCheckScript>();

            FindObjectOfType<DataPersistenceManager>().NextCheckpoint(NEXT_CHECKPOINT);

            soundController.SetPlayerInTree(true);
            var fogDensity = RenderSettings.fogDensity;
            yield return new WaitForSeconds(0.5f);
            RenderSettings.fogDensity = 0;

            PlayerFreezer.FreezeAllInput();
            lightCheckScript.calculateLight = false;
            lightCheckScript.DisableLightCheckCameras();


            yield return new WaitForSeconds(time);


            soundController.SetPlayerInTree(false);
            RenderSettings.fogDensity = fogDensity;

            PlayerFreezer.ReleaseAllInputFreeze();
            lightCheckScript.EnableLightCheckCameras();
            lightCheckScript.calculateLight = true;
        }

        public void LoadData(GameData gameData)
        {
            _cannotBeTriggered = gameData.hasBeenEagle;
        }

        public void SaveData(ref GameData gameData)
        {
            gameData.hasBeenEagle = _cannotBeTriggered;
        }
    }
}