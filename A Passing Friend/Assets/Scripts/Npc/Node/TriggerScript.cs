using UnityEngine;

namespace Npc
{
    public abstract class TriggerScript: MonoBehaviour
    {
        public abstract void ExecuteTrigger(GameObject npc);
    }
}