using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissClick : MonoBehaviour
{
    public static int missedPVC = 0;
    public int testresult = 1;
    public GameObject testforstartmenu;
    public GameObject missedclick;
    public GameObject buffawarded;

    // Use this for initialization
    void Start()
    {
   //     missedclick = GameObject.Find("BuffNotAwarded!");
     //   missedclick.gameObject.SetActive(false);
    }
    
    void OnMouseDown()
    {
        testforstartmenu = GameObject.Find("StartButton");
        if (testforstartmenu == true)
        {
            return;
        }
        else
        {
            testresult = 1;
        }

        buffawarded = GameObject.Find("BuffAwarded");
        if (buffawarded == true)
        {
            return;
        }
        if (testresult == 1) 
        {
            gameObject.SetActive(false);
            missedclick.gameObject.SetActive(true);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
    
    }
}