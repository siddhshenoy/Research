using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;


public class BonePoseSimilarity : MonoBehaviour
{
    [Header("Debug Labels")]
    public TMP_Text methodLabel;
    public TMP_Text rmseLabel;

    [Header("Target Bones")]
    public Transform tgtRootBone;
    public Transform[] tgtBones;

    [Header("Source Bones")]
    public Transform srcRootBone;
    public Transform[] srcBones;

    [Header("Similarities")]
    public float[] manhanttanSimilarities;
    public float[] euclideanSimiliarities;
    public float[] cosineSimilarities;
    public float[] chebyshevSimilarities;
    public bool IsPoseSimilar = false;
    public SimilarityAlgorithm Algorithm;


    [Header("Thresholds")]
    public float minEuclideanError = 0.1f;
    public float minCosineSimilarity = 0.9f;
    public float minCosineDistance = 0.1f;
    public float minManhattanDistance = 0.2f;
    public float minChebyshevDistance = 0.1f;

    [Header("Metrics")]
    public float RSME;

    [Header("Coding stuff")]
    public int CurrentAlgorithm = 0;
    public enum SimilarityAlgorithm
    {
        Cosine,
        Euclidean,
        Manhattan,
        Chebyshev
    }

    public class TargetSrcData
    {
        public Vector3[] srcBoneData;
        public Vector3[] tgtboneData;
        public String[] matchData;
    };

    public List<TargetSrcData> TargetSrcDataList = new List<TargetSrcData>();


    public void Start()
    {
        this.manhanttanSimilarities = new float[tgtBones.Length];
        this.euclideanSimiliarities = new float[tgtBones.Length];
        this.cosineSimilarities = new float[tgtBones.Length];
        this.chebyshevSimilarities = new float[tgtBones.Length];
    }
    public void Update()
    {
        for(int i = 0; i < tgtBones.Length; i++)
        {
            euclideanSimiliarities[i] = euclideanDistance(
                tgtBones[i].transform.position.normalized,
                srcBones[i].transform.position.normalized
                );
            cosineSimilarities[i] = cosineSimilarity(
                tgtBones[i].transform.position.normalized,
                srcBones[i].transform.position.normalized
                );
            manhanttanSimilarities[i] = manhattanDistance(
                tgtBones[i].transform.position.normalized,
                srcBones[i].transform.position.normalized
                );
            chebyshevSimilarities[i] = chebyshevDistance(
                tgtBones[i].transform.position.normalized,
                srcBones[i].transform.position.normalized
                );
        }
        if (Algorithm == SimilarityAlgorithm.Euclidean)
        {
            for (int i = 0; i < euclideanSimiliarities.Length; i++)
            {
                if (euclideanSimiliarities[i] < minEuclideanError)
                {
                    IsPoseSimilar = true;
                    continue;
                }
                else
                {
                    IsPoseSimilar = false;
                    break;
                }
            }
        }
        else if (Algorithm == SimilarityAlgorithm.Cosine)
        {
            for(int i = 0; i < cosineSimilarities.Length; i++)
            {
                if (cosineSimilarities[i] >= minCosineSimilarity)
                {
                    IsPoseSimilar = true;
                    continue;
                }
                else
                {
                    IsPoseSimilar = false;
                    break;
                }
            }
        }
        else if(Algorithm == SimilarityAlgorithm.Manhattan)
        {
            for (int i = 0; i < manhanttanSimilarities.Length; i++)
            {
                if (manhanttanSimilarities[i] < minManhattanDistance)
                {
                    IsPoseSimilar = true;
                    continue;
                }
                else
                {
                    IsPoseSimilar = false;
                    break;
                }
            }
        }
        else if (Algorithm == SimilarityAlgorithm.Chebyshev)
        {
            for (int i = 0; i < manhanttanSimilarities.Length; i++)
            {
                if (chebyshevSimilarities[i] < minChebyshevDistance)
                {
                    IsPoseSimilar = true;
                    continue;
                }
                else
                {
                    IsPoseSimilar = false;
                    break;
                }
            }
        }
        RSME = CalculateRMSE();
        if(methodLabel != null)
        {
            methodLabel.text = "Method: " + Algorithm.ToString();
        }
        if(rmseLabel != null)
        {
            Vector3 rsme_axes = CalculateRMSEAxes();
            rmseLabel.text = "RMSE: " + rsme_axes.x + ", " + rsme_axes.y + ", " + rsme_axes.z;
        }
        if(OVRInput.GetDown(OVRInput.Button.Four))
        {
            CurrentAlgorithm = (CurrentAlgorithm + 1) % 4;
            if (CurrentAlgorithm == 0) Algorithm = SimilarityAlgorithm.Cosine;
            else if (CurrentAlgorithm == 1) Algorithm = SimilarityAlgorithm.Euclidean;
            else if (CurrentAlgorithm == 2) Algorithm = SimilarityAlgorithm.Manhattan;
            else if (CurrentAlgorithm == 3) Algorithm = SimilarityAlgorithm.Chebyshev;

        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            string csv_path = Application.persistentDataPath + "/data.csv";
            if (File.Exists(csv_path))
            {
                File.Delete(csv_path);
            }
            FileStream f = File.Create(csv_path);
            StreamWriter writer = new StreamWriter(f);
            string headerStr = "";
            for(int i = 0; i < tgtBones.Length; i++)
            {
                headerStr += PrepareVectorHeaderName(tgtBones[i].name, "user_") + ",";
            }
            for (int i = 0; i < srcBones.Length; i++)
            {
                headerStr += PrepareVectorHeaderName(srcBones[i].name, "trnr_") + ",";
            }
            headerStr += "Euclidean,Manhattan,Chebyshev,Cosine\n";
            string dataString = "";
            for(int i = 0; i < TargetSrcDataList.Count; i++)
            {
                TargetSrcData dt = TargetSrcDataList[i];
                for(int j = 0; j < dt.tgtboneData.Length; j++)
                {
                    dataString += PrepareVectorString(dt.tgtboneData[j]) + ",";
                }
                for (int j = 0; j < dt.srcBoneData.Length; j++)
                {
                    dataString += PrepareVectorString(dt.srcBoneData[j]) + ",";
                }
                dataString += dt.matchData[0] + ", " + dt.matchData[1] + ", " + dt.matchData[2] + ", " + dt.matchData[3] + "\n";
            }
            writer.Write(headerStr + dataString);
            writer.Close();
            f.Close();

        }
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            TargetSrcData data = new TargetSrcData();
            data.tgtboneData = new Vector3[tgtBones.Length];
            data.srcBoneData = new Vector3[srcBones.Length];
            for(int i = 0; i < tgtBones.Length; i++)
            {
                data.tgtboneData[i] = new Vector3(
                    tgtBones[i].transform.position.x,
                    tgtBones[i].transform.position.y,
                    tgtBones[i].transform.position.z);
                data.srcBoneData[i] = new Vector3(
                    srcBones[i].transform.position.x,
                    srcBones[i].transform.position.y,
                    srcBones[i].transform.position.z);
            }
            data.matchData = new string[4];
            data.matchData[0] = returnBoolString(EvaluateAlgo(SimilarityAlgorithm.Euclidean));
            data.matchData[1] = returnBoolString(EvaluateAlgo(SimilarityAlgorithm.Manhattan));
            data.matchData[2] = returnBoolString(EvaluateAlgo(SimilarityAlgorithm.Chebyshev));
            data.matchData[3] = returnBoolString(EvaluateAlgo(SimilarityAlgorithm.Cosine));
            TargetSrcDataList.Add(data);
        }
    }
    public String PrepareVectorString(Vector3 data) { return "" + data.x + "," + data.y + "," + data.z;  }
    public String PrepareVectorHeaderName(String name, String pref) { return (pref + name) + "_x," + (pref + name) + "_y," + (pref + name) + "_z"; }
    public string returnBoolString(bool v) { if (v) return "True"; else return "False"; }
    public bool EvaluateAlgo(SimilarityAlgorithm algo)
    {
        bool res = false;
        if (algo == SimilarityAlgorithm.Euclidean)
        {
            for (int i = 0; i < euclideanSimiliarities.Length; i++)
            {
                if (euclideanSimiliarities[i] < minEuclideanError)
                {
                    res = true;
                    continue;
                }
                else
                {
                    res = false;
                    break;
                }
            }
        }
        else if (algo == SimilarityAlgorithm.Cosine)
        {
            for (int i = 0; i < cosineSimilarities.Length; i++)
            {
                if (cosineSimilarities[i] >= minCosineSimilarity)
                {
                    res = true;
                    continue;
                }
                else
                {
                    res = false;
                    break;
                }
            }
        }
        else if (algo == SimilarityAlgorithm.Manhattan)
        {
            for (int i = 0; i < manhanttanSimilarities.Length; i++)
            {
                if (manhanttanSimilarities[i] < minManhattanDistance)
                {
                    res = true;
                    continue;
                }
                else
                {
                    res = false;
                    break;
                }
            }
        }
        else if (algo == SimilarityAlgorithm.Chebyshev)
        {
            for (int i = 0; i < manhanttanSimilarities.Length; i++)
            {
                if (chebyshevSimilarities[i] < minChebyshevDistance)
                {
                    res = true;
                    continue;
                }
                else
                {
                    res = false;
                    break;
                }
            }
        }
        return res;
    }
    public float manhattanDistance(Vector3 jointA, Vector3 jointB)
    {
        float xDiff = Mathf.Abs(jointA.x - jointB.x);
        float yDiff = Mathf.Abs(jointA.y - jointB.y);
        float zDiff = Mathf.Abs(jointA.z - jointB.z);
        return (xDiff + yDiff + zDiff);
    }
    public float chebyshevDistance(Vector3 jointA, Vector3 jointB)
    {
        float xDiff = Mathf.Abs(jointA.x - jointB.x);
        float yDiff = Mathf.Abs(jointA.y - jointB.y);
        float zDiff = Mathf.Abs(jointA.z - jointB.z);
        return Mathf.Max(new float[] {xDiff, yDiff, zDiff});
    }
    public float euclideanDistance(Vector3 jointA, Vector3 jointB)
    {
        return Vector3.Distance(jointA, jointB);
    }
    public float cosineSimilarity(Vector3 jointA, Vector3 jointB)
    {
        return (Vector3.Dot(jointA, jointB) / (jointA.magnitude * jointB.magnitude));
    }
    public float cosineDistance(Vector3 jointA, Vector3 jointB)
    {
        return 1 - cosineSimilarity(jointA, jointB);
    }
    public float CalculateRMSE()
    {
        float result = 0.0f;
        for(int i = 0; i <  tgtBones.Length; i++)
        {
            result += Mathf.Pow((tgtBones[i].transform.position.normalized - srcBones[i].transform.position.normalized).magnitude, 2.0f);
        }
        result /= tgtBones.Length;
        return Mathf.Sqrt(result);
    }

    public Vector3 CalculateRMSEAxes()
    {
        float resultX = 0.0f;
        float resultY = 0.0f;
        float resultZ = 0.0f;
        for (int i = 0; i < tgtBones.Length; i++)
        {
            //result += Mathf.Pow((tgtBones[i].transform.position.normalized - srcBones[i].transform.position.normalized).magnitude, 2.0f);
            resultX += Mathf.Pow(tgtBones[i].transform.position.normalized.x - srcBones[i].transform.position.normalized.x, 2.0f);
            resultY += Mathf.Pow(tgtBones[i].transform.position.normalized.y - srcBones[i].transform.position.normalized.y, 2.0f);
            resultZ += Mathf.Pow(tgtBones[i].transform.position.normalized.z - srcBones[i].transform.position.normalized.z, 2.0f);
        }
        resultX = Mathf.Sqrt(resultX /tgtBones.Length);
        resultY = Mathf.Sqrt(resultY / tgtBones.Length);
        resultZ = Mathf.Sqrt(resultZ / tgtBones.Length);
        return new Vector3(resultX, resultY, resultZ);
    }


    /*[Header("Similarities")]
    public Vector3[] RotationalSimilarities;
    public Vector3[] PositionalSimilarities;
    public float[] CosineSimilarities;
    public bool[] Similitaries;
    public bool IsPoseSimilar = false;
    public int TotalTrues = 0;
    public float SimilarityFactor = 0.0f;
    public Vector3 PositionalSimiliarityFactor = Vector3.zero;

    [Header("Allowable Differences")]
    public Vector3 RotationDiff = new Vector3(20.0f, 20.0f, 20.0f);
    public Vector3 PositionDiff = new Vector3(1.0f, 1.0f, 1.0f);
    
    // Start is called before the first frame update
    void Start()
    {
        this.RotationalSimilarities = new Vector3[tgtBones.Length];
        this.PositionalSimilarities = new Vector3[tgtBones.Length];
        this.CosineSimilarities = new float[tgtBones.Length];
        this.Similitaries = new bool[tgtBones.Length];
        for (int i = 0; i < tgtBones.Length; i++)
        {
            this.Similitaries[i] = false;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
        TotalTrues = 0;
        for(int i = 0; i < tgtBones.Length; i++)
        {
            
            this.RotationalSimilarities[i] = Vector3.zero;
            this.PositionalSimilarities[i] = Vector3.zero;
            this.RotationalSimilarities[i] = tgtBones[i].eulerAngles - srcBones[i].eulerAngles;
            this.PositionalSimilarities[i] = Vec3Abs(tgtBones[i].position.normalized - srcBones[i].position.normalized);
            this.CosineSimilarities[i] = CosineSimilarity(tgtBones[i].position, srcBones[i].position);
            if (this.PositionalSimilarities[i].x <= PositionDiff.x && this.PositionalSimilarities[i].y <= PositionDiff.y && this.PositionalSimilarities[i].z <= PositionDiff.z)
            {
                this.Similitaries[i] = true;
                TotalTrues++;
            }
            else
            {
                this.Similitaries[i] = false;
            }
            this.PositionalSimiliarityFactor += this.PositionalSimilarities[i];
        }
        this.PositionalSimiliarityFactor /= tgtBones.Length;
        if (this.PositionalSimiliarityFactor.x <= PositionDiff.x && this.PositionalSimiliarityFactor.y <= PositionDiff.y && PositionalSimiliarityFactor.z <= PositionDiff.z)
        {
            IsPoseSimilar = true;
        }
        else
        {
            IsPoseSimilar = false;
        }
        
    }
    public float CosineSimilarity(Vector3 a, Vector3 b)
    {
        return (Vector3.Dot(a, b)) / (a.magnitude * b.magnitude);
    }
    Vector3 Vec3Abs(Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }*/
}
