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
    
    void Update()
    {
        _jumpSprite.gameObject.SetActive(_characterMovementScript.getJumpCharged() > _characterMovementScript.GetMinimumChargeJumpValue());
        _maxJumpSprite.gameObject.SetActive(_characterMovementScript.getJumpCharged() > _characterMovementScript.GetMinimumChargeJumpValue());
        float y = _jumpSprite.GetComponent<RectTransform>().sizeDelta.y;
        _jumpSprite.GetComponent<RectTransform>().sizeDelta =
            new Vector2(_characterMovementScript.getJumpCharged() * 100, y);
        _maxJumpSprite.GetComponent<RectTransform>().sizeDelta =
            new Vector2(_characterMovementScript.getOverchargeLevel() * 100, y);
    }
}