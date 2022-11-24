using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChargeJumpPlaceHolder : MonoBehaviour
{
    // TODO: This script is only for showing the placeholder UI and needs to be deleted after the UI is implemented
    [SerializeField] private CharacterMovementScript _characterMovementScript;

    [SerializeField] private Transform _jumpSprite;

    [SerializeField] private Transform _maxJumpSprite;

    // Start is called before the first frame update -Deze staat hier speciaal voor Hugo ;)
    void Start()
    {
    }

    void Update()
    {
        float y = _jumpSprite.GetComponent<RectTransform>().sizeDelta.y;
        _jumpSprite.GetComponent<RectTransform>().sizeDelta =
            new Vector2(_characterMovementScript.GetJumpCharged() * 100, y);
        _maxJumpSprite.GetComponent<RectTransform>().sizeDelta =
            new Vector2(_characterMovementScript.GetOverchargeLevel() * 100, y);
    }
}