using UnityEngine;

namespace Npc.Prototype
{
    public class NpcPrototypeTriggerTester : TriggerScript
    {
         
        public override void ExecuteTrigger(NpcMovementController npc)
        {
            GetComponent<Renderer>().material.color = Random.ColorHSV();
        }
    }
}
