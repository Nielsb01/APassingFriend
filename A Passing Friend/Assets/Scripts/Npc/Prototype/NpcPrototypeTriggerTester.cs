using Npc;
using UnityEngine;
using Random = UnityEngine.Random;

public class NpcPrototypeTriggerTester :  MonoBehaviour, ITriggerScript
{
    public void ExecuteTrigger()
    {
        GetComponent<Renderer>().material.color = Random.ColorHSV();
    }
}
