using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BaseBodyFunctionality : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform Player;
    public Transform TrainerObject;
    [Header("Debugging variables")]
    public bool UpdateOnce;
    public bool SimulateAnimationIndexUpdate = false;
    public int AnimationIndex = 0;
    public int MaxAnimationIndex = 2;
    public int MinAnimationIndex = 1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.Get(OVRInput.Button.One) || UpdateOnce)
        {
            this.transform.position = new Vector3(Player.position.x, this.transform.position.y, Player.position.z);
            TrainerObject.transform.position = new Vector3(Player.position.x, this.transform.position.y, Player.position.z);
            TrainerObject.transform.forward = this.transform.forward;
            if(UpdateOnce)
            {
                UpdateOnce = false;
            }
        }
        if(OVRInput.GetDown(OVRInput.Button.Two))
        {
            GameObject TrainerGameObject = TrainerObject.gameObject;
            if(TrainerGameObject.activeSelf)
            {
                TrainerGameObject.GetComponent<Animator>().enabled = false;
                TrainerGameObject.SetActive(false);
            }
            else
            {
                TrainerGameObject.SetActive(true);
                TrainerGameObject.GetComponent<Animator>().enabled = true;
            }
        }
        if(OVRInput.GetDown(OVRInput.Button.Three) || SimulateAnimationIndexUpdate)
        {
            GameObject TrainerGameObject = TrainerObject.gameObject;
            if (TrainerGameObject.activeSelf)
            {
                TrainerGameObject.GetComponent<Animator>().enabled = false;
                TrainerGameObject.SetActive(false);
                TrainerGameObject.SetActive(true);
                TrainerGameObject.GetComponent<Animator>().enabled = true;
                AnimationIndex = GetNextAnimationIndex(AnimationIndex);
                TrainerGameObject.GetComponent<Animator>().SetInteger("AnimationIndex", AnimationIndex);
            }
            if(SimulateAnimationIndexUpdate)
                SimulateAnimationIndexUpdate = false;
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
            writer.WriteLine("A,B");
            f.Close();

        }
    }
    public int GetNextAnimationIndex(int index)
    {
        int nextIdx = index + 1;
        if (nextIdx > MaxAnimationIndex) return MinAnimationIndex;
        return nextIdx;
    }
}
