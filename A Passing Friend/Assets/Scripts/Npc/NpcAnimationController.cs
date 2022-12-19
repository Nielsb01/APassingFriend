using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class NpcAnimationController : MonoBehaviour
{
    private Animator _npcAnimator;
    private float _npcSpeed;
    private bool _npcIsTalking; 
    private bool _npcIsPickingUp;
    private NavMeshAgent _navMeshAgent;
    public void Awake()
    {
      _npcAnimator = GetComponentInChildren(typeof(Animator)) as Animator;
      _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        setNpcSpeed(_navMeshAgent.velocity.magnitude);
    }

    public void setNpcSpeed(float npcSpeed)
    {
        _npcSpeed = npcSpeed;
        _npcAnimator.SetFloat("speed",_npcSpeed);
    }

    public void setNpcTalking(bool npcIsTalking)
    {
        _npcIsTalking = npcIsTalking;
        _npcAnimator.SetBool("isTalking",_npcIsTalking);
    }

    public void setNpcIsPickingup(bool npcIsPickingUp)
    {
        _npcIsPickingUp = npcIsPickingUp;
        _npcAnimator.SetBool("isTakingItem",_npcIsPickingUp);
    }

    public void setAnimationState(NpcAnimations animation)
    {
        switch (animation)
        {
            case NpcAnimations.startPickup:
                setNpcIsPickingup(true);
                return;
            case NpcAnimations.stopPickup:
                setNpcIsPickingup(false);
                return;
            case NpcAnimations.startTalking:
                setNpcTalking(true);
                return;
            case NpcAnimations.stopTalking:
                setNpcTalking(false);
                return;
            case NpcAnimations.startWalking:
                setNpcSpeed(1);
                return;
            case NpcAnimations.stopWalking:
                setNpcSpeed(0);
                return;
        }
    }
}
