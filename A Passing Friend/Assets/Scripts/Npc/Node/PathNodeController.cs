using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Npc
{
    public class PathNodeController : MonoBehaviour
    {
        #region noCollapse

        [Space(10)]
        [Header("General NPC node controls")]

        #endregion

        [Tooltip("Set to -1 to disable")]
        [SerializeField]
        private float _newMovementSpeed = -1;

        [Tooltip("Set to -1 to disable")] [SerializeField]
        private float _roundingForThisNode = -1;

        [Tooltip("Set to -1 to disable")] [SerializeField]
        private float _waitTimeAtThisNode = -1;

        [SerializeField] private bool _teleportToNextNode = false;

        [SerializeField] private List<TriggerScript> _triggerScripts;

        #region noCollapse

        [Space(20)]
        [Header("Ball NPC node controls")]

        #endregion

        [Tooltip("Set to -1 to disable")]
        [SerializeField]
        private float _bounceStrengthFromThisNode = -1;

        [Tooltip("Set to -1 to disable")] [SerializeField]
        private float _ballFollowSpeedFromThisNode = -1;

        [SerializeField] private bool _lockBallToController = false;

        [SerializeField] private bool _unlockBallFromController = false;


        public float NewMovementSpeed => _newMovementSpeed;

        public float RoundingForThisNode => _roundingForThisNode;

        public float WaitTimeAtThisNode => _waitTimeAtThisNode;

        public bool TeleportToNextNode => _teleportToNextNode;

        public List<TriggerScript> TriggerScripts => _triggerScripts;

        public float BounceStrengthFromThisNode => _bounceStrengthFromThisNode;

        public float BallFollowSpeedFromThisNode => _ballFollowSpeedFromThisNode;

        public bool LockBallToController => _lockBallToController;
        public bool UnlockBallFromController => _unlockBallFromController;


        public void Trigger(GameObject npc)
        {
            if (_triggerScripts.Count == 0)
            {
                throw new Exception("A trigger node needs one or more trigger scripts.");
            }

            try
            {
                _triggerScripts.ForEach(script => script.ExecuteTrigger(npc));
            }
            catch (Exception e)
            {
                Debug.LogError("Could not run trigger: " + e);
            }
        }
    }
}