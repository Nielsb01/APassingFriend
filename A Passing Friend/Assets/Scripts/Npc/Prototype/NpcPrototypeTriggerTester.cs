using UnityEngine;

namespace Npc.Prototype
{
    public class NpcPrototypeTriggerTester : TriggerScript
    {
         
        public override void ExecuteTrigger(GameObject npc)
        {
            GetComponent<Renderer>().material.color = Random.ColorHSV();
        }
    }
}
