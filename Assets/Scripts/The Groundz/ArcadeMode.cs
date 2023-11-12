using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeMode : MonoBehaviour
{
    int level;
    LevelManager lm;
    // scenes visited
    List<string> oppsFaced = new List<string>();

    public float diffFactor = .5f;

    bool completed;

    string currentOppName;

    void Start()
    {
        
    }

     public ArcadeMode(LevelManager x)
    {
        lm = x;
        GameRule gr = lm.GetGameRule();
        gr.ballCount = Mathf.Clamp(3 + level,3, 6);
        gr.roundsToWin = Mathf.Clamp(3 + level, 3, 6);
        // Controller3D.hasGrabMag = false;
       // Controller3D.grabMag = 10f;
        Controller3D.hasThrowMag = false;
        Controller3D.hasSeekVec = false;
       // Controller3D.throwMagnetism = 5.65f;
        Controller3D.maxSeekVec = 100f;
}
    void Update()
    {
        
    }

    internal void levelUp()
    {
        level++;
        GameRule gr = lm.GetGameRule();
        gr.ballCount = Mathf.Clamp(3 + level, 3, 6);
        gr.roundsToWin = Mathf.Clamp(3 + level, 3, 6);
        currentOppName = GetNextOpp();

        if (level == 1)
        {
            float throwMag = Controller3D.throwMagnetism;
            Controller3D.throwMagnetism = throwMag / 2f;
        }

        if (level == 2)
        {
            Controller3D.hasThrowMag = false;

        }

        if (level == 3)
        {
            Controller3D.hasSeekVec = false;
        }

    }

    public string GetScene()
    {
        if (level == 0)
        {
            return "TheGroundzEast";
        }
        if (level == 1)
        {
            return "TheGym";
        }

        if (level == 2)
        {
            return "TheBlock";
        }

        if (level == 3)
        {
            return "TheGroundzWest";
        }

        return "TheGroundzEast";
    }

    internal int GetSceneIndex()
    {
        if (level == 0)
        {
            return 5;
        }
        if (level == 1)
        {
            return 6;
        }

        if (level == 2)
        {
            return 7;
        }

        if (level == 3)
        {
            return 8;
        }

        return 5;
    }

    public void AddOppCharacter(string oppChar)
    {
        oppsFaced.Add(oppChar);
    }

    public void SetCompleted( bool x)
    {
        completed = x;

        if (completed)
        {
            level = 0;
            oppsFaced.Clear();

        }
    }

    public bool GetCompleted()
    {
        if (level+1 >= 4)
        {
            completed = true;
        }

        return completed;
    }



    private string GetNextOpp()
    {
        


        return "";
    }

    public void SetCurrentOpp(string oppName)
    {
        currentOppName = oppName;
    }

}
