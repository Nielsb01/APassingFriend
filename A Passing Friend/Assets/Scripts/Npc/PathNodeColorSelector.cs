using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathNodeColorSelector : MonoBehaviour
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