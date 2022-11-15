using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Npc
{
    public class ShowObjectOnlyInEditor : MonoBehaviour
    {
        [ExecuteInEditMode]
        public void Start()
        {
            if (!EditorApplication.isPlaying && !Application.isPlaying) return;
        
            GetComponent<Renderer>().enabled = false;
            var lightComp = GetComponent<Light>();
            if (lightComp != null)
            {
                lightComp.enabled = false;
            }
        }
    }
}