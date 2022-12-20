using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogChoiceAudio", menuName = "ScriptableObjects/DialogChoiceAudio", order = 2)]
public class DialogChoiceAudioSO : ScriptableObject
{
    public List<FMODUnity.EventReference> audioEvents;
}