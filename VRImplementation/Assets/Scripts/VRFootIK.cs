using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRFootIK : MonoBehaviour
{
    public Animator animator;

    [Range(0,1)]
    public float rightFootWeight = 1.0f;
    [Range(0, 1)]
    public float leftFootWeight = 1.0f;

    public Vector3[] FeetPosition = new Vector3[2];
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        FeetPosition[0] = Vector3.zero;
        FeetPosition[1] = Vector3.zero;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        CheckFootIK(AvatarIKGoal.LeftFoot, leftFootWeight, 0);
        CheckFootIK(AvatarIKGoal.RightFoot, rightFootWeight, 1);
    }

    public void CheckFootIK(AvatarIKGoal goal, float weight, int feetIndex)
    {
        int layerMask = 7;
        Vector3 footPos = animator.GetIKPosition(goal);
        Debug.Log("Foot Pos: " + footPos);
        RaycastHit hit;
        FeetPosition[feetIndex] = footPos;
        bool legHit = Physics.Raycast(footPos + Vector3.up, Vector3.down, out hit, Mathf.Infinity, layerMask);
        if (legHit)
        {
            animator.SetIKPositionWeight(goal, weight);
            animator.SetIKPosition(goal, hit.point);
        }
        else
        {
            animator.SetIKPositionWeight(goal, 0);
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawSphere(FeetPosition[0], 0.05f);
        Gizmos.DrawSphere(FeetPosition[1], 0.05f);
    }
}
