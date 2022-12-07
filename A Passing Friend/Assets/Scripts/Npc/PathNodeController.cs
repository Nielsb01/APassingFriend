using System;
using System.Collections;
using System.Collections.Generic;
using Npc;
using UnityEngine;
using UnityEngine.AI;

public class PathNodeController : MonoBehaviour
{
    [SerializeField] private float _newMovementSpeed = -1;
    [SerializeField] private float _roundingForThisNode = -1;
    [SerializeField] private float _waitTimeAtThisNode = -1;
    [SerializeField] private float _bounceStrengthFromThisNode = -1;
    [SerializeField] private float _ballFollowSpeedFromThisNode = -1;
    [SerializeField] private List<TriggerScript> _triggerScripts;

    public float NewMovementSpeed => _newMovementSpeed;
    public float RoundingForThisNode => _roundingForThisNode;
    public float WaitTimeAtThisNode => _waitTimeAtThisNode;
    public float BounceStrengthFromThisNode => _bounceStrengthFromThisNode;
    public float BallFollowSpeedFromThisNode => _ballFollowSpeedFromThisNode;
    public List<TriggerScript> TriggerScripts => new List<TriggerScript>(_triggerScripts);

    public void Trigger()
    {
        if (_triggerScripts.Count == 0)
        {
            throw new Exception("A trigger node needs one or more trigger scripts.");
        }

        try
        {
            _triggerScripts.ForEach(script => script.ExecuteTrigger());
        }
        catch (Exception e)
        {
            Debug.LogError("Could not run trigger: " + e);
        }
    }

    public void ApplyNodeEffects(NavMeshAgent navMeshAgent, Transform npcTransform)
    {
    }
}