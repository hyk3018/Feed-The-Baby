using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject follow = null;


    // Just keep locked onto the gameobject for now
    void Update()
    {
        var followPosition = follow.transform.position;
        transform.position = new Vector3(followPosition.x, followPosition.y,
            transform.position.z);
    }
}