using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data
{

    private static bool isDemo;

    public static bool IsDemo
    {
        get
        {
            return isDemo;
        }
        set
        {
            isDemo = value;
        }
    }
}
