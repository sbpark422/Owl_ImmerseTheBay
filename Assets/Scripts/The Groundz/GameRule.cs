using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameRule            // abstract ... Basic
{

    private string gameMode;

    int team1Count;
    int team2Count;

    public int ballCount = 1;

    public int roundsToWin;

    bool hasTimer;


    bool hasVFX;
    bool hasAIIntensity;

    int playerHealthStockCount;

    bool hasHalfCourtLine;
    bool hasCatchToGetIn;

    bool hasSlowDownAssist;
    bool hasGrabMag;
    bool hasThrowAssist;

    bool hasRandomBall;

    bool hasStaminia;

    public float throwStaminaCost;           
    public float moveStaminaCost;
    public float catchStaminaCost;
    public float pickUpStaminaCost;
    public float staminaDodgeCost = 10.0f;
    public float staminaReadyCost = 25.0f;

    bool hasScreenShake;
    bool hasHitPause;


    void Start()
    {

    }
     /*
    public GameRule(string gameMode)
    {
        this.gameMode = gameMode;

        if (gameMode == "local")
        {

        }
    }
     */

    internal int GetTeamCount(int v)
    {
        if (v == 1)
        {
            return team1Count;
        }

        if (v == 2)
        {
            return team2Count;
        }

        return 0;
    }

    public void SetTeamCount( int team, int count)
    {
        if (team == 1)
        {
            team1Count = count;
        }

        if (team == 2)
        {
            team2Count = count;
        }
    }

    public void SetRoundsToWin(int x)
    {
        roundsToWin = x;
    }

}
