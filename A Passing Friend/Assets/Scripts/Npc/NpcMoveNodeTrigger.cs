using System;
using System.Collections.Generic;
using UnityEngine;

namespace Npc
{
    public class NpcMoveNodeTrigger : MonoBehaviour
    {
        [SerializeField] private List<TriggerScript> _triggerScripts;

        public void Trigger()
        {
            if (_triggerScripts.Count == 0)
            {
                throw new Exception("A trigger node needs one or more trigger scripts.");
            }

            _triggerScripts.ForEach(script => { script.ExecuteTrigger(); });
        }
    }
}