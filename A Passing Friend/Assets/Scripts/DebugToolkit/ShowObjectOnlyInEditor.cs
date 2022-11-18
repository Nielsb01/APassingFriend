using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace DebugToolkit
{
    [ExecuteInEditMode]
    public class ShowObjectOnlyInEditor : MonoBehaviour
    {
        private MaterialPropertyBlock _propertyBlock;
        private Renderer _renderer;

        public void OnValidate()
        {
            Renderer renderer = GetComponent<Renderer>();
            if (_propertyBlock == null)
                _propertyBlock = new MaterialPropertyBlock();
            var color = GetRandomColor();
            Debug.Log(color);

            // Assign our new value.
            _propertyBlock.SetColor("_Color", color);
            // Apply the edited values to the renderer.
            renderer.SetPropertyBlock(_propertyBlock);

            // if (_propertyBlock == null)
            //     _propertyBlock = new MaterialPropertyBlock();
            // _propertyBlock = new MaterialPropertyBlock();
            // _renderer = GetComponent<Renderer>();
            // ChangeColourDependingOnNodeActivities();
            // if (!EditorApplication.isPlaying && !Application.isPlaying) return;
            // // HideIfGameIsRunning();
        }

        private void HideIfGameIsRunning()
        {
            GetComponent<Renderer>().enabled = false;
            var lightComp = GetComponent<Light>();
            if (lightComp != null)
            {
                lightComp.enabled = false;
            }
        }

        private void ChangeColourDependingOnNodeActivities()
        {
            var color = new Color();

            var newMovementSpeed = GetComponent<Variables>().declarations.Get("NewMovementSpeed");
            var roundingForThisNode = GetComponent<Variables>().declarations.Get("RoundingForThisNode");
            var waitTimeAtThisNode = GetComponent<Variables>().declarations.Get("WaitTimeAtThisNode");
            var triggerEvent = GetComponent<Variables>().declarations.Get("TriggerEvent");
            Debug.Log(newMovementSpeed);
            Debug.Log(roundingForThisNode);
            Debug.Log(waitTimeAtThisNode);
            Debug.Log(triggerEvent);
            if (newMovementSpeed != null)
            {
                color.b += 127.5f;
            }

            if (roundingForThisNode != null)
            {
                color.b += 127.5f;
            }

            if (waitTimeAtThisNode != null)
            {
                color.g += 127.5f;
            }

            if (triggerEvent != null)
            {
                color.r += 127.5f;
            }
        }

        static Color GetRandomColor()
        {
            return Color.HSVToRGB(Random.value, 1, .9f);
        }
    }
}