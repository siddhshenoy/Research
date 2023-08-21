using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Python.Runtime;
using UnityEditor.Scripting.Python;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

public class PythonScriptRunner : MonoBehaviour
{
    [Header("BoneData")]
    public Transform[] BoneData;
    [Header("Debug")]
    public bool ForceUpdate = true;
    public bool AlwaysRun = true;
    public void Start()
    {
        
    }
    //
    public void Update()
    {
        
        string target_file = $"{Application.dataPath}/params.out";
        if (File.Exists(target_file))
        {
            File.Delete(target_file);
        }
        FileStream writeFile = File.Create(target_file);
        StreamWriter writer = new StreamWriter(writeFile);
        string result = "";
        for (int i = 0; i < BoneData.Length; i++)
        { 
            result += BoneData[i].position.normalized.x + " " + BoneData[i].position.normalized.y + " " + BoneData[i].position.normalized.z + " ";
        }
        result = result.Substring(0, result.Length - 2);
        writer.Write(result);
        writer.Flush();
        writer.Close();
        writeFile.Close();
        PythonRunner.RunFile($"{Application.dataPath}/test_script.py");
       
    }
}
