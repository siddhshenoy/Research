using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class BoneRendererSetup : MonoBehaviour
{
    public BoneRenderer Renderer;
    public Transform RootBone;
    public List<Transform> BoneList;
    // Start is called before the first frame update
    void Start()
    {
        Renderer = this.GetComponent<BoneRenderer>();
        //Renderer.transforms
        /*if (RootBone != null)
        {
            PopulateBoneList(RootBone);
            Renderer.transforms = BoneList.ToArray();
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PopulateBoneList(Transform root)
    {
        if(root != null)
        {
            BoneList.Add(root);
            if(root.childCount > 0)
            {
                for(int i = 0; i < root.childCount; i++)
                {
                    PopulateBoneList(root.GetChild(i));
                }
            }
        }
    }

}
