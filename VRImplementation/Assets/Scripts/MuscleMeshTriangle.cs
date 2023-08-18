using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuscleMeshTriangle 
{
    public Vector3[] VertexList = new Vector3[3];
    public List<int> IndexList = new List<int>();

    public MuscleMeshTriangle()
    {
        for (int i = 0; i < VertexList.Length; i++) VertexList[i] = Vector3.zero;
    }
    public MuscleMeshTriangle(int[] indices)
    {
        for (int i = 0; i < VertexList.Length; i++) VertexList[i] = Vector3.zero;
        for(int i = 0; i < indices.Length; i++)
        {
            IndexList.Add(indices[i]);
        }
    }
    public void AssignVertexIndices(int[] indices)
    {
        for (int i = 0; i < indices.Length; i++)
        {
            IndexList.Add(indices[i]);
        }
    }
    public void AddIndex(int indice)
    {
        IndexList.Add(indice);
    }
    public float CalculateArea(Mesh mesh)
    {
        for(int i = 0; i < IndexList.Count; i++)
        {
            VertexList[i] = mesh.vertices[IndexList[i]];
        }
        float side0 = Vector3.Distance(VertexList[1], VertexList[0]);
        float side1 = Vector3.Distance(VertexList[2], VertexList[0]);
        float side2 = Vector3.Distance(VertexList[1], VertexList[2]);

        float half_p = (side0 + side1 + side2) / 2;
        return Mathf.Sqrt(half_p * (half_p - side0) * (half_p - side1) * (half_p - side2)) * 10000;

    }
}
