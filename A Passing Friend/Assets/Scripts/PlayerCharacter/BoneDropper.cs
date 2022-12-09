#region

using System.Collections;
using UnityEngine;

#endregion

public class BoneDropper : MonoBehaviour
{
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
        yield return new WaitForSeconds(0.7f);

        child.gameObject.GetComponent<Rigidbody>().useGravity = true;
    }
}