using System;
using Npc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class StopNpcStartDialog : TriggerScript
{
    [SerializeField] private TextAsset _text;
    private GameObject _npc;
    private float _resumeSpeed;
    private NavMeshAgent _navMeshAgent;

    private void OnEnable()
    {
        UIController.DialogExited += ResumeMovement;
    }
    private void OnDisable()
    {
        UIController.DialogExited -= ResumeMovement;
    }

    private void ResumeMovement()
    {
        Debug.Log("Here " + _resumeSpeed);
        _navMeshAgent.speed = _resumeSpeed;
    }
    
    public override void ExecuteTrigger(GameObject npc)
    {
        _npc = npc;
        var dialog = _npc.GetComponent<DialogBuilder>();
        dialog.LoadDialog(_text);
        _navMeshAgent = _npc.GetComponent<NavMeshAgent>();
        _resumeSpeed = _navMeshAgent.speed;
        _navMeshAgent.speed = 0;
        dialog.LoadDialog(_text);
        
        
    }
}
