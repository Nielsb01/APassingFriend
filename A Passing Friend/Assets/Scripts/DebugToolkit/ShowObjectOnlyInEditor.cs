using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace DebugToolkit
{
    [ExecuteInEditMode]
    public class ShowObjectOnlyInEditor : MonoBehaviour
    {
        private Renderer _renderer;

        public void OnValidate()
        {
            _renderer = GetComponent<Renderer>();
            if (!EditorApplication.isPlaying && !Application.isPlaying) return;
            HideIfGameIsRunning();
        }

        private void HideIfGameIsRunning()
        {
            _renderer.enabled = false;
            var lightComp = GetComponent<Light>();
            if (lightComp != null)
            {
                lightComp.enabled = false;
            }
        }
    }
}