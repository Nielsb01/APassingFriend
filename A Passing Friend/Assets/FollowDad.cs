using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowDad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var _rb = GetComponent<Rigidbody>();
        // _rb.AddForce(transform.position - GetComponentInParent<Transform>().position);
    }
}
