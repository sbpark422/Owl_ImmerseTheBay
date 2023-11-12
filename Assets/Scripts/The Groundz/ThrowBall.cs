using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThrowBall : AIState
{

    bool inAction;
    GameObject gameObject;
    GameManager gameManager;


    AI ai;

    public float vertInput;
    public float horzInput;
    public bool jumpInput;

    public bool action1Input;
    public bool rTriggerInput;
    public bool superInput;
    public bool blockInput;
    public bool ballGrabbed;

    int num = 2;
    string name = "ThrowBall";

    float completionPercentage;


    public void Start(GameManager manager, AI ai_)
    {
        ai = ai_;
        gameObject = ai.gameObject;
        gameManager = manager;

    }


    public void Update(GameManager manager, AI _ai)
    {
        ai.EvaluateGameState();

        if (ai.type == AI.Type.timid)
        {
            TimidBehavior(manager, _ai);
        }

        if (ai.type == AI.Type.aggresive)
        {
            AggressiveBehavior(manager, _ai);
        }

        if (ai.type == AI.Type.random)
        {
            RandomBehavior();
        }

    }

    #region Behaviors
    private void TimidBehavior(GameManager manager, AI ai)
    {
        
            if (ai.gameState == AI.GameState.safe)
            {
                if (inAction)
                {
                    Action(manager, ai, 2, Vector3.zero);
                }

                else
                {
                    ai.rTriggerInput = false;
                    ai.SetState(ai.getBall_);
                }
            }
            if (ai.gameState == AI.GameState.mildly_safe)
            {
                if (inAction)
                {
                    Action(manager, ai, 1, Vector3.zero);
                }
                else
                {
                    ai.rTriggerInput = false;
                    ai.SetState(ai.getBall_);
                }
            }

            if (ai.gameState == AI.GameState.mild)
            {
                if (inAction)
                {
                    Action(manager, ai, 0, Vector3.zero);  // <--- Should try wait code here too
                }
                else
                {
                    ai.rTriggerInput = false;
                    ai.SetState(ai.getBall_);
                }
            }

            if (ai.gameState == AI.GameState.mildly_dangerous)
            {
            if (inAction && completionPercentage < .25f)
            {
                Action(manager, ai, 0, Vector3.zero);  // <--- Should try wait code here too 
            }
            else
            {
                ai.SetState(ai.panic_);
            }

            }

            if (ai.gameState == AI.GameState.dangerous)
            {
            if (inAction && completionPercentage < .125f)
            {
                Action(manager, ai, 0, Vector3.zero);  // <--- Should try wait code here too 
            }
            else
            {
                ai.SetState(ai.panic_); 
            }
            }
    }

    private void AggressiveBehavior(GameManager manager, AI ai)
    {
        if (ai.ballGrabbed)
        {
            Action(manager, ai, 3, Vector3.zero);
        }
        else
        {
            ai.rTriggerInput = false;
            ai.SetState(ai.getBall_);

        }
    }

    void RandomBehavior()
    {

        float randomInt = UnityEngine.Random.Range(0, 3f);

        if (randomInt < 1 && randomInt > 0)
        {
            if (ai.ballGrabbed)
            {
                Action(gameManager, ai, 2, Vector3.zero);
            }
            else
            {
                ai.SetState(ai.getBall_);
            }
        }

        if (randomInt > 1 && randomInt < 2)
        {
            ai.SetState(ai.idle_);
        }

        if (randomInt > 2 && randomInt < 3)
        {
            ai.SetState(ai.panic_);
        }


    }

    #endregion

    public void Action(GameManager manager, AI _ai, float urgency, Vector3 target)
    {
            ai = _ai;
            Vector3 pos = ai.navMeshAgent.gameObject.transform.position;

            inAction = true;
            float proximity = 50 - 10*urgency;
        float distanceFromNearestOpp = Vector3.Distance(pos, GetNearestOpp(manager, ai));

        completionPercentage = proximity / distanceFromNearestOpp;

            if (distanceFromNearestOpp < proximity || IsNearHalfCourt(pos))
            {
                FaceOpp();
                ai.rTriggerInput = true;
            completionPercentage = 0;
           // Debug.Log("AI Throwing");

            if (ai.ballGrabbed == false)
            {
                inAction = false;
               // ai.SetNavVelocity(Vector3.zero);
            }

            }
            else
            {
                MoveTowardsOpp(manager, ai);
             //   Debug.Log("Moving to Player");
            }
        }

    private bool IsNearHalfCourt(Vector3 pos)
    {
       if (Math.Abs(pos.x- gameManager.levelManager.stage.halfCourtLine) < 5f)
        {
            return true;
        }

        return false;
    }

    private void FaceOpp()
    {
        bool isFacingRight = !ai.spriteRenderer.flipX;

       if (ai.playerScript.team == 1 && !isFacingRight)
        {
            ai.SpriteFlip();
        }
       else
        {
            if (ai.playerScript.team == 2 && isFacingRight)
            {
                ai.SpriteFlip();
            }
        }
    }

    private Vector3 GetNearestOpp(GameManager manager,AI _ai )
    {
        LevelManager lm = manager.levelManager;
        ai = _ai;
        Vector3 pos = ai.navMeshAgent.gameObject.transform.position;
        Vector3 move;
        float min = 10000000f;
        GameObject closestOpp = null;

        if (ai.transform.GetComponentInParent<Player>().team == 1)
        {
            foreach (GameObject opp in lm.tm2.players)
            {
                if (Vector3.Distance(opp.transform.GetChild(0).position,pos) < min)
                {
                    closestOpp = opp;
                }
            }
        }
        else
        {
            foreach (GameObject opp in lm.tm1.players)
            {
                if (Vector3.Distance(opp.transform.GetChild(0).position, pos) < min)
                {
                    closestOpp = opp;
                }
            }
        }
        return closestOpp.transform.GetChild(0).position;
    }
    private void MoveTowardsOpp(GameManager manager, AI _ai)
    {
        ai = _ai;
        Vector3 pos = ai.navMeshAgent.gameObject.transform.position;
        Vector3 nearestOppPos = GetNearestOpp( manager, ai);
        Vector3 distanceVec = (nearestOppPos - pos).normalized;
        float aiXVelocity;
        aiXVelocity = distanceVec.x * ai.xSpeed;    //  * Mathf.Clamp(ai.navSpeed,1f,12f)
        float aiZVelocity;
        aiZVelocity = distanceVec.z * ai.zSpeed;

        
        ai.SetNavVelocity(new Vector3(aiXVelocity , 0f, aiZVelocity ));           // *arbitrary nums

       // Debug.Log("aiVelocity= " + ai.navMeshAgent.velocity);
       // Debug.Log("aiXVelocity= " + aiXVelocity);
        //Debug.Log("aiZVelocity= " + aiZVelocity);

    }

    int AIState.GetNum()
    {
        return num;
    }

    string AIState.GetName()
    {
       // Debug.Log("Returning " + name);
        return name;
    }

    public void SetInAction(bool x)
    {
        inAction = x;
    }
}
