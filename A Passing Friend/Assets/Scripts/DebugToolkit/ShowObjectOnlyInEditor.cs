using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DebugToolkit
{
    public class ShowObjectOnlyInEditor : MonoBehaviour
    {
        [SerializeField] private Material mat1;
        [SerializeField] private Material mat2;
        [SerializeField] private Material mat3;
        [SerializeField] private Material mat4;

        public void Awake()
        {
            ChangeColourDependingOnNodeActivities();
            if (!EditorApplication.isPlaying && !Application.isPlaying) return;
            HideIfGameIsRunning();
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
            bool newMovementSpeed = ((float)GetComponent<Variables>().declarations.Get("NewMovementSpeed") > 0);
            bool roundingForThisNode = ((float)GetComponent<Variables>().declarations.Get("RoundingForThisNode") > 0);
            bool waitTimeAtThisNode = ((float)GetComponent<Variables>().declarations.Get("WaitTimeAtThisNode") > 0);
            bool triggerEvent = (bool)GetComponent<Variables>().declarations.Get("TriggerEvent");
            if (newMovementSpeed)
            {
                GetComponent<Renderer>().material.color = mat1.color;
            }

            if (roundingForThisNode)
            {
                GetComponent<Renderer>().material.color = mat2.color;
            }

            if (waitTimeAtThisNode)
            {
                GetComponent<Renderer>().material.color = mat3.color;
            }

            if (triggerEvent)
            {
                GetComponent<Renderer>().material.color = mat4.color;
            }
        }

        static Color GetRandomColor()
        {
            return Color.HSVToRGB(Random.value, 1, .9f);
        }
    }
}