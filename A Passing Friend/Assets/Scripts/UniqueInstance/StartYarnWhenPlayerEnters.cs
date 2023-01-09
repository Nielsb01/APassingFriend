using System.Collections.Generic;
using Npc;
using UnityEngine;

public class StartYarnWhenPlayerEnters : MonoBehaviour
{
    [SerializeField] private WaypointRoute _route;
    [SerializeField] private NpcMovementController _npc;
    [SerializeField] private List<GameObject> _deleteGameObjectsWhenBallMoves;
    private string _playertag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playertag))
        {
            _npc.StartRoute(_route);
            foreach (var gameObject in _deleteGameObjectsWhenBallMoves)
            {
                Destroy(gameObject);
            }
            _deleteGameObjectsWhenBallMoves.Clear();
        }
    }
}