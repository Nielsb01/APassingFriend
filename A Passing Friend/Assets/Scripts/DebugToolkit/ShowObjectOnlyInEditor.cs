using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DebugToolkit
{
    [ExecuteInEditMode]
    public class ShowObjectOnlyInEditor : MonoBehaviour
    {
        [SerializeField] private Material defaultNodeMaterial;
        [SerializeField] private Material triggerNodeMaterial;
        [SerializeField] private Material speedChangeNodeMaterial;
        [SerializeField] private Material waitNodeMaterial;
        [SerializeField] private Material customRoundingNodeMaterial;
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
                _renderer.material = triggerNodeMaterial;
            }
            else if ((float)GetComponent<Variables>().declarations.Get("WaitTimeAtThisNode") > 0)
            {
                _renderer.material = waitNodeMaterial;
            }
            else if ((float)GetComponent<Variables>().declarations.Get("NewMovementSpeed") > 0)
            {
                _renderer.material = speedChangeNodeMaterial;
            }
            else if ((float)GetComponent<Variables>().declarations.Get("RoundingForThisNode") > 0)
            {
                _renderer.material = customRoundingNodeMaterial;
            }
            else
            {
                _renderer.material = defaultNodeMaterial;
            }
        }
    }
}