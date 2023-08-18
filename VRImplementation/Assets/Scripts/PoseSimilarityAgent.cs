using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class AnimPoseData
{
    public String Name;
    public int TotalAnimations;
    public int AnimationIndex;
}

public class PoseSimilarityAgent : MonoBehaviour
{

    public AnimPoseData[] PoseDatas;

    public Animator TrainerAnimator;

    public bool IsPlayingAnim = false;
    public BonePoseSimilarity SimilarityAgent;

    public bool HasHeldPose = false;
    public float LastPoseHeldTime = 0.0f;
    public float PoseHeldTime = 0.0f;

    public float DesirePoseHoldTime = 10.0f; // 10 seconds?

    [Header("Visuals")]
    public TMP_Text PoseDisplayText;
    // Start is called before the first frame update
    void Start()
    {
        this.SimilarityAgent = GetComponent<BonePoseSimilarity>();

    }

    // Update is called once per frame
    void Update()
    {
        if(this.SimilarityAgent != null) { 
            if(
               /* (this.SimilarityAgent.PositionalSimiliarityFactor.x <= this.SimilarityAgent.PositionDiff.x) && 
                (this.SimilarityAgent.PositionalSimiliarityFactor.y <= this.SimilarityAgent.PositionDiff.y) &&
                (this.SimilarityAgent.PositionalSimiliarityFactor.z <= this.SimilarityAgent.PositionDiff.z) &&*/
               this.SimilarityAgent.IsPoseSimilar && 
                !this.IsPlayingAnim
            )
            {
                if (!HasHeldPose) {
                    HasHeldPose = true;
                    LastPoseHeldTime = Time.time;
                }
                PoseHeldTime = Time.time - LastPoseHeldTime;
                PoseDisplayText.text = "Pose: Same, hold " + Mathf.Round(DesirePoseHoldTime - PoseHeldTime) + " sec";
                if (PoseHeldTime > DesirePoseHoldTime)
                {
                    // User has successfully held the pose for 10 seconds..
                    TrainerAnimator.SetTrigger("ShouldTriggerNextState");
                    TrainerAnimator.SetInteger("AnimationNumber", TrainerAnimator.GetInteger("AnimationNumber") + 1);
                }
                //Pose is similar
            }
            else
            {
                PoseHeldTime = 0.0f;
                HasHeldPose = false;
                PoseDisplayText.text = "Pose: Not same";
            }
        }
    }
}
