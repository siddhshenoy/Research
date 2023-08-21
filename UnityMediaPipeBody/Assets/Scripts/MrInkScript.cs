using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrInkScript : MonoBehaviour
{
    [Header("Trainer")]

    public Transform TrainerObject;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TrainerObject.transform.position = this.transform.position + offset;
        TrainerObject.transform.forward = this.transform.forward;
    }
}
