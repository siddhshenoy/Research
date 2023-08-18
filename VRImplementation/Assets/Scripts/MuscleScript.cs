using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleScript : MonoBehaviour
{


    public Material ActivityMaterial;
    public Material DormantMaterial;

    public SkinnedMeshRenderer skin_renderer;
    public MeshFilter filter;
    public Mesh mesh;

    [Header("Mesh Data")]
    
    private Vector3[] OriginalVertexData;
    public Vector3[] VertexData;
    public Transform SkinnedTransform;

    public Mesh newMesh;

    [Header("Original Mesh Data")]
    public Vector3 OriginalCenter;
    public float[] OriginalCenterDistances;

    [Header("Modified Mesh Data")]
    public Vector3 ModifiedCenter;
    public float[] ModifiedCenterDistances;

    public float[] Differences;

    public int TotalTriangles;

    public float[] TriangleArea;
    public int TotalVertices;
    public int MaxChangedVertices;

    public List<MuscleMeshTriangle> triangle_list = new List<MuscleMeshTriangle>();
    public Color OriginalColor;
    public Vector4 OriginalColorVec4;
    public Color ModifiedColor;
    public Vector4 ModifiedColorVec4;
    // Start is called before the first frame update
    void Start()
    {
        skin_renderer = this.GetComponent<SkinnedMeshRenderer>();
        
        filter = this.GetComponent<MeshFilter>();
        //mesh = filter.mesh;
        mesh = skin_renderer.sharedMesh;

        Debug.Log("Mesh name: " + mesh.name);
        Debug.Log("Total Vertices: " + mesh.vertices.Length);
        OriginalVertexData = mesh.vertices;
        VertexData = mesh.vertices;
        SkinnedTransform = skin_renderer.transform;
        newMesh = new Mesh();
        skin_renderer.BakeMesh(newMesh, true);
        VertexData = newMesh.vertices;
        for(int i = 0; i < VertexData.Length; i++)
        {
            OriginalCenter += VertexData[i];
        }
        OriginalCenter /= VertexData.Length;
        OriginalCenterDistances = new float[VertexData.Length];
        ModifiedCenterDistances = new float[VertexData.Length];
        Differences = new float[VertexData.Length];
        for(int i = 0; i < VertexData.Length; i++)
        {
            OriginalCenterDistances[i] = Vector3.Distance(OriginalCenter, VertexData[i]);
        }
        TotalVertices = VertexData.Length;

        TotalTriangles = mesh.triangles.Length / 3;
        TriangleArea = new float[TotalTriangles];
        for (int i = 0; i < TotalTriangles; i++)
        {
            triangle_list.Add(new MuscleMeshTriangle());
        }
        /*for(int i = 0; i < triangle_list.Count; i++)
        {
            for(int j = 3 * i; j < (3 * i + 3); j++)
            {
                triangle_list[i].AddIndex(mesh.triangles[j]);
            }
        }*/
        OriginalColor = skin_renderer.materials[0].color;
    }

    // Update is called once per frame
    void Update()
    {
        MaxChangedVertices = 0;
        skin_renderer.BakeMesh(newMesh, true);
        VertexData = newMesh.vertices;
        ModifiedCenter = Vector3.zero;
        for (int i = 0; i < VertexData.Length; i++)
        {
            ModifiedCenter += VertexData[i];
        }
        ModifiedCenter /= VertexData.Length;
        for (int i = 0; i < VertexData.Length; i++)
        {
            ModifiedCenterDistances[i] = Vector3.Distance(ModifiedCenter, VertexData[i]);
            Differences[i] = ModifiedCenterDistances[i] - OriginalCenterDistances[i];
            if(Mathf.Abs(Differences[i]) > 0.0015f)
            {
                MaxChangedVertices++;
            }
        }
        OriginalColorVec4 = new Vector4(OriginalColor.r, OriginalColor.g, OriginalColor.b, OriginalColor.a);
        if (MaxChangedVertices > Percentage(TotalVertices, 85))
        {
            skin_renderer.materials[0].color =
            new Color(
                Mathf.Lerp(OriginalColor.r, 1.0f, ((float)MaxChangedVertices / (float)TotalVertices)),
                //Mathf.Lerp(OriginalColor.r, 1.0f, ((float)MaxChangedVertices / (float)TotalVertices)),
                0.0f,
                //OriginalColor.g,
                //Mathf.Lerp(1.0f, 0.0f, ((float)MaxChangedVertices / (float)TotalVertices)),
                0.0f
                //OriginalColor.b
               //Mathf.Lerp(OriginalColor.b, 0.0f, ((float)MaxChangedVertices / (float)TotalVertices))
               );
            ModifiedColor = skin_renderer.materials[0].color;
            ModifiedColorVec4 = new Vector4(ModifiedColor.r, ModifiedColor.g, ModifiedColor.b, ModifiedColor.a);
        }
        else
        {
            skin_renderer.materials[0].color = Color.yellow;
            //skin_renderer.materials[0].color = OriginalColor;
        }
/*        if(MaxChangedVertices > Percentage(TotalVertices, 75))//TotalVertices / 2)
        {
            //skin_renderer.material = ActivityMaterial;
            skin_renderer.materials[0].color = Color.yellow;
        }
        else
        {
            //skin_renderer.material = DormantMaterial;
            skin_renderer.materials[0].color = Color.red;
            //skin_renderer.materials[0] = DormantMaterial;
        }*/
        /*        for(int i = 0; i < triangle_list.Count; i++)
                {
                    TriangleArea[i] = triangle_list[i].CalculateArea(newMesh);
                }*/


    }
    public int Percentage(int number, float percentage)
    {
        return (int)(((float)number * percentage) / 100.0f);
    }
    public void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(this.transform.position + this.ModifiedCenter, 0.01f);
    }
}
