using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    private UIController _ui;

    private bool _npcInInteractRange;

    private float _interactRange = 2.5f;

    private Outline _outline;

    private DialogBuilder _npcDialogBuilder;

    [SerializeField] 
    private Camera _mainCam;

    [SerializeField] 
    private LayerMask _whatIsNpc;


    private void Start()
    {
        _outline = GetComponent<Outline>();
        _ui = GameObject.Find("UIDocument").GetComponent<UIController>();
    }

    private void Update()
    {
        /* Instead of this method, you can also use the RayCast in the PlayerController script to check if the player is in interact range.
           Preferably only use 1 method, but both also works. */
        _npcInInteractRange = Physics.CheckSphere(transform.position, _interactRange, _whatIsNpc);
        var npcs = Physics.OverlapSphere(transform.position, _interactRange, _whatIsNpc);
        foreach (var npc in npcs)
        {
            _outline = npc.transform.GetComponent<Outline>();
            _npcDialogBuilder = npc.transform.GetComponent<DialogBuilder>();
        }

        if (_outline != null)
        {
            if (_npcInInteractRange && !_outline.enabled)
            {
                _outline.enabled = true;
                _ui.SetInteractButtonVisibility();
                _ui.SetDialogBuilder(_npcDialogBuilder);
            }
            else if (!_npcInInteractRange && _outline.enabled)
            {
                _outline.enabled = false;
                _ui.SetInteractButtonVisibility();
            }
        }
    }
}
