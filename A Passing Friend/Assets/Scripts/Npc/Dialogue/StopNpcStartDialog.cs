using Npc;
using UnityEngine;
using UnityEngine.AI;

public class StopNpcStartDialog : TriggerScript
{
    [SerializeField] private TextAsset _text;
    private NpcMovementController _npc;

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
        if (_npc == null) return;
        
        _npc.UnpauseNpc();
        _npc = null;
    }
    
    public override void ExecuteTrigger(NpcMovementController npc)
    {
        _npc = npc;
        _npc.PauseNpc();
        npc.GetComponent<DialogBuilder>().LoadDialog(_text);
    }
}
