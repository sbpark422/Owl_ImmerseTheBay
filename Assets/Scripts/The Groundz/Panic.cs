using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panic : AIState {

    AI ai;
    public bool inAction;
    GameObject gameObject;

    GameManager gameManager;

    public float vertInput;
    public float horzInput;
    public bool jumpInput;

    public bool action1Input;
    public bool rTriggerInput;
    public bool superInput;
    public bool blockInput;


    float sec = 0;
    float t0 = 0;

    public bool panicked;

    float panickTime;

    float panick0 = .2f;
       
    float panickDelayTime;
    float panickDelay;

    float panicked0;

    float aiCatchProb =.25f;
    bool rolledCatch;
    bool isCatchAware;

    int aiLevel;

    int num = 4;
    string name = "Panic";     // Rule number one, DONT PANIC lol


    float ranVelVec;

    public void Start(GameManager manager, AI ai_)
    {
        ai = ai_;
        gameObject = ai.gameObject;
        panickDelayTime = ai.GetPanickDelayTime();
        gameManager = manager;

    }


    // Update is called once per frame
    public void Update(GameManager manager, AI ai) {

        CheckPanickDelay();

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

    public void Action(GameManager manager, AI ai, float dur, Vector3 target) 
    {     //TBC
        Update(manager,ai);
        
        // ai.horzInput = Random.Range(-2.0f, 2.0f);
      //  ai.vertInput = Random.Range(-2.0f, 2.0f);
    }

    public void Action(float intensity, AI ai)
    {
        if ( panickTime == 0.0f)
        {
            Debug.Log("initial panic intensity = " + intensity);

            panickTime = Mathf.Abs(intensity * panick0);
          //  Debug.Log("panickTime = " + panickTime);
            ai.isPanicking = true;

            ai.FaceOpp();                                          // <-- this may not be working lol; 

            aiLevel = ai.GetLevel();

            aiCatchProb = ai.GetCatchProb();

            if (DodgeProb())
            {
                ai.jumpInput = true;
            }

            // aiLevel stuff here

        }
        

            if (sec < panickTime)
            {
             // Debug.Log("PANICKING");
           // Debug.Log("panick intensity = " + intensity);

            ranVelVec = Random.Range(-1f, 1f) * (intensity / 25f);

            inAction = true;

            ai.vertInput = Mathf.Lerp(ranVelVec, ai.vertInput, .0125f);

            if (!ai.ballGrabbed && intensity < -15 )
                {
                if ( !rolledCatch)
                {
                    if (CatchProb(aiCatchProb))
                    {
                      
                    

                    }
                }
                else
                    {
                        if (isCatchAware)
                        {
                         Ball nearestBall = ai.GetNearestBall().GetComponent<Ball>();
                        if ((ai.GetTeam() ==2 && nearestBall.thrownBy1) || (ai.GetTeam() == 1 && nearestBall.thrownBy2))
                        {
                          if( nearestBall.GetComponent<Rigidbody>().velocity.magnitude/100f < Mathf.Clamp(ai.level,1f,3.125f))
                            {
                                ai.action1Input = true;
                                Debug.Log("ActionInput");
                                Debug.Log("Ball speed = " + nearestBall.GetComponent<Rigidbody>().velocity.magnitude/100f);
                            }
                        }
                        }
                    }
               
 
                }

          

            //Debug.Log("ai.vertInput = "  + ai.vertInput);
            // ai.vertInput = Mathf.Clamp(ai.vertInput, -1.0f, 1.0f);

            sec += Time.deltaTime;
            }

            else
            {
            if (!panicked)
            {
                //   Debug.Log("panicked : @ " + sec);
                inAction = false;
                ai.vertInput = 0.0f;
                ai.action1Input = false;
                panicked = true;
                panicked0 = Time.realtimeSinceStartup;
                ai.isPanicking = false;
                rolledCatch = false;
                isCatchAware = false;
            }
            }
    }

    void TimidBehavior()
    {
        {
            if (ai.gameState == AI.GameState.safe)
            {
                if (inAction)
                {
                    Action(ai.intensity / 5f, ai);
                }
                else
                {
                    if (!ai.ballGrabbed)
                    {
                        ai.SetState(ai.getBall_);
                    }
                    else
                    {
                        ai.SetState(ai.throwBall_);
                    }
                }
  
            }
            if (ai.gameState == AI.GameState.mildly_safe)
            {
                if (inAction)
                {
                    Action(ai.intensity / 4f, ai);
                }
                else
                {
                    if (!ai.ballGrabbed)
                    {
                        ai.SetState(ai.getBall_);
                    }
                    else
                    {
                        ai.SetState(ai.throwBall_);
                    }
                }
            }


            if (ai.gameState == AI.GameState.mild)
            {

                if (inAction)
                {
                    Action(ai.intensity / 3f, ai);
                }
                else
                {
                    ai.SetState(ai.idle_);
                }
            }

            if (ai.gameState == AI.GameState.mildly_dangerous)
            {
                if (inAction)
                {
                    Action(ai.intensity / 2f, ai);
                }
                else
                {
                    if (panicked)
                    {

                        ai.SetState(ai.retreat_);
                    }
                    else
                    {
                        Action(ai.intensity, ai);
                    }
                }
            }

            if (ai.gameState == AI.GameState.dangerous)
            {
                if (inAction)
                {
                    Action(ai.intensity , ai);
                }

                else
                {
                    if (panicked)
                    {

                        ai.SetState(ai.retreat_);
                    }
                    else
                    {
                        Action(ai.intensity, ai);
                    }
                }
            }
        }
    }

    void AggressiveBehavior()
    {

    }


    void RandomBehavior()
    {

    }

    void CheckPanickDelay()
    {

       if (panicked)
        {
            float panickedTf = Time.realtimeSinceStartup;
            panickDelay = panickedTf - panicked0;
            //Debug.Log("panickDelay = " + panickDelay);

            panickDelayTime = ai.GetPanickDelayTime();

            if (panickDelay >= panickDelayTime)
            {
             //  Debug.Log("panickDelayTime = " + panickDelayTime);

                panicked = false;
                panickDelay = 0;
                panickTime = 0.0f;
                sec = 0.0f;

            }
        }
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

    bool CatchProb( float prob)
    {
        float ran = UnityEngine.Random.Range(0.0f, 1.0f);
        rolledCatch = true;

        Debug.Log("Catch prob = " + prob);
        Debug.Log("Catch ran = " + ran);


        if (ran < prob) {

             isCatchAware = true;
            
            return true;
        }
         else
        {
            isCatchAware = false;
            return false;
        }
    }

    bool DodgeProb()
    {
        float ran = Random.Range(0f, 1f);
        float dodgeProb = ai.GetLevel() * .25f;

        if (ran <dodgeProb)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetPanickTime( float t)
    {
        panickDelayTime = t;
    }

    public void SetInAction(bool x)
    {
        inAction = x;
    }
}
