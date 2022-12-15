using System;
using System.Collections;
using System.Collections.Generic;
using Npc;
using UnityEngine;

public class StartYarnWhenPlayerEnters : MonoBehaviour
{
    [SerializeField] private WaypointRoute _route;
    [SerializeField] private NpcMovementController _npc;
    private string _playertag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError("1");
        Debug.LogError(other.tag);

        if (other.CompareTag(_playertag))
        {
            Debug.Log("Starting yarn movement");
            _npc.StartRoute(_route);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.LogError("2");
    }
}