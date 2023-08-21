using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleCalculator : MonoBehaviour
{
    public GameObject OriginalObject;
    public GameObject TipObject;
    public GameObject FrontObject;
    public GameObject BottomObject;

    public Vector3 FrontObjectLocation;
    public Vector3 BottomObjectLocation;

    public Vector3 OriginalObjectLocation;
    public Vector3 TipObjectLocation;
    public Vector3 direction;

    public float RaisingAngle;
    public float AbductionAngle;

    // Start is called before the first frame update
    void Start()
    {
        FrontObjectLocation = FrontObject.transform.position;
        BottomObjectLocation = BottomObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Assign the location
        FrontObjectLocation = FrontObject.transform.position;
        BottomObjectLocation = BottomObject.transform.position;
        OriginalObjectLocation = OriginalObject.transform.position;
        TipObjectLocation = TipObject.transform.position;

        //Calculate the difference of the arm to the tip
        //i.e., the end of the arm
        direction = OriginalObjectLocation - TipObjectLocation;
        Vector3 FrontDiffVector = (OriginalObjectLocation - FrontObjectLocation);
        Vector3 RightDiffVector = (OriginalObjectLocation - BottomObjectLocation);
        //Arm raising angle
        RaisingAngle = 90.0f - Mathf.Acos(
            Vector3.Dot(direction.normalized, FrontDiffVector.normalized)
            ) * Mathf.Rad2Deg;

        //Arm abduction angle
        AbductionAngle = 90.0f - Mathf.Acos(
            Vector3.Dot(direction.normalized, RightDiffVector.normalized)
            ) * Mathf.Rad2Deg;

    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(OriginalObject.transform.position, 0.025f);
    }
}
