using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GetBall : AIState {

    // Use this for initialization
    bool inAction;

    AI ai;
    GameObject gameObject;


	public float vertInput;
	public float horzInput;
	public bool jumpInput;

	public bool action1Input;
	public bool rTriggerInput;
	public bool superInput;
	public bool blockInput;


    float counter;
    float t0;
    float tF;

    int num = 1;
    string name = "GetBall";

    float completionPercentage;
    float ballTargetDistance;
    GameObject ballTarget;

    GameManager gameManager;

    public void Start(GameManager manager, AI ai_)          // make gameManager
    {
        ai = ai_;
        gameObject = ai.gameObject;
        gameManager = manager;

    }


    public void Update(GameManager manager, AI ai)
    {

        if (!ai.playerConfigObject.GetComponent<PlayerConfiguration>().ballContact)
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
        
        else
        {
            
            ballTarget = GetNearestBallContacted(ai.navMeshAgent.gameObject.transform.position, manager);
            if (ballTarget)
            {

                float currentBallTargetDistance = Vector3.Distance(ballTarget.transform.position, ai.navMeshAgent.gameObject.transform.position);

                if (currentBallTargetDistance <= ai.grabRadius)
                {
                    //  Debug.Log("ActionInput");
                    ai.action1Input = true;


                    if (ai.ballGrabbed)
                    {
                        ballTarget.GetComponent<Ball>().isBeingPursued = false;
                        ballTarget = null;
                        ballTargetDistance = 0f;
                    }
                }

                else
                {
                    ballTarget.GetComponent<Ball>().isBeingPursued = true;
                    MoveTowardsTarget(ballTarget.transform.position);
                }

            }
       
            
        }

         

        //   Debug.Log("InAction = " + inAction);
    }

    public void Action(GameManager manager, AI ai, float urgency, Vector3 tar)                  // Make better
    {
          // Debug.Log("GetBall Action");
            inAction = true;

            Vector3 pos = ai.navMeshAgent.gameObject.transform.position;
        int count = 0;
        int team = ai.gameObject.GetComponentInParent<Player>().team;

        //  Debug.Log("urgency");

        if (!ballTarget)
        {
            ballTarget = GetNearestBall(pos, manager);
           
            if (ballTarget)
            {
                ai.SetAgentDestination(ballTarget.transform);
                ballTargetDistance = Vector3.Distance(ballTarget.transform.position, ai.navMeshAgent.gameObject.transform.position);
                ballTarget.GetComponent<Ball>().isBeingPursued = true;
            }
        }

        else
        {
           if ( !IsBallIsStillLegal(ballTarget)) {
                Debug.Log("Ball not legal anymore");
                ai.EndAgentNavigation();
                ballTarget.GetComponent<Ball>().isBeingPursued = false;
                ballTarget = GetNearestBall(pos, manager);

                if (ballTarget)
                {
                    if (ai.navMeshAgent.destination != ballTarget.transform.position)
                    {
                        ai.navMeshAgent.isStopped = false;
                        ai.SetAgentDestination(ballTarget.transform);
                        ballTarget.GetComponent<Ball>().isBeingPursued = true;
                    }
                }
            }
        }


        if (ballTarget != null)
            {

               // Debug.Log("nearestballPos = " + ballTarget.transform.position);
            // Debug.Log("Found Ball");

            float currentBallTargetDistance = Vector3.Distance(ballTarget.transform.position, ai.navMeshAgent.gameObject.transform.position);

                if (currentBallTargetDistance <= ai.grabRadius) 
                {
                   //  Debug.Log("ActionInput");
                    ai.action1Input = true;
                   

                    if (ai.ballGrabbed)
                    {
                    ballTarget.GetComponent<Ball>().isBeingPursued = false;
                        inAction = false;
                        ballTarget = null;
                        ballTargetDistance = 0f;
                    ai.EndAgentNavigation();
                }
                }
                else
                {
                    if (ai.navMeshAgent.isStopped)
                    {
                        ai.navMeshAgent.isStopped = false;
                   ;
         
                }

                ai.SetAgentDestination(ballTarget.transform);
                //Debug.Log("completionPercentage = " + completionPercentage);
                completionPercentage = (ballTargetDistance - currentBallTargetDistance) / ballTargetDistance;



                // Vector3 move = GetMoveTowards(pos, nearestBall);    // deprecated
                //    ai.horzInput = move.x;
                //    ai.vertInput = move.z;

                //  Debug.Log("Moving towards ball");
                // Debug.Log("GrabRadius = "+ ai.grabRadius);
                // Debug.Log("Distance = " + Vector3.Distance(nearestBall.transform.position, ai.navMeshAgent.gameObject.transform.position));

                // ai.navSpeed

            }
            }

            else
            {
              //    Debug.Log("No Ball found");
                inAction = false;
            ai.navMeshAgent.velocity = Vector3.zero;
            ai.EndAgentNavigation();

        }


    }
    
    private bool IsBallIsStillLegal(GameObject thisBallTarget)
    {
        if (ai.GetTeam() == 2)
        {
            if (thisBallTarget.transform.position.x > gameManager.levelManager.stage.halfCourtLine)
            {
                return true;
            }
        }
        else
        {
            if (thisBallTarget.transform.position.x < gameManager.levelManager.stage.halfCourtLine)
            {
                return true;
            }
        }

      

        return false;
    }

    #region Behaviors
    void TimidBehavior()
    {
        if (ai.gameState == AI.GameState.safe)
        {
            if (inAction)
            {
                Action(gameManager, ai, 3, Vector3.zero);
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
                        Action(gameManager, ai, 3, Vector3.zero);
                    }
                    else
                    {
                        ai.SetState(ai.idle_);
                    }
                }
            }
        }


        if (ai.gameState == AI.GameState.mildly_safe)
        {
            if (inAction && completionPercentage < .75f)
            {
                Action(gameManager, ai, 2, Vector3.zero);
            }
            else
            {
                if (ai.ballGrabbed)
                {
                    ai.SetState(ai.throwBall_);
                }
                if (ai.GetNearestBall())
                {
                    Action(gameManager, ai, 2, Vector3.zero);
                }
                else
                {
                    ai.SetState(ai.idle_);
                }
            }
        
        }

        if (ai.gameState == AI.GameState.mild)
        {
            if (inAction && completionPercentage < .5f)
            {
                Action(gameManager, ai, 1, Vector3.zero);
            }

            else
            {
                if (ai.ballGrabbed)
                {
                    ai.SetState(ai.throwBall_);     // <--- Should try wait code here too
                }
                if (ai.GetNearestBall())
                {
                    Action(gameManager, ai, 1, Vector3.zero);
                }
                else
                {
                    ai.SetState(ai.idle_);
                }
            }
            }

            if (ai.gameState == AI.GameState.mildly_dangerous )
        {
            if (inAction && completionPercentage < .25f)
            {
                Action(gameManager, ai, 0, Vector3.zero);
            }

            else
            {
                if (ai.ballGrabbed)
                {
                    ai.SetState(ai.retreat_);     // <--- Should try wait code here too
                }
                if (ai.GetNearestBall())
                {
                    Action(gameManager, ai, 0, Vector3.zero); // <--- Should try wait code here too
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
                Action(gameManager, ai, 0, Vector3.zero);
            }

            else
            {
                ai.SetState(ai.panic_);
            }

        }
    }

    void AggressiveBehavior ()
    {
        Vector3 pos = ai.navMeshAgent.gameObject.transform.position;

        if (ai.ballGrabbed)
        {
            ai.SetState(ai.throwBall_);
        }
        else
        {
            if (GetNearestBall(pos, gameManager))
            {
                Action(gameManager, ai, 2, Vector3.zero);
            }
            else
            {
                ai.SetState(ai.idle_); // or run pattern
            }
            
        }
    }

    void RandomBehavior()
    {
        Vector3 pos = ai.navMeshAgent.gameObject.transform.position;

        float randomInt = UnityEngine.Random.Range(0, 3f);

        if (randomInt < 1 && randomInt > 0)
        {
            if (GetNearestBall(pos, gameManager))
            {
                ai.SetState(ai.throwBall_);
            }
            else
            {
                Action(gameManager, ai, 2, Vector3.zero);
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

    GameObject GetNearestBall( Vector3 pos, GameManager manager){


		float min = 10000000f;
		GameObject nearestBall =null;
        int team = gameObject.GetComponentInParent<Player>().team;
        float halfCourt = manager.levelManager.stage.halfCourtLine;

        LevelManager lm = manager.levelManager;

       // Debug.Log("team :" + team);

        if (team == 1) {
            foreach (GameObject ball in lm.balls) {
                Ball ballScript = ball.GetComponent<Ball>();
                if (ballScript.grounded && !ballScript.grabbed && ball.transform.position.x <= halfCourt -.5 /* && !ballScript.isBeingPursued */) {                 // gameRule config
                    if (Vector3.Distance(pos, ball.transform.position) < min) {
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

                    if (ballScript.grounded && !ballScript.grabbed   && ball.transform.position.x >= halfCourt + .5 /* &&  !ballScript.isBeingPursued */)               // gameRule config
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

	Vector3 GetMoveTowards( Vector3 pos, GameObject target){
		Vector3 move = new Vector3 ();
		move.x = (target.transform.position.x - pos.x)/15;
		move.z = (target.transform.position.z - pos.z)/15;
		return move;
	}


    GameObject GetNearestBallContacted(Vector3 pos, GameManager manager)
    {


        float min = 10000000f;
        GameObject nearestBall = null;
        int team = gameObject.GetComponentInParent<Player>().team;
        float halfCourt = manager.levelManager.stage.halfCourtLine;

        LevelManager lm = manager.levelManager;

        // Debug.Log("team :" + team);

        if (team == 1)
        {
            foreach (GameObject ball in lm.hits.Keys)
            {
                Ball ballScript = ball.GetComponent<Ball>();
                if (ball.transform.position.x <= halfCourt - .5 /*&& !ballScript.isBeingPursued */)
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
            foreach (GameObject ball in lm.hits.Keys)
            {
                Ball ballScript = ball.GetComponent<Ball>();

                if ( ball.transform.position.x >= halfCourt + .5 /* && !ballScript.isBeingPursued */)               // gameRule config
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


    private void MoveTowardsTarget(Vector3 target)
    {
   
        Vector3 pos = ai.navMeshAgent.gameObject.transform.position;
        Vector3 move = (target - pos).normalized;
        float aiXVelocity;
        aiXVelocity = move.x * ai.xSpeed;    //  * Mathf.Clamp(ai.navSpeed,1f,12f)
        float aiZVelocity;
        aiZVelocity = move.z * ai.zSpeed;

      //  Debug.Log("aiXVelocity= " + aiXVelocity);
      //  Debug.Log("aiZVelocity= " + aiZVelocity);

        ai.SetNavVelocity(new Vector3(aiXVelocity, aiZVelocity));           // *arbitrary nums


      //  Debug.Log("aiVelocity towards contact ball= " + ai.navMeshAgent.velocity);
        // Debug.Log("aiXVelocity= " + aiXVelocity);
        //Debug.Log("aiZVelocity= " + aiZVelocity);

    }
}



