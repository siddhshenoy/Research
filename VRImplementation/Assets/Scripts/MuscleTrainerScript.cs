using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationSequence
{
    public String AnimationSequenceName;
    public AnimationClip[] Clips;

}

public class MuscleTrainerScript : MonoBehaviour
{
    public AnimationSequence[] Sequences;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
