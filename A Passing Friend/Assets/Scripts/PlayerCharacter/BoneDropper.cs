#region

using System.Collections;
using UnityEngine;

#endregion

public class BoneDropper : MonoBehaviour
{
    [SerializeField] private float _secondsBeforeDroppingBones = 0.7f;

    private void Awake()
    {
        DropBones(transform.GetChild(1));
    }

    private void DropBones(Transform parent)
    {
        foreach (Transform child in parent)
        {
            StartCoroutine(DropBone(child));
        }
    }

    private IEnumerator DropBone(Transform child)
    {
        yield return new WaitForSeconds(_secondsBeforeDroppingBones);

        child.gameObject.GetComponent<Rigidbody>().useGravity = true;
    }
}