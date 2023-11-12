using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : AIState {

    AI ai;
	GameObject gameObject;
	AI.Type type;
    bool inAction;
    float tF;
    float t0;
    float idle_time;

    float dur = 0f;

    bool isWaiting;

    int num = 0;
    string name = "Idle";
    GameManager gameManager;

    float completionPercentage;

    public void Start(GameManager manager, AI ai_){
        ai = ai_;
        gameObject = ai.gameObject;
        gameManager = manager;
	}


    public void Update(GameManager manager, AI ai)     
    { 

        ai.EvaluateGameState();

            if (ai.type == AI.Type.timid)
            {
                TimidBehavior();
            }

            if (ai.type == AI.Type.aggresive)
            {
                AggressiveBehavior();
            }

            if (ai.type == AI.Type.random)
            {
               RandomBehavior();
            }
    }

	public void Action(GameManager manager, AI ai, float durIn, Vector3 target){

        //Debug.Log("Idling");
        CheckFace();
        inAction  = true;
		ai.vertInput = 0f;
		ai.horzInput = 0f;
        ai.navMeshAgent.isStopped = true;


        if (t0 == 0)    // first time catch
        {
            t0 = Time.realtimeSinceStartup;
            dur = durIn;
        }

        if (!inAction)
        {
          
            t0 = Time.realtimeSinceStartup;
            idle_time = 0.0f;
        }

        else
        {
             tF = Time.realtimeSinceStartup;
             idle_time = tF - t0;
            completionPercentage = idle_time / dur;

            if (idle_time >= dur)
            {
                ai.navMeshAgent.isStopped = false;
                inAction = false;
                t0 = 0;
                completionPercentage = 0;
            }
        }
	}

    #region Behaviors
    void TimidBehavior()
    {

        if (ai.gameState == AI.GameState.safe)
        {
            if (inAction)
            {
                Action(gameManager, ai, 4.0f, Vector3.zero);
            }
            else
            {
                if (ai.ballGrabbed)
                {
                    ai.SetState(ai.throwBall_);
                }
                else
                {
                    if (ai.GetNearestBall())
                    {
                        ai.SetState(ai.getBall_);
                    }
                    else
                    {
                        Action(gameManager, ai, 4.0f, Vector3.zero);
                    }
                   
                }
            }

        }

        if (ai.gameState == AI.GameState.mildly_safe)
        {
            if (inAction)
            {
                Action(gameManager, ai, 3.0f, Vector3.zero);
            }
            else
            {
                if (ai.ballGrabbed)
                {
                    ai.SetState(ai.throwBall_);
                }
                else
                {
                    if (ai.GetNearestBall())
                    {
                        ai.SetState(ai.getBall_);
                    }
                    else
                    {
                        Action(gameManager, ai, 3.0f, Vector3.zero);
                    }

                }
            }
        }

        if (ai.gameState == AI.GameState.mild)
        {
            if (inAction /* || isWaiting */)
            {
                Action(gameManager, ai, 2.0f, Vector3.zero);
            }
            else
            {
                if (ai.ballGrabbed)
                {
                   // WaitAndDoTask(1.0f, ai.throwBall_.GetName());
                    ai.SetState(ai.throwBall_);
                }
                else
                {
                    if (ai.GetNearestBall())
                    {
                        // WaitAndDoTask(1.0f, ai.getBall_.GetName());
                        ai.SetState(ai.getBall_);
                    }
                    else
                    {
                        Action(gameManager, ai, 2.0f, Vector3.zero);
                    }

                }
            }
        }

        if (ai.gameState == AI.GameState.mildly_dangerous)
        {
            if (inAction && completionPercentage < .25f)
            {
                Action(gameManager, ai, 1.0f, Vector3.zero);
            }
            else
            {
                if (ai.ballGrabbed) {
                    ai.SetState(ai.panic_);
                }
                else
                {
                    ai.SetState(ai.retreat_);
                }
                
            }
        }

        if (ai.gameState == AI.GameState.dangerous)
        {
            if (inAction && completionPercentage < .125f)
            {
                Action(gameManager, ai, 0.0f, Vector3.zero);
            }
            else
            {
                ai.SetState(ai.panic_);
            }
        }
    }

    void AggressiveBehavior()
    {
        Vector3 pos = ai.navMeshAgent.gameObject.transform.position;
        GameObject nearestBall = GetNearestBall(pos, gameManager);

            if (ai.ballGrabbed)
            {
                ai.SetState(ai.throwBall_);
            }
            else
            {
            if (nearestBall)
            {
                ai.SetState(ai.getBall_);
            }
            else
            {
                ai.SetState(ai.panic_); // or Shake
            }
            }   
    }

    void RandomBehavior()
    {
        float randomInt = UnityEngine.Random.Range(0, 3f);

        if (randomInt < 1 && randomInt > 0)
        {
            if (ai.ballGrabbed)
            {
                ai.SetState(ai.throwBall_);
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


    private void CheckFace()
    {
        ai.FaceOpp();
        
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

    GameObject GetNearestBall(Vector3 pos, GameManager manager)
    {


        float min = 10000000f;
        GameObject nearestBall = null;
        int team = gameObject.GetComponentInParent<Player>().team;
        float halfCourt = manager.levelManager.stage.halfCourtLine;

        LevelManager lm = manager.levelManager;

        // Debug.Log("team :" + team);

        if (team == 1)
        {
            foreach (GameObject ball in lm.balls)
            {
                Ball ballScript = ball.GetComponent<Ball>();
                if (ballScript.grounded && !ballScript.grabbed && ball.transform.position.x <= halfCourt - .5)
                {                 // gameRule config
                    if (Vector3.Distance(pos, ball.transform.position) < min)
                    {
                        if (!ballScript.isSupering)
                        {
                            min = Vector3.Distance(pos, ball.transform.position);
                            nearestBall = ball;
                            return nearestBall;
                        }
                    }
                }
            }
        }
        if (team == 2)
        {
            foreach (GameObject ball in lm.balls)
            {
                Ball ballScript = ball.GetComponent<Ball>();

                if (ballScript.grounded && !ballScript.grabbed && ball.transform.position.x >= halfCourt + .5)               // gameRule config
                {
                    if (Vector3.Distance(pos, ball.transform.position) < min)
                    {
                        if (!ballScript.isSupering)
                        {
                            min = Vector3.Distance(pos, ball.transform.position);
                            nearestBall = ball;
                            return nearestBall;
                        }
                    }
                }
            }
        }

        // Debug.Log(" null balls");
        return nearestBall;
    }

    public void SetInAction(bool x)
    {
        inAction = x;
    }

    IEnumerator WaitAndDoTask(float duration, string taskName)
    {
        isWaiting = true;
        yield return new WaitForSeconds(duration);
        Debug.Log("Coroutine ended: " + Time.time + " seconds");
    }
}