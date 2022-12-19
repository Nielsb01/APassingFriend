using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcAnimationController : MonoBehaviour
{
    private Animator _npcAnimator;
    private float _npcSpeed;
    private bool _npcIsTalking; 
    private bool _npcIsPickingUp;

    public void Awake()
    {
      _npcAnimator = GetComponentInChildren(typeof(Animator)) as Animator;
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
}
