using Unity.VisualScripting;
using UnityEngine;

public class PathNodeColorSelector : MonoBehaviour
{
    [SerializeField] private Material _defaultNodeMaterial;
    [SerializeField] private Material _triggerNodeMaterial;
    [SerializeField] private Material _speedChangeNodeMaterial;
    [SerializeField] private Material _waitNodeMaterial;
    [SerializeField] private Material _customRoundingNodeMaterial;
    [SerializeField] private Material _bounceNodeMaterial;
    [SerializeField] private Material _followBallSpeedChangeNodeMaterial;
    private Renderer _renderer;

    public void OnValidate()
    {
        _renderer = GetComponent<Renderer>();
        ChangeColourDependingOnNodeActivities();
    }

    private void ChangeColourDependingOnNodeActivities()
    {
        var controller = GetComponent<PathNodeController>();
        if (controller.TriggerScripts.Count != 0)
        {
            _renderer.material = _triggerNodeMaterial;
        }
        else if (controller.WaitTimeAtThisNode > -1)
        {
            _renderer.material = _waitNodeMaterial;
        }
        else if (controller.NewMovementSpeed > -1)
        {
            _renderer.material = _speedChangeNodeMaterial;
        }
        else if (controller.RoundingForThisNode > -1)
        {
            _renderer.material = _customRoundingNodeMaterial;
        }
        else if (controller.BounceStrengthFromThisNode > -1)
        {
            _renderer.material = _defaultNodeMaterial;
        }
        else if (controller.BallFollowSpeedFromThisNode > -1)
        {
            _renderer.material = _defaultNodeMaterial;
        }
        else
        {
            _renderer.material = _defaultNodeMaterial;
        }
    }
}