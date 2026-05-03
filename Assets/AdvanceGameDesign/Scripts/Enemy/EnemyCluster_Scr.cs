using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Enemy_Cluster_Scr : MonoBehaviour
{

    bool player_Detected;

    List<GoblinAI> subjects = new List<GoblinAI>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in transform)
        {
            GoblinAI e = child.GetComponent<GoblinAI>();
            if (e != null)
            {
                subjects.Add(e);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void detect_Player()
    {
        foreach (GoblinAI subject in subjects)
        {
            subject.DetectPlayer();
        }
    }
}
