using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace DebugToolkit
{
    public class ShowObjectOnlyInEditor : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Renderer>().enabled = false;
            var lightComp = GetComponent<Light>();
            if (lightComp != null)
            {
                lightComp.enabled = false;
            }
        }
    }
}