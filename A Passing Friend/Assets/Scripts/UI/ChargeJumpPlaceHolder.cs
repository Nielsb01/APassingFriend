    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    using UnityEngine.UIElements;

    public class ChargeJumpPlaceHolder : MonoBehaviour
{
    [SerializeField] private CharacterMovementScript characterMovementScript;

    [SerializeField] private Transform jumpSprite;
    [SerializeField] private Transform MaxJumpSprite;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float y = jumpSprite.GetComponent<RectTransform>().sizeDelta.y;
        jumpSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(characterMovementScript.getJumpCharged() * 100, y);
        MaxJumpSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(characterMovementScript.getOverchargeLevel() * 100, y);

    }
}
