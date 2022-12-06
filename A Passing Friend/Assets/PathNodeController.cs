using System;
using System.Collections;
using System.Collections.Generic;
using Npc;
using UnityEngine;

public class PathNodeController : MonoBehaviour
{
    [SerializeField] private float NewMovementSpeed { get; }
    [SerializeField] private float RoundingForThisNode { get; }
    [SerializeField] private float WaitTimeAtThisNode { get; }
    [SerializeField] private float TriggerEvent { get; }
    [SerializeField] private List<TriggerScript> _triggerScripts;

    public void Trigger()
    {
        if (_triggerScripts.Count == 0)
        {
            throw new Exception("A trigger node needs one or more trigger scripts.");
        }

        _triggerScripts.ForEach(script => script.ExecuteTrigger());
    }
    
}