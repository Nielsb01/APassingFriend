using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateNeckboneTest : MonoBehaviour
{
    [SerializeField] private Animation _animation;
    [SerializeField] private Transform moveBone;

    [SerializeField] private HumanBodyBones _humanBodyBones;
    
    // Start is called before the first frame update
    void Start()
    {
        _animation["test"].AddMixingTransform(moveBone);
        moveBone.transform.rotation = new Quaternion(180, 180, 180, 0);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
