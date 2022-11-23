using System;
using System.Collections;
using System.Collections.Generic;
using Npc;
using UnityEngine;

public class NpcMoveNodeTrigger : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> _triggerScripts;

    public void Trigger()
    {
        if (_triggerScripts.Count == 0)
        {
            throw new Exception("A trigger node needs one or more trigger scripts.");
        }

        _triggerScripts.ForEach(script =>
        {
            var script2 = (ITriggerScript)script;
            script2.ExecuteTrigger();
        });
    }
}