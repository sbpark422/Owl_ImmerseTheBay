using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ready : AIState
{
    AI ai;
    GameObject gameObject;
    AI.Type type;
    bool inAction;
    float tF;
    float t0;
    float idle_time;

    int num = 3;
    string name = "Ready";

    bool isReady = true;
    float isReadyCool = 3; // sec
    float iRC_t0;
    float iRC_tF;
    bool resetting;



    public void Start(GameManager manager, AI ai_)
    {
        ai = ai_;
        gameObject = ai.gameObject;

    }
    public void Update(GameManager manager, AI ai)
    {
        // gotta check if player has ball or balls been thrown
        // gotta check if we caught before proceeding to next state

        ai.EvaluateGameState();

            if (inAction || isReady)
            {
                 if (ai.gameState == AI.GameState.safe)
                {
                    Action(manager, ai, 1, Vector3.zero);
                }
                if (ai.gameState == AI.GameState.mildly_safe)
                {
                    Action(manager, ai, 2, Vector3.zero);
                }
                if (ai.gameState == AI.GameState.mild)
                {
                    Action(manager, ai, 3, Vector3.zero);
                }
                if (ai.gameState == AI.GameState.mildly_dangerous)
                {
                    Action(manager, ai, 4, Vector3.zero);
                }
                if (ai.gameState == AI.GameState.dangerous)
                {
                    Action(manager, ai, 5, Vector3.zero);
                }
            }

        else
            {
            if (ai.type == AI.Type.timid)
            {
                if (ai.gameState == AI.GameState.safe)
                {
                    ai.SetState(ai.getBall_);
                }
                if (ai.gameState == AI.GameState.mildly_safe)
                {
                    ai.SetState(ai.getBall_);
                }
                if (ai.gameState == AI.GameState.mild)
                {
                    ai.SetState(ai.idle_);
                }
                if (ai.gameState == AI.GameState.mildly_dangerous)
                {
                    ai.SetState(ai.retreat_);
                }
                if (ai.gameState == AI.GameState.dangerous)
                {
                    ai.SetState(ai.panic_);
                }
            }

            if (!isReady)
            {
                ResetIsReady();
            }


            }
    }

    private void ResetIsReady()
    {
       if (!resetting)
        {
            iRC_t0 = Time.realtimeSinceStartup;
            resetting = true;
        }

        iRC_tF = Time.realtimeSinceStartup;

        if (iRC_tF - iRC_t0 >= isReadyCool)
        {
            isReady = true;
            resetting = false;
        }
    }

    public void Action(GameManager manager, AI ai, float dur, Vector3 target)
    {
        dur *= 3.5f;

        if (!inAction)                   // initial case
        {
            t0 = Time.realtimeSinceStartup;
            inAction = true;

        }
      
        if (inAction)
        {
            tF = Time.realtimeSinceStartup;

            if (tF - t0 < dur)
            {
                ai.action1Input = true;
            }
            else
            {
                isReady = false;
                inAction = false;
            }
        }

    }

    int AIState.GetNum()
    {
        return num;
    }
    string AIState.GetName()
    {
      //  Debug.Log("Returning " + name);
        return name;
    }

    public void SetInAction(bool x)
    {
        inAction = x;
    }

}