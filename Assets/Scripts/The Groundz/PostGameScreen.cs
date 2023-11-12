using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostGameScreen : MonoBehaviour
{
    public GameObject freePlayDefeatObj;
    public GameObject freePlayWinObj;
    public GameObject arcadeDefeatObj;
    public GameObject arcadeWinObj;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FreePlayWin(string charName)
    {
        
    }

    public void FreePlayDefeat(string charName)
    {
        
    }

    public void arcadeWin(string charName)
    {
       // print(charName + " wins");

       
    }

    public void arcadeDefeat(string charName)
    {
        
    }

    internal void SelectRestartArcadeDefeatButton()
    {
        Button restartButton = arcadeDefeatObj.transform.GetChild(1).GetComponent<Button>();
        restartButton.Select();

    }
}
