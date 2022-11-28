using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace DebugToolkit
{
    [ExecuteInEditMode]
    public class ShowObjectOnlyInEditor : MonoBehaviour
    {
        [SerializeField] private Material _defaultNodeMaterial;
        [SerializeField] private Material _triggerNodeMaterial;
        [SerializeField] private Material _speedChangeNodeMaterial;
        [SerializeField] private Material _waitNodeMaterial;
        [SerializeField] private Material _customRoundingNodeMaterial;
        private Renderer _renderer;

        public void OnValidate()
        {
            _renderer = GetComponent<Renderer>();
            ChangeColourDependingOnNodeActivities();
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

        private void ChangeColourDependingOnNodeActivities()
        {
            if ((bool)GetComponent<Variables>().declarations.Get("TriggerEvent"))
            {
                _renderer.material = _triggerNodeMaterial;
            }
            else if ((float)GetComponent<Variables>().declarations.Get("WaitTimeAtThisNode") > 0)
            {
                _renderer.material = _waitNodeMaterial;
            }
            else if ((float)GetComponent<Variables>().declarations.Get("NewMovementSpeed") > 0)
            {
                _renderer.material = _speedChangeNodeMaterial;
            }
            else if ((float)GetComponent<Variables>().declarations.Get("RoundingForThisNode") > 0)
            {
                _renderer.material = _customRoundingNodeMaterial;
            }
            else
            {
                _renderer.material = _defaultNodeMaterial;
            }
        }
    }
}