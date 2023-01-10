using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogChoiceAudioSO", menuName = "ScriptableObjects/DialogChoiceAudioSO", order = 2)]
public class DialogChoiceAudioSO : ScriptableObject
{
    public List<FMODUnity.EventReference> audioEvents;
}