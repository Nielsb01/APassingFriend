using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class NpcAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _npcAnimator;
    private float _npcSpeed;
    private bool _npcIsTalking;
    private bool _npcIsPickingUp;
    private NavMeshAgent _navMeshAgent;

    public void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        SetNpcSpeed(_navMeshAgent.velocity.magnitude);
    }

    public void SetNpcSpeed(float npcSpeed)
    {
        _npcSpeed = npcSpeed;
        _npcAnimator.SetFloat("speed", _npcSpeed);
    }

    public void SetNpcTalking(bool npcIsTalking)
    {
        _npcIsTalking = npcIsTalking;
        _npcAnimator.SetBool("isTalking", _npcIsTalking);
    }

    public void SetNpcIsPickingup(bool npcIsPickingUp)
    {
        _npcIsPickingUp = npcIsPickingUp;
        _npcAnimator.SetBool("isTakingItem", _npcIsPickingUp);
    }

    public void SetAnimationState(NpcAnimations animation)
    {
        switch (animation)
        {
            case NpcAnimations.startPickup:
                SetNpcIsPickingup(true);
                return;
            case NpcAnimations.stopPickup:
                SetNpcIsPickingup(false);
                return;
            case NpcAnimations.startTalking:
                SetNpcTalking(true);
                return;
            case NpcAnimations.stopTalking:
                SetNpcTalking(false);
                return;
            case NpcAnimations.startWalking:
                SetNpcSpeed(1);
                return;
            case NpcAnimations.stopWalking:
                SetNpcSpeed(0);
                return;
        }
    }
}