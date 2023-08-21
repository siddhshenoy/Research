using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

/* Currently very messy because both the server code and hand-drawn code is all in the same file here.
 * But it is still fairly straightforward to use as a reference/base.
 */


public class PipeServer : MonoBehaviour
{
    public Vector3 LeftShoulderForward;
    public Vector3 ShoulderToElbowDirection;
    public Vector3 ShoulderOffsetLeft = new Vector3(90.0f, 0.0f, 0.0f);
    public Vector3 ShoulderOffsetRight = new Vector3(90.0f, 0.0f, 0.0f);
    public Vector3 ElbowOffsetLeft = new Vector3(90.0f, 0.0f, 0.0f);
    public Vector3 ElbowOffsetRight = new Vector3(90.0f, 0.0f, 0.0f);
    public Vector3 LeftKneeOffset = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 RightKneeOffset = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 KneeOffset = new Vector3(0.0f, 0.0f, 0.0f);
    public Transform rParent;
    public Transform lParent;
    public GameObject landmarkPrefab;
    public GameObject linePrefab;
    public GameObject headPrefab;
    public bool enableHead = true;
    public float multiplier = 10f;
    public float landmarkScale = 1f;
    public float maxSpeed = 50f;
    public float debug_samplespersecond;
    [Header("Bone Parts")]
    public Vector3 ActualBodyOffset;
    public Transform ActualBody;
    public Transform LeftShoulder;
    public Transform LeftElbow;
    public Transform LeftWrist;
    public Transform RightShoulder;
    public Transform RightElbow;
    public Transform RightWrist;
    public Transform LeftHip;
    public Transform LeftKnee;
    public Transform LeftAnkle;
    public Transform RightHip;
    public Transform RightKnee;
    public Transform RightAnkle;

    public Transform Hips;
    public Transform Neck;

    //public Transform Spine0;

    //public Vector3 OriginalHipsForward;
    public Vector3 HipForward;
    public Vector3 HipCenter;

    public Dictionary<String, DictionaryMap> BodyPartGizmosMap = new Dictionary<string, DictionaryMap>();


    NamedPipeServerStream server;

    const int LANDMARK_COUNT = 33;
    public enum Landmark {
        NOSE = 0,
        LEFT_EYE_INNER = 1,
        LEFT_EYE = 2,
        LEFT_EYE_OUTER = 3,
        RIGHT_EYE_INNER = 4,
        RIGHT_EYE = 5,
        RIGHT_EYE_OUTER = 6,
        LEFT_EAR = 7,
        RIGHT_EAR = 8,
        MOUTH_LEFT = 9,
        MOUTH_RIGHT = 10,
        LEFT_SHOULDER = 11,
        RIGHT_SHOULDER = 12,
        LEFT_ELBOW = 13,
        RIGHT_ELBOW = 14,
        LEFT_WRIST = 15,
        RIGHT_WRIST = 16,
        LEFT_PINKY = 17,
        RIGHT_PINKY = 18,
        LEFT_INDEX = 19,
        RIGHT_INDEX = 20,
        LEFT_THUMB = 21,
        RIGHT_THUMB = 22,
        LEFT_HIP = 23,
        RIGHT_HIP = 24,
        LEFT_KNEE = 25,
        RIGHT_KNEE = 26,
        LEFT_ANKLE = 27,
        RIGHT_ANKLE = 28,
        LEFT_HEEL = 29,
        RIGHT_HEEL = 30,
        LEFT_FOOT_INDEX = 31,
        RIGHT_FOOT_INDEX = 32
    }
    const int LINES_COUNT = 11;

    public struct AccumulatedBuffer
    {
        public Vector3 value;
        public int accumulatedValuesCount;
        public AccumulatedBuffer(Vector3 v,int ac)
        {
            value = v;
            accumulatedValuesCount = ac;
        }
    }

    public class DictionaryMap
    {
        public Vector3 origin;
        public Vector3 direction;
        public Vector3 up;
    }

    public class Body 
    {
        public Transform parent;
        public AccumulatedBuffer[] positionsBuffer = new AccumulatedBuffer[LANDMARK_COUNT];
        public Vector3[] localPositionTargets = new Vector3[LANDMARK_COUNT];
        public GameObject[] instances = new GameObject[LANDMARK_COUNT];
        public LineRenderer[] lines = new LineRenderer[LINES_COUNT];

        public bool active;

        public Body(Transform parent, GameObject landmarkPrefab, GameObject linePrefab,float s, GameObject headPrefab)
        {
            this.parent = parent;
            for (int i = 0; i < instances.Length; ++i)
            {
                instances[i] = Instantiate(landmarkPrefab);// GameObject.CreatePrimitive(PrimitiveType.Sphere);
                
                instances[i].transform.localScale = Vector3.one * s;
                instances[i].transform.parent = parent;
                instances[i].name = ((Landmark)i).ToString();
                instances[i].layer = 6;
                instances[i].SetActive(false);
            }
            for (int i = 0; i < lines.Length; ++i)
            {
                lines[i] = Instantiate(linePrefab).GetComponent<LineRenderer>();
            }
            if (headPrefab)
            {
                /*GameObject head = Instantiate(headPrefab);
                head.transform.parent = instances[(int)Landmark.NOSE].transform;
                head.transform.localPosition = headPrefab.transform.position;
                head.transform.localRotation = headPrefab.transform.localRotation;
                head.transform.localScale = headPrefab.transform.localScale;*/
            }
        }
        public void UpdateLines()
        {
        /*    lines[0].positionCount = 4;
            lines[0].SetPosition(0, Position((Landmark)32));
            lines[0].SetPosition(1, Position((Landmark)30));
            lines[0].SetPosition(2, Position((Landmark)28));
            lines[0].SetPosition(3, Position((Landmark)32));
            lines[1].positionCount = 4;
            lines[1].SetPosition(0, Position((Landmark)31));
            lines[1].SetPosition(1, Position((Landmark)29));
            lines[1].SetPosition(2, Position((Landmark)27));
            lines[1].SetPosition(3, Position((Landmark)31));

            lines[2].positionCount = 3;
            lines[2].SetPosition(0, Position((Landmark)28));
            lines[2].SetPosition(1, Position((Landmark)26));
            lines[2].SetPosition(2, Position((Landmark)24));z
            lines[3].positionCount = 3;
            lines[3].SetPosition(0, Position((Landmark)27));
            lines[3].SetPosition(1, Position((Landmark)25));
            lines[3].SetPosition(2, Position((Landmark)23));

            lines[4].positionCount = 5;
            lines[4].SetPosition(0, Position((Landmark)24));
            lines[4].SetPosition(1, Position((Landmark)23));
            lines[4].SetPosition(2, Position((Landmark)11));
            lines[4].SetPosition(3, Position((Landmark)12));
            lines[4].SetPosition(4, Position((Landmark)24));

            lines[5].positionCount = 4;
            lines[5].SetPosition(0, Position((Landmark)12));
            lines[5].SetPosition(1, Position((Landmark)14));
            lines[5].SetPosition(2, Position((Landmark)16));
            lines[5].SetPosition(3, Position((Landmark)22));
            lines[6].positionCount = 4;
            lines[6].SetPosition(0, Position((Landmark)11));
            lines[6].SetPosition(1, Position((Landmark)13));
            lines[6].SetPosition(2, Position((Landmark)15));
            lines[6].SetPosition(3, Position((Landmark)21));

            lines[7].positionCount = 4;
            lines[7].SetPosition(0, Position((Landmark)16));
            lines[7].SetPosition(1, Position((Landmark)18));
            lines[7].SetPosition(2, Position((Landmark)20));
            lines[7].SetPosition(3, Position((Landmark)16));
            lines[8].positionCount = 4;
            lines[8].SetPosition(0, Position((Landmark)15));
            lines[8].SetPosition(1, Position((Landmark)17));
            lines[8].SetPosition(2, Position((Landmark)19));
            lines[8].SetPosition(3, Position((Landmark)15));

            lines[9].positionCount = 2;
            lines[9].SetPosition(0, Position((Landmark)10));
            lines[9].SetPosition(1, Position((Landmark)9));

lines[10].positionCount = 5;
            lines[10].SetPosition(0, Position((Landmark)8));
            lines[10].SetPosition(1, Position((Landmark)5));
            lines[10].SetPosition(2, Position((Landmark)0));
            lines[10].SetPosition(3, Position((Landmark)2));
            lines[10].SetPosition(4, Position((Landmark)7));*/

        }

        public float GetAngle(Landmark referenceFrom, Landmark referenceTo, Landmark from, Landmark to)
        {
            Vector3 reference = (instances[(int)referenceTo].transform.position - instances[(int)referenceFrom].transform.position).normalized;
            Vector3 direction = (instances[(int)to].transform.position - instances[(int)from].transform.position).normalized;
            return Vector3.SignedAngle(reference, direction, Vector3.Cross(reference, direction));
        }
        public float Distance(Landmark from,Landmark to)
        {
            return (instances[(int)from].transform.position - instances[(int)to].transform.position).magnitude;
        }
        public Vector3 LocalPosition(Landmark Mark)
        {
            return instances[(int)Mark].transform.localPosition;
        }
        public Vector3 Position(Landmark Mark)
        {
            return instances[(int)Mark].transform.position;
        }

    }

    private Body body;

    public int samplesForPose = 1;

    public bool active;

    private void Start()
    {
        body = new Body(lParent,landmarkPrefab,linePrefab,landmarkScale,enableHead?headPrefab:null);
        ActualBodyOffset = Hips.transform.position - ActualBody.transform.position;
        //OriginalHipsForward = Hips.forward;
        Thread t = new Thread(new ThreadStart(Run));
        t.Start();

    }
    private void Update()
    {
        UpdateBody(body);
    }

    private void UpdateBody(Body b)
    {
        for (int i = 0; i < LANDMARK_COUNT; ++i)
        {
            if (b.positionsBuffer[i].accumulatedValuesCount < samplesForPose)
                continue;
            // b.instances[i].transform.localPosition = b.positionsBuffer[i] / (float)b.samplesCounter * multiplier;
            b.localPositionTargets[i] = b.positionsBuffer[i].value / (float)b.positionsBuffer[i].accumulatedValuesCount * multiplier;
            b.positionsBuffer[i] = new AccumulatedBuffer(Vector3.zero,0);
        }

        for (int i = 0; i < LANDMARK_COUNT; ++i)
        {
            b.instances[i].transform.localPosition=Vector3.MoveTowards(b.instances[i].transform.localPosition, b.localPositionTargets[i], Time.deltaTime * maxSpeed);
        }
        b.UpdateLines();


        /*LeftShoulder.transform.position = b.Position(Landmark.RIGHT_SHOULDER);
        
        ShoulderToElbowDirection =   b.Position(Landmark.RIGHT_ELBOW) - b.Position(Landmark.RIGHT_SHOULDER);
        ShoulderToElbowDirection.Normalize();
        
        LeftShoulder.transform.rotation = Quaternion.LookRotation(ShoulderToElbowDirection,Vector3.up);
        LeftShoulderForward = LeftShoulder.transform.forward;
        LeftShoulder.transform.eulerAngles += ShoulderOffset;


        Vector3 ElbowToHandDirection = b.Position(Landmark.RIGHT_ELBOW) - b.Position(Landmark.RIGHT_WRIST);
        ElbowToHandDirection.Normalize();
        LeftElbow.transform.rotation = Quaternion.LookRotation(ElbowToHandDirection, Vector3.up);
        LeftElbow.transform.eulerAngles += ElbowOffset;

*/
        

        RepositionHip(b, Hips);
        RepositionNeck(b, Neck);
        UpdateBodyPart(b, LeftShoulder, Landmark.RIGHT_SHOULDER, Landmark.RIGHT_ELBOW, ShoulderOffsetRight);
        UpdateBodyPart(b, LeftElbow, Landmark.RIGHT_ELBOW, Landmark.RIGHT_WRIST, ElbowOffsetLeft);
        UpdateBodyPart(b, RightShoulder, Landmark.LEFT_SHOULDER, Landmark.LEFT_ELBOW, ShoulderOffsetLeft);
        UpdateBodyPart(b, RightElbow, Landmark.LEFT_ELBOW, Landmark.LEFT_WRIST, ElbowOffsetRight);
        /*UpdateBodyPart(b, LeftHip, Landmark.RIGHT_HIP, Landmark.RIGHT_KNEE, LeftKneeOffset);
        UpdateBodyPart(b, LeftKnee, Landmark.RIGHT_KNEE, Landmark.RIGHT_HEEL, KneeOffset);
        UpdateBodyPart(b, RightHip, Landmark.LEFT_HIP, Landmark.LEFT_KNEE, RightKneeOffset);
        UpdateBodyPart(b, RightKnee, Landmark.LEFT_KNEE, Landmark.LEFT_HEEL, KneeOffset);*/

/*        UpdateBodyPart(b, LeftHip, Landmark.LEFT_HIP, Landmark.LEFT_KNEE, LeftKneeOffset);
        UpdateBodyPart(b, LeftKnee, Landmark.LEFT_KNEE, Landmark.LEFT_HEEL, KneeOffset);
        UpdateBodyPart(b, RightHip, Landmark.RIGHT_HIP, Landmark.RIGHT_KNEE, RightKneeOffset);
        UpdateBodyPart(b, RightKnee, Landmark.RIGHT_KNEE, Landmark.RIGHT_HEEL, KneeOffset);*/
        //UpdateShoulderSpineRotation(b, Spine0);
        ActualBody.transform.position = Hips.transform.position - ActualBodyOffset;

        //float angle = Mathf.Rad2Deg * Mathf.Atan(HipForward.z / HipForward.x);
        //Hips.eulerAngles = new Vector3(Hips.eulerAngles.x, angle, Hips.eulerAngles.z);


    }

    public void UpdateBodyPart(Body b, Transform source, Landmark sourceLandmark, Landmark targetLandmark, Vector3 offsetRotation, bool updateSourcePosition = false)
    {
        if(updateSourcePosition)
        {
            source.transform.position = b.Position(sourceLandmark);
        }
        Vector3 direction = b.Position(targetLandmark) - b.Position(sourceLandmark);
        direction.Normalize();
        source.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        source.transform.eulerAngles += offsetRotation;
        if(!BodyPartGizmosMap.ContainsKey(source.name))
        {
            BodyPartGizmosMap.Add(source.name, new DictionaryMap());
        }
        BodyPartGizmosMap[source.name].origin = source.transform.position;
        BodyPartGizmosMap[source.name].direction = direction;
        BodyPartGizmosMap[source.name].up = Vector3.up;
    }
    public void UpdateShoulderSpineRotation(Body b, Transform source)
    {
        Vector3 leftShoulderPosition = b.Position(Landmark.LEFT_SHOULDER);
        Vector3 rightShoulderPosition = b.Position(Landmark.RIGHT_SHOULDER);
        Vector3 Center = (leftShoulderPosition + rightShoulderPosition) / 2.0f;
        Vector3 difference = b.Position(Landmark.RIGHT_SHOULDER) - b.Position(Landmark.LEFT_SHOULDER);
        difference.Normalize();
        source.forward = Vector3.Cross(-difference, Vector3.up);
    }
    public void RepositionHip(Body b, Transform source)
    {
        Vector3 leftHipPosition = b.Position(Landmark.LEFT_HIP);
        Vector3 rightHipPosition = b.Position(Landmark.RIGHT_HIP);
        Vector3 Center = (leftHipPosition + rightHipPosition) / 2.0f;
        Vector3 difference = b.Position(Landmark.RIGHT_HIP) - b.Position(Landmark.LEFT_HIP);
        //Hips.forward = Quaternion.Euler(0, -90, 0) * OriginalHipsForward;
        //HipForward = Hips.forward;
        Hips.forward = Vector3.Cross(-difference, Vector3.up);
        /*HipCenter = difference / 2.0f;
        difference.Normalize();
        HipForward = Vector3.Cross(-difference, Vector3.up);
        Hips.LookAt(HipForward, Vector3.up);*/
        source.transform.position = Center;

    }
    public void RepositionNeck(Body b, Transform source)
    {
        Vector3 leftShoulderLocation = b.Position(Landmark.LEFT_SHOULDER);
        Vector3 rightShoulderLocation = b.Position(Landmark.RIGHT_SHOULDER);
        Vector3 Center = (leftShoulderLocation + rightShoulderLocation) / 2.0f;
        source.transform.position = Center;
    }
    void Run()
    {
        // Open the named pipe.
        server = new NamedPipeServerStream("UnityMediaPipeBody",PipeDirection.InOut, 99, PipeTransmissionMode.Message);

        print("Waiting for connection...");
        server.WaitForConnection();

        print("Connected.");
        var br = new BinaryReader(server);

        while (true)
        {
            try
            {
                Body h = body;
                var len = (int)br.ReadUInt32();
                var str = new string(br.ReadChars(len));

                string[] lines = str.Split('\n');
                foreach (string l in lines)
                {
                    if (string.IsNullOrWhiteSpace(l))
                        continue;
                    string[] s = l.Split('|');
                    if (s.Length < 4) continue;
                    int i;
                    if (!int.TryParse(s[0], out i)) continue;
                    h.positionsBuffer[i].value += new Vector3(float.Parse(s[1]), float.Parse(s[2]), float.Parse(s[3]));
                    h.positionsBuffer[i].accumulatedValuesCount += 1;
                    h.active = true;
                }
            }
            catch (EndOfStreamException)
            {
                break;                    // When client disconnects
            }
        }

    }
    public void OnDrawGizmos()
    {

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(HipCenter, HipCenter + HipForward * 10);
        foreach(KeyValuePair<String, DictionaryMap> entry in BodyPartGizmosMap)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(entry.Value.origin, entry.Value.direction);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(entry.Value.origin, entry.Value.up);
        }
    }
    private void OnDisable()
    {
        print("Client disconnected.");
        server.Close();
        server.Dispose();
    }
}
