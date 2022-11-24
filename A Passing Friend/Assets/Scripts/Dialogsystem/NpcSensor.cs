using UnityEngine;

public class NpcSensor : MonoBehaviour
{
    private UIController _ui;

    private const float INTERACT_RANGE = 2.5f;

    private Outline _outline;

    private DialogBuilder _npcDialogBuilder;

    [SerializeField] private UnityEngine.Camera _mainCam;

    [SerializeField] private LayerMask _npcLayerMask;


    private void Start()
    {
        _outline = GetComponent<Outline>();
        _ui = GameObject.Find("UIDocument").GetComponent<UIController>();
    }

    private void Update()
    {
        var npcs = Physics.OverlapSphere(transform.position, _interactRange, _npcLayerMask);
        foreach (var npc in npcs)
        {
            _outline = npc.transform.GetComponent<Outline>();
            _npcDialogBuilder = npc.transform.GetComponent<DialogBuilder>();
        }

        if (_outline != null)
        {
            if (npcs.Length > 0 && !_outline.enabled)
            {
                _outline.enabled = true;
                _ui.SetDialogSystemVisible();
                _ui.SetDialogBuilder(_npcDialogBuilder);
            }
            else if (npcs.Length <= 0 && _outline.enabled)
            {
                _outline.enabled = false;
                _ui.SetDialogSystemInvisible();
            }
        }
    }
}
