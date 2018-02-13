using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data
{

    private static bool isDemo;
    private static int realPlayers;

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

    public static int RealPlayers {
        get {
            return realPlayers;
        }
        set {
            realPlayers = value;
        }
    }
}
