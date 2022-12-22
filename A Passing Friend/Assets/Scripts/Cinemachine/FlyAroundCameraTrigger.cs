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

        public void OnTriggerEnter(Collider collisionCollider)
        {
            if (_cannotBeTriggered) return;
            _cannotBeTriggered = true;

            GetComponent<PlayableDirector>().Play();
            StartCoroutine(LockPlayerControlsForTime(_movementLockTime, collisionCollider));
        }

        private IEnumerator LockPlayerControlsForTime(float time, Collider collisionCollider)
        {
            collisionCollider.GetComponent<PlayerInput>().enabled = false;
            FindObjectOfType<LightCheckScript>().calculateLight = false;
            FindObjectOfType<LightCheckScript>().DisableLightCheckCameras();
            yield return new WaitForSeconds(time);
            collisionCollider.GetComponent<PlayerInput>().enabled = true;
            FindObjectOfType<LightCheckScript>().EnableLightCheckCameras();
            FindObjectOfType<LightCheckScript>().calculateLight = true;
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