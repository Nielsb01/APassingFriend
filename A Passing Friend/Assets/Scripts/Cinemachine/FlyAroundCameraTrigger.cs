using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class FlyAroundCameraTrigger : MonoBehaviour
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
        yield return new WaitForSeconds(time);
        collisionCollider.GetComponent<PlayerInput>().enabled = true;
    }
}