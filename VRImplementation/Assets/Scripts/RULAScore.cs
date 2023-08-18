using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class RULAScore : MonoBehaviour
{

    [Header("Neck")]
    public Transform Neck;

    [Header("Spine")]
    public Transform LowerSpine;

    [Header("Left-Arm")]
    public Transform LeftArm;
    public Transform LeftForeArm;
    public Transform LeftWrist;
    [Header("Right-Arm")]
    public Transform RightArm;
    public Transform RightForeArm;
    public Transform RightWrist;
    // Start is called before the first frame update

    public int LeftUpperArmScore;
    public int RightUpperArmScore;
    public int LeftLowerArmScore;
    public int RightLowerArmScore;
    public int LowerArmScore;
    public int TrunkScore;
    public int WristScore = 1;
    public int WristTwist = 1;
    public int NeckScore;


    [Header("Angles")]
    public float TrunkAngle;
    public float LeftArmElevation_Y = 0.0f;
    public float RightArmElevation_Y = 0.0f;
    public Vector3 LeftArmGlobalRotations;
    public Vector3 RightArmGlobalRotations;
    public Vector3 LeftArmLocalRotations;
    public Vector3 RightArmLocalRotations;
    public float NeckAngle;
    public float NeckBend;

    [Header("Debug texts")]
    public GameObject ScorePanel;
    public TMP_Text NeckScoreText;
    public TMP_Text LeftUpperArmScoreText;
    public TMP_Text RightUpperArmScoreText;
    public TMP_Text TrunkScoreText;
    public TMP_Text LeftLowerArmScoreText;
    public TMP_Text RightLowerArmScoreText;
    public TMP_Text RulaScoreText;
    public TMP_Text PostureAdviseText;
    //public float[][]


    public int[,] TableA;
    public int[,] TableB;
    public int[,] TableC;
    void Start()
    {
        TableA = new int[,]
        {
            {1,2,2,2,2,3,3,3},
            {2,2,2,2,3,3,3,3},
            {2,3,3,3,3,3,4,4},
            {2,3,3,3,3,4,4,4},
            {3,3,3,3,3,4,4,4},
            {3,4,4,4,4,4,5,5},
            {3,3,4,4,4,4,5,5},
            {3,4,4,4,4,4,5,5},
            {4,4,4,4,4,5,5,5},
            {4,4,4,4,4,5,5,5},
            {4,4,4,4,4,5,5,5},
            {4,4,4,5,5,5,6,6},
            {5,5,5,5,5,6,6,7},
            {5,6,6,6,6,7,7,7},
            {6,6,6,7,7,7,7,8},
            {7,7,7,7,7,8,8,9},
            {8,8,8,8,8,9,9,9},
            {9,9,9,9,9,9,9,9}

        };
        TableB = new int[,]
        {
            {1,3,2,3,3,4,5,5,6,6,7,7},
            {2,3,2,3,4,5,5,5,6,7,7,7},
            {3,3,3,4,4,5,5,6,6,7,7,7},
            {5,5,5,6,6,7,7,7,7,7,8,8},
            {7,7,7,7,7,8,8,8,8,8,8,8},
            {8,8,8,8,8,8,8,9,9,9,9,9}
        };
        TableC = new int[,]
        {
            {1,2,3,3,4,5,5},
            {2,2,3,4,4,5,5},
            {3,3,3,4,4,5,6},
            {3,3,3,4,5,6,6},
            {4,4,4,5,6,7,7},
            {4,4,5,6,6,7,7},
            {5,5,6,6,7,7,7},
            {5,5,6,7,7,7,7}
        };
    }

    // Update is called once per frame
    void Update()
    {

        //  ======================================================================
        //  UPPER ARM SCORE
        //  ======================================================================
        Debug.Log(LeftArm.eulerAngles.z);
        LeftUpperArmScore = GetUpperArmScore(LeftArm);
        RightUpperArmScore = GetUpperArmScore(RightArm);
        LeftLowerArmScore = GetLowerArmScore(LeftForeArm);
        RightLowerArmScore = GetLowerArmScore(RightForeArm);
    //  ======================================================================
    //  TRUNK SCORE
    //  ======================================================================
        TrunkScore = 0;
        TrunkAngle = Mathf.Abs(LowerSpine.eulerAngles.x);
        if (TrunkAngle == 0.0f)
        {
            TrunkScore += 1;
        }
        else if(TrunkAngle > 0.0f && TrunkAngle <= 20.0f)
        {
            TrunkScore += 2;
        }
        else if (TrunkAngle > 20.0f && TrunkAngle <= 60.0f)
        {
            TrunkScore += 3;
        }
        else if (TrunkAngle > 60.0f)
        {
            TrunkScore += 4;
        }

        // ========================================================================
        // NECK SCORE
        // ========================================================================
        NeckScore = GetNeckScore(Neck);

        TrunkScoreText.text = "Trunk: " + TrunkScore;
        LeftUpperArmScoreText.text = "LeftUpperArm: " + LeftUpperArmScore;
        RightUpperArmScoreText.text = "RightUpperArm: " + RightUpperArmScore;
        NeckScoreText.text = "Neck: " + NeckScore ;
        LeftLowerArmScoreText.text = "LeftLowerArm: " + LeftLowerArmScore;
        RightLowerArmScoreText.text = "RightLowerArm: " + RightLowerArmScore;
        int score = GetRULAScore(
            TableA[
                //GetRowIndexTableA(LeftUpperArmScore, LeftLowerArmScore), GetColumnIndexTableA(1, 1)
                (int)((float)GetRowIndexTableA(LeftUpperArmScore, LeftLowerArmScore) + GetRowIndexTableA(RightUpperArmScore, RightLowerArmScore) / 2.0f), GetColumnIndexTableA(1, 1)
            ],
            TableB[
                GetRowIndexTableB(NeckScore), GetColumnIndexTableB(TrunkScore, 2)
            ]
            );
        RulaScoreText.text = "RULA Score: " + score;
        if (score > 0 && score < 3)
        {
            PostureAdviseText.text = "Good Posture";
            PostureAdviseText.color = Color.green;
        }
        else if(score > 2 && score < 5)
        {
            PostureAdviseText.text = "Okay Posture";
            PostureAdviseText.color = Color.yellow;
        }
        else if (score > 4)
        {
            PostureAdviseText.text = "Bad Posture";
            PostureAdviseText.color = Color.red;
        }
        LeftArmGlobalRotations = LeftArm.eulerAngles;
        LeftArmLocalRotations = LeftArm.localEulerAngles;
        RightArmLocalRotations = RightArm.localEulerAngles;
        RightArmGlobalRotations = RightArm.eulerAngles;

    }
    public int GetUpperArmScore(Transform arm)
    {
        AngleCalculator calculator = arm.GetComponent<AngleCalculator>();
        int score = 0;
        if(calculator != null)
        {
            float ArmAbduction = calculator.AbductionAngle;
            float ArmElevation = calculator.RaisingAngle;
            if (ArmAbduction > 0.0f)
            {
                score += 1;
            }
            if (ArmElevation > 0.0f && ArmElevation <= 20.0f) score += 1;
            else if (ArmElevation > 20.0f && ArmElevation <= 45.0f) score += 2;
            else if (ArmElevation > 45.0f && ArmElevation <= 90.0f) score += 3;
            else if (ArmElevation > 90.0f) score += 4;
        }
        /*
        int score = 0;
        float ArmAbduction = 90.0f - Mathf.Abs(arm.localEulerAngles.x);
        float ArmElevation = 90.0f - Mathf.Abs(arm.localEulerAngles.x);
        if (arm.name.Contains("Left"))
            LeftArmElevation_Y = ArmElevation;
        else
            RightArmElevation_Y = ArmElevation;
        if(ArmAbduction > 0.0f)
        {
            score += 1;
        }
        if (ArmElevation > 0.0f && ArmElevation <= 20.0f) score += 1;
        else if (ArmElevation > 20.0f && ArmElevation <= 45.0f) score += 2;
        else if (ArmElevation > 45.0f && ArmElevation <= 90.0f) score += 3;
        else if (ArmElevation > 90.0f) score += 4;
        */
        return score;

    }
    public int GetLowerArmScore(Transform arm)
    {
        int score = 0;
        float ArmAbduction = 90.0f - Mathf.Abs(arm.localEulerAngles.x);
        float ArmElevation = 90.0f - Mathf.Abs(arm.localEulerAngles.x);
        if (arm.name.Contains("Left"))
            LeftArmElevation_Y = ArmElevation;
        else
            RightArmElevation_Y = ArmElevation;
        if (ArmAbduction > 0.0f)
        {
            score += 1;
        }
        if (ArmElevation >= 0.0 && ArmElevation <= 60.0f) score += 2;
        else if (ArmElevation > 60.0f && ArmElevation <= 100.0f) score += 1;
        return 2;

    }
    public int GetNeckScore(Transform neck)
    {
        NeckAngle = neck.eulerAngles.x;
        NeckBend = neck.eulerAngles.z;
        if (NeckAngle >= 0.0f && NeckAngle <= 10.0f) return 1;
        else if (NeckAngle > 10.0f && NeckAngle <= 20.0f) return 2;
        else if (NeckAngle > 20.0f) return 3;
        else return 4;
    }

    public int GetRowIndexTableA(int UpperArmScore, int LowerArmScore)
    {
        if(UpperArmScore == 1)
        {
            if (LowerArmScore == 1) return 0;
            else if (LowerArmScore == 2) return 1;
            else if (LowerArmScore == 3) return 2;
        }
        else if (UpperArmScore == 2)
        {
            if (LowerArmScore == 1) return 3;
            else if (LowerArmScore == 2) return 4;
            else if (LowerArmScore == 3) return 5;
        }
        if (UpperArmScore == 3)
        {
            if (LowerArmScore == 1) return 6;
            else if (LowerArmScore == 2) return 7;
            else if (LowerArmScore == 3) return 8;
        }
        if (UpperArmScore == 4)
        {
            if (LowerArmScore == 1) return 9;
            else if (LowerArmScore == 2) return 10;
            else if (LowerArmScore == 3) return 11;
        }
        if (UpperArmScore == 5)
        {
            if (LowerArmScore == 1) return 12;
            else if (LowerArmScore == 2) return 13;
            else if (LowerArmScore == 3) return 14;
        }
        if (UpperArmScore == 6)
        {
            if (LowerArmScore == 1) return 15;
            else if (LowerArmScore == 2) return 16;
            else if (LowerArmScore == 3) return 17;
        }
        return 0;
    }
    public int GetColumnIndexTableA(int UpperArmScore, int LowerArmScore)
    {
        if (UpperArmScore == 1)
        {
            if (LowerArmScore == 1) return 0;
            else if (LowerArmScore == 2) return 1;
        }
        else if (UpperArmScore == 2)
        {
            if (LowerArmScore == 1) return 2;
            else if (LowerArmScore == 2) return 3;
        }
        if (UpperArmScore == 3)
        {
            if (LowerArmScore == 1) return 4;
            else if (LowerArmScore == 2) return 5;
        }
        if (UpperArmScore == 4)
        {
            if (LowerArmScore == 1) return 6;
            else if (LowerArmScore == 2) return 7;
        }
        return 0;
    }
    public int GetRowIndexTableB(int NeckScore)
    {
        return NeckScore;
    }
    public int GetColumnIndexTableB(int TrunkScore, int LegScore)
    {
        if(TrunkScore == 1)
        {
            if (LegScore == 1) return 0;
            else if(LegScore == 2) return 1;
        }
        else if (TrunkScore == 2)
        {
            if (LegScore == 1) return 2;
            else if (LegScore == 2) return 3;
        }
        else if (TrunkScore == 3)
        {
            if (LegScore == 1) return 4;
            else if (LegScore == 2) return 5;
        }
        else if (TrunkScore == 4)
        {
            if (LegScore == 1) return 6;
            else if (LegScore == 2) return 7;
        }
        else if (TrunkScore == 5)
        {
            if (LegScore == 1) return 8;
            else if (LegScore == 2) return 9;
        }
        else if (TrunkScore == 6)
        {
            if (LegScore == 1) return 10;
            else if (LegScore == 2) return 11;
        }
        return 0;
    }
    public int GetRULAScore(int TableAScore, int TableBScore)
    {
        int tabAScore = TableAScore - 1;
        int tabBScore = TableBScore - 1;
        if(tabAScore > 7)
        {
            tabAScore = 7;
        }
        if(tabBScore > 6)
        {
            tabBScore = 6;
        }
        return TableC[tabAScore,tabBScore];
    }
}
