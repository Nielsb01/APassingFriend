using UnityEngine;

namespace DebugToolkit
{
    public class ShowObjectOnlyInEditor : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Renderer>().enabled = false;
            var lightComp = GetComponent<Light>();
            var meshCollider = GetComponent<MeshCollider>();
            if (lightComp != null)
            {
                lightComp.enabled = false;
            }
            if (meshCollider != null)
            {
                meshCollider.enabled = false;
            }
        }
    }
}