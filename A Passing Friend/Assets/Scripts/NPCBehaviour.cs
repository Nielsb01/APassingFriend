using Cinemachine;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    private GameObject _npc;

    private UIController _ui;

    private bool _playerInInteractRange;

    private float _interactRange = 2.5f;

    private Outline _outline;

    [SerializeField] 
    private CinemachineVirtualCamera _mainCam;

    [SerializeField] 
    private LayerMask _whatIsPlayer;


    private void Start()
    {
        _npc = GetComponent<GameObject>();
        _outline = GetComponent<Outline>();
        _ui = GameObject.Find("UIDocument").GetComponent<UIController>();
    }

    private void Update()
    {
        /* Instead of this method, you can also use the RayCast in the PlayerController script to check if the player is in interact range.
           Preferably only use 1 method, but both also works. */
        _playerInInteractRange = Physics.CheckSphere(transform.position, _interactRange, _whatIsPlayer);

        if (_playerInInteractRange && !_outline.enabled)
        {
            _outline.enabled = true;
            _ui.SetInteractButtonVisibility();
        } 
        else if (!_playerInInteractRange && _outline.enabled)
        {
            _outline.enabled = false;
            _ui.SetInteractButtonVisibility();
        }
    }
}
