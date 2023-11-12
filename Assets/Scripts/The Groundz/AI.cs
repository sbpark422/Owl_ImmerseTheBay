using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour {

    public GameObject parent;
    public Player playerScript;

    private LevelManager levelManager;
    GameManager gameManager;

    public GameObject playerConfigObject;

    public NavMeshAgent navMeshAgent;
    public Transform target;

    int [,] board; //   <-- interesting
	//public Board boardManager;
	public enum Type {aggresive,timid,random};
	public Type type;

	public float vertInput;   // shouldnt these liv in MyJoystick
    public float horzInput;
	public bool jumpInput;

	public bool action1Input;
	public bool rTriggerInput;
	public bool superInput;
	public bool blockInput;

    public GameObject ball;
    public GameObject ballHit;

    private Animator animator;
    public SpriteRenderer spriteRenderer;
    private Transform transform;
	private Rigidbody rigidbody;
	private Collider collider; //which?
	private float sizeX;
	private float sizeY;
	private float sizeZ;
	private Vector3 move;
    private int team;

    private Vector3 throww;

    float throwCharge = 0;
    bool isCharging = false;
    float chargeTime;
    Vector3 chargeVel;
    float maxCharge = 10f;

    private float randomThrowFactor = 30f;
    public float randomThrowFactor0 = 30f;
    public int level = 1;
    float throwScale = 25f;                   
    float speedScale = 2f;
    float catchProb = .2f;

    bool catchReady = true;
    public float catchLagTime = .25f; //secs
    private float catchCoolDown;
    int catchFrameCount = 1;           
    int catchFrameCool;
    float panickDelayTime = 3.0f;
    public bool isPanicking;


    public float xSpeed = 0f;
    public float zSpeed = 0f;
    public float acceleration; 

    private float xCeleration = 0;
	public float maxCeleration = 6;
	//public float accelerationTime = 0.35f;
    public float accelerationRate = .125f;

    private Vector3 velocityDamp;
	public float jumpSpeed = 10.0f;

	//private Vector3 velocity;      TBC, look at its references as well

    public float navSpeed = 5f;
    public float navXceleration = 4;
	private float angle;

    public float stamina;
    public float staminaCool;
    public float staminaCoolRate;
    public float dodgeStaminaCost = 10f;
    public float throwStaminaCost;
    public float moveStaminaCost;
    public float catchStaminaCost;
    public float pickUpStaminaCost;

    public float toughness =5f;

	private bool isFacingRight;
    private bool onGround = true;
    private bool wallCollision;
	public bool inBounds;


    private float dodgeCoolTime = 2f;
    private float dodgeCool = 0.0f;
    public bool isDodging;
	public float dodgeSpeed = 3f;

	public Vector3 handSize = new Vector3 (3f, 3f, 3f);
	public float grabRadius = 5f;


	public float throwPower = 200f;
    public float standingThrowPower;

	private Vector3 throwDirection;
	//public bool ballContact;
	public bool ballGrabbed = false;
	public bool ballThrown;
    public bool ballCaught;

    public bool isBlocking;

    private float t_c0;
    private float t_cF;

    private float t_s0;
    private float t_sF;

    private GameObject ballSuperPackage;
    public bool isSupering;
	public float superCoolDown = 10f;

    private GameObject ballSupered;
    public bool isKnockedOut;
    private float knockedOutTime;

    bool isSlowingDown;


    private float t_k0;
    private float t_kF;


    public AIState aiState;
    public string aiStateDisplayString;
    int prevStateNum;

    public enum GameState { safe, mildly_safe, mild, mildly_dangerous, dangerous };
    public GameState gameState;


    public int intensity;
    
    public AIState idle_ = new Idle();                  //0
    public AIState getBall_ = new GetBall();           //1
    public AIState throwBall_ = new ThrowBall();      //2
    public AIState ready_ = new Ready();             //3
    public AIState panic_ = new Panic();            //4
    public AIState retreat_ = new Retreat();       //5
    public AIState shake_ = new Shake();          //6

                                         // public AIState runPattern_ = new RunPattern();       //7



    public Transform retreatPoint;

    public bool randomizing;

    Color color;

    public bool debugMode;

    public bool addedAtStage;

    float awareness = 1f;
    bool didAwarenessRoll;
    bool isAware;

    bool isPausing;  // Gives a sense of realism between actions

    private void Awake()
    {

    }


    void Start () {

        GameObject gameManagerObject = GameObject.Find("GameManager");

        if (gameManagerObject)
        {
            levelManager = gameManagerObject.GetComponent<LevelManager>();
        }

        else
        {
            gameManagerObject = GameObject.Find("GameManager(Clone)");

            if (gameManagerObject)
            {
                levelManager = gameManagerObject.GetComponent<LevelManager>();
            }
        }

        levelManager = gameManagerObject.GetComponent<LevelManager>();

        playerScript = gameObject.GetComponentInParent<Player>();
        parent = gameObject.transform.parent.gameObject;
        transform = gameObject.transform;

        playerConfigObject = playerScript.playerConfigObject;
        navMeshAgent = playerConfigObject.GetComponent<NavMeshAgent>();
        animator = playerConfigObject.GetComponent<Animator>();
        spriteRenderer = playerConfigObject.GetComponent<SpriteRenderer>();
        rigidbody = playerConfigObject.GetComponent<Rigidbody>();
		collider = playerConfigObject.GetComponent<Collider> ();
		sizeX = collider.bounds.max.x - collider.bounds.min.x;
		sizeY = collider.bounds.max.y - collider.bounds.min.y;
		sizeZ = collider.bounds.max.z - collider.bounds.min.z;

        team = playerScript.team;
        color = playerScript.color;


        /*
        if (retreatPoint == null)
        {
           if(playerScript.number == 1)
            {
              retreatPoint =  GameObject.Find("P1 Retreat Point").transform;
            }
            if (playerScript.number == 2)
            {
                retreatPoint = GameObject.Find("P2 Retreat Point").transform;
            }
            if (playerScript.number == 3)
            {
                retreatPoint = GameObject.Find("P3 Retreat Point").transform;
            }
            if (playerScript.number == 4)
            {
                retreatPoint = GameObject.Find("P4 Retreat Point").transform;
            }
        }

         */

         gameManager = levelManager.gameObject.GetComponent<GameManager>();                        // todo
        retreatPoint = parent.transform;  // or grab from lm 


        idle_.Start(gameManager, this);     // abstract classes don't inherit monobehaviours start
        getBall_.Start(gameManager, this);
        throwBall_.Start(gameManager, this);
        panic_.Start(gameManager, this);
        retreat_.Start(gameManager, this);
        ready_.Start(gameManager, this);


     //   navMeshAgent.enabled = true;

       aiState = idle_;                    //   <-- $ Name me something initty 
      //   aiState = ready_;
    }
    

    internal void SpriteFlip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }


    // Update is called once per frame
    void Update () {

        if (levelManager.isPlaying) {
            if (navMeshAgent.isOnNavMesh )
            {
                //     print(" IsOnNAvMesh" );


                if (!isKnockedOut && !isPausing)
                {

                    if (playerConfigObject.GetComponent<PlayerConfiguration>().ballContact && !ballGrabbed)
                    {
                        aiState = getBall_;

                    }

                   // print(" ");
                   // print("aiState = " + aiState.GetName());
                    aiState.Update(gameManager, this);
             
                    aiStateDisplayString = aiState.GetName();


                    MoveInput();
                    GrabInput();
                    SuperInput();
                    BlockInput();

                }
                else
                {
                    HandleContact();
  
                }
            }

            else
            {
                print(" !OnNAvMesh");
            }

        }
        else
        {
                                                                                                                                                                                                                                                            
             EndAgentNavigation();                          // place onStandby
        }

        playerConfigObject.transform.LookAt(Camera.main.transform.position, Vector3.up);
        playerConfigObject.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0f, transform.localEulerAngles.z);     // Stay 2D for me please lol
    }

    internal void SetKnockedOut(float magnitude)
    {
        isKnockedOut = true;
        knockedOutTime = magnitude / toughness;
        print("KnockedOutTime " +knockedOutTime);
        print("aiState= " + aiState.GetName());
        print("ai.gameState= " + gameState);
    }

    internal void EndAgentNavigation()
    {
        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            horzInput = 0f;
            vertInput = 0f;
        }
    }

    internal void SetAgentDestination(Transform t)
    {
        navMeshAgent.SetDestination(t.position);
    }

    private void HandleContact()
    {
        if (isKnockedOut)                  
        {
            knockedOutTime -= Time.deltaTime;

            if (knockedOutTime <= 0f)
            {
                isKnockedOut = false;
                knockedOutTime = 0;
            }

        }
    }

    void MoveInput(){


        if (InBounds())
        {

                if (onGround && !isDodging)
                {

                    CustomMoveInput();      
                }

                float moveThresh = 2f;

                if ( navMeshAgent.velocity.magnitude > moveThresh)          //* arbitrary nums
                {
                    if (!isDodging)
                    {

                        if (animator)
                        {
                            animator.SetFloat("Speed", Mathf.Clamp(navMeshAgent.velocity.magnitude / 20f, 1f, 2f)); // *arbitrary nums

                            if (animator.GetBool("Running") == false)
                            {
                                animator.SetBool("Running", true);
                            }
                        }
                    }
                }

                else
                {

                    if (animator)
                    {

                        if (animator.GetBool("Running") == true)
                        {
                            animator.SetBool("Running", false);
                        }
                        if (staminaCool > 0.0f)
                        {
                            staminaCool -= 1f; // deprecated
                        }
                    }
                }
        }


		//handle Dodge/Jump Input
		if ((jumpInput) && dodgeCool <= 0 && staminaCool < stamina) {
			if (InBounds ()) 
            {
				//print ("Dodge!");
				// TODO vary force on Input. I.e implement an actual jump
				isDodging = true;
                Vector3 dodgeForce = (new Vector3(0f, 0f,UnityEngine.Random.Range(-2.0f, 2.0f) * dodgeSpeed));

                if (dodgeForce.magnitude < dodgeSpeed )
                {
                    dodgeForce.z = Mathf.Clamp(dodgeForce.z, Mathf.Sign(dodgeForce.z) * dodgeSpeed, Mathf.Sign(dodgeForce.z) * dodgeSpeed);
                }

                navMeshAgent.velocity += dodgeForce;

               //print("dodgeForce = " + dodgeForce);
                jumpInput = false;
                dodgeCool = dodgeCoolTime;
                staminaCool += dodgeStaminaCost;

                if (animator)
                {
                    animator.SetTrigger("Dodge");
                }

                Invoke("ResetDodgeTrigger", .2f);

            } 
		}	else {
           
				if (dodgeCool > 0) {

                dodgeCool -= Time.deltaTime;

                if (dodgeCool > dodgeCoolTime/2f)
                {
                    navMeshAgent.velocity /= 1.025f;
                }
                else
                {
                    if (isDodging)
                    {
                        isDodging = false;
                    }
                   
                }


                 
				}
		}

        if (navMeshAgent.velocity.x > 3.0f)    // *arbitray nums
        {
            isFacingRight = true;
           // Pivot();
            spriteRenderer.flipX = false;
            throwDirection.x = 1;              // hmm? lol
        }

        if (navMeshAgent.velocity.x < -3.0f)
        {
            isFacingRight = false;
          //  Pivot();
            spriteRenderer.flipX = true;
            throwDirection.x = -1;
        }
       
        if (navMeshAgent.velocity.z > 0.0f)
        {
            throwDirection.z = 1;
        }
        if (navMeshAgent.velocity.z < 0)
        {
            throwDirection.z = -1;
        }
    }

    private void Pivot()
    {
        //spriteRenderer.flipX = false;

        if (animator)
        {
            animator.SetTrigger("Pivot");
        }
    }

    private void CustomMoveInput()
    {
        move.x = horzInput;
        move.z = vertInput;

        if (Vector3.SqrMagnitude(move) > 0.0f)    // aka if we want to overide navMesh
        {
            Vector3 agentVelocity = new Vector3(move.x * xSpeed, 0.0f, move.z * zSpeed);
            SetNavVelocity(agentVelocity);
          //  print("CustomMove Debug");
          //  print("agentVelocity = " + agentVelocity);
        }
    }

    internal void SetNavVelocity(Vector3 vector3)
    {

        if (!isSlowingDown)
        {
           // navMeshAgent.velocity = Vector3.Lerp(navMeshAgent.velocity, vector3, accelerationRate);
            navMeshAgent.velocity = vector3;
           // print("vector3 " + vector3);
           // print(" navMeshAgent.velocity = " + navMeshAgent.velocity);
            //  print("vector3 = " + vector3);
            //  print("navMeshAgent.velocity = " + navMeshAgent.velocity);

        }
        else
        {
           // print("slowing down");
            navMeshAgent.velocity = Vector3.Lerp(navMeshAgent.velocity, Vector3.zero, accelerationRate);
           
        }

        float runAnimThresh = 8f;
        if (vector3.magnitude >=  runAnimThresh)
        {
            animator.SetBool("Running", true);
        }
    }

    public void SlowDown(float decelerationRate, float stallTime)
    {
        isSlowingDown = true;
        accelerationRate = decelerationRate;

        Invoke("NormalNavSpeed", stallTime);
    }

    void NormalNavSpeed()
    {
        isSlowingDown = false;
        accelerationRate = .85f;
        navMeshAgent.speed = navSpeed;
    }
  
    void GrabInput() {

        // pick up /catch

        CheckCatchCool();
        CheckCharge();

        if (staminaCool< stamina - 1)
        {
         //   print("GrabInput check 0: stamina check");

            if ((action1Input) && !ballGrabbed && catchReady)

        {
            //    print("GrabInput check 1: actionInput && !ballGrabbed && catchReady");

            float action1Cost = 0.1f;                      // level and character dependent
            DepleteStamina(action1Cost);                                                                                                                 

                ball = GetNearestBall();  
              if (ball) {
                    if ((Vector3.Distance(playerConfigObject.transform.position, ball.transform.position) < grabRadius) && ball)
                    {
                        //   print("GrabInput check 2: distance less than grab radius");

                        if (!ball.GetComponent<Ball>().isSupering)
                        {

                            //    print("GrabInput check 3: !isSupering");
                            Vector3 velocityCaught = ball.GetComponent<Rigidbody>().velocity;
                            ball.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);

                            ballGrabbed = true;                                 // grabbed 
                            ball.GetComponent<Ball>().grounded = false;
                            ball.GetComponent<Ball>().grabbed = true;
                            ball.transform.GetChild(3).gameObject.SetActive(false);
                            ball.GetComponent<SpriteRenderer>().enabled = false;
                            ball.GetComponent<SphereCollider>().enabled = false;
                            ball.GetComponent<Rigidbody>().useGravity = false;

                            ball.GetComponent<Ball>().PickUpDeactivate();
                            ball.transform.GetChild(1).gameObject.SetActive(false);     // most likely pikUp Deactivaate


                            if (ThrownByOpp(ball, 2) || ThrownByOpp(ball, 1))                          // check catch
                            {
                                if (animator)
                                {
                                    animator.ResetTrigger("Hit");
                                    animator.SetTrigger("Catch");
                                }

                                //ballContact = false;  // what if therre's multiple balls
                                ballCaught = true;     // what if therre's multiple balls
                                ResetBallCaught(.5f);

                                playerScript.SetHitFX(false);
                                ball.GetComponent<Ball>().playCatch();
                                levelManager.ClearContacts(ball);
                                levelManager.AddCatch(ball, parent);
                                levelManager.LastThrowerOut(ball);
                                levelManager.GetAnotherPlayer(gameObject.GetComponentInParent<Player>().team);   // GR check
                                levelManager.RemoveHit(ball);
                                levelManager.CatchDisplay(playerConfigObject.transform.position);
                                ball.GetComponent<Ball>().DeactivateThrow();

                                Pause(.5f);

                                float catchPauseDuration = Mathf.Clamp(velocityCaught.magnitude / 100f, FXManager.min_CatchPauseDuration, FXManager.max_CatchPauseDuration);
                                float catchPausePreDelay = .36f;

                                DelayPause(catchPauseDuration, catchPausePreDelay);

                                print("~!Caught!~");


                            }
                            else
                            {
                                print("~PickUp~");

                                if (animator)
                                {
                                    animator.SetTrigger("PickUp");
                                    animator.ResetTrigger("PickUp");
                                }
                            }

                            action1Input = false;
                        }
                        else                                  // gotta reset +1
                        {
                            action1Input = false;
                        }
                    }

                    else
                    {
                        //if (!ball.GetComponent<Ball>().isSupering)                   
                        {
                            if (!isPanicking) {
                                animator.SetTrigger("Ready");
                            }
                                else
                            {
                                float ran = UnityEngine.Random.Range(0f, 1f);
                                float prob = .5f / level;
                                if (ran < prob)
                                {
                                    animator.SetTrigger("Ready");
                                }
                            }
  
                            // print("Miss Pick Up");
                            action1Input = false;

                            /*
                            float rand = UnityEngine.Random.Range(-1.0f, 1.0f);             //   <--   insert frame counting coding
                                                                                                                                               // here, we're also reaching a level of athleticsim, "fear", emotion intensity
                            print("IJNcnisniNteri" + intensity +       "smiitiyintensity");

                            if (rand> 0)
                            {
                                action1Input = true;
                            }
                                else
                            {
                                action1Input = true;
                            }
                          */

                        }

                    }
                }
               

                if (catchReady)       // and catchframecount <= 0
                {
                   // catchReady = false;
                  //  catchCoolDown = catchLagTime;
                }


                CheckHasBallAnim();


            }



            else     // throw  = terrible coding        <-- ... whatever bitch           .   . . ...~ X  "Gentlemen!" lol 
            {

                Vector3 cockBackPos = new Vector3(playerConfigObject.transform.position.x + throwDirection.x * (collider.bounds.size.magnitude + handSize.x), playerConfigObject.transform.position.y + 1, playerConfigObject.transform.position.z);     // *might not even be a ball to cockback

                if (levelManager.IsInGameBounds(cockBackPos))
                {
                    if (ballGrabbed && (rTriggerInput) && ball && catchReady)           //should technically be throwReady
                    {

                        if (!isCharging)
                        {
                             chargeVel = navMeshAgent.velocity;
                            throwCharge += chargeVel.magnitude / 100f;
                            isCharging = true;
                            ball.GetComponent<Ball>().isCharging = true;
                            //  float glide = .01f - chargeVel.magnitude / 100000f;     //arbs
                            //  accelerationRate = Mathf.Clamp(glide, 0.000001f, 1.0f);  //arbs


                            animator.SetTrigger("Charge");
                        }



                        if (chargeTime > .2f)
                        {



                            print("~Throwing");

                            ball.transform.position = cockBackPos;

                            if (playerScript.team == 1 && throwDirection.x == -1)                // orientate correctly
                            {
                                spriteRenderer.flipX = !spriteRenderer.flipX;
                                throwDirection.x = 1;
                            }
                            if (playerScript.team == 2 && throwDirection.x == 1)
                            {
                                spriteRenderer.flipX = !spriteRenderer.flipX;
                                throwDirection.x = -1;
                            }

                            Transform nearestOpp = GetTargetedOpp();
                            Vector3 seekVec = Vector3.zero;

                            if (nearestOpp)
                            {
                                seekVec = nearestOpp.transform.position - ball.transform.position;
                            }

                            Vector3 randThrowVec = GetRandThrowVec(randomThrowFactor);
                            seekVec += randThrowVec;


                            Transform targetedOpp = GetTargetedOpp();
                            float renderLength = GetRenderLength();

                            throww = ((seekVec + rigidbody.velocity) / 2) * (throwPower + throwCharge);

                            throww = new Vector3(throww.x, 2.5f, throww.z);

                            ball.GetComponent<Ball>().Throw(throww, playerScript.color, false, 0, targetedOpp, renderLength, 0f);

                            print("throwCharge = " + throwCharge);


                            if (animator)
                            {
                                float throwMag = Vector3.Magnitude(throww);
                                float throwSpeedThresh = 300f;

                                float throwAnimSpeed = Mathf.Clamp(throwMag / throwSpeedThresh, 2f, 3f);

                                animator.SetFloat("ThrowSpeed", throwAnimSpeed);

                                animator.SetFloat("ThrowSpeed", throwAnimSpeed);
                                animator.SetTrigger("Release");
                            }


                            levelManager.AddThrow(ball, parent);

                            staminaCool += 5f;

                            ballGrabbed = false;
                            // Debug.Log("AI Standing Throw");

                            if (playerScript.team == 1)
                            {
                                ball.GetComponent<Ball>().SetThrown(gameObject.transform.parent.gameObject, 1);
                            }
                            if (playerScript.team == 2)
                            {
                                ball.GetComponent<Ball>().SetThrown(gameObject.transform.parent.gameObject, 2);
                            }
                            if (animator)
                            {
                                animator.SetBool("hasBall", false);
                                Invoke("ResetThrowAnimations", .0125f);

                            }

                            chargeVel = Vector3.zero;
                            throwCharge = 0;
                            chargeTime = 0;
                            isCharging = false;
                            ball.GetComponent<Ball>().isCharging = false;
                        }
                    }
                 
                }
            }
        }
 
            // TODO spriteRenderer.sprite = throwing;

        
		// move ball
		if (ballGrabbed && !isBlocking){
            Vector3 cockBackPos = new Vector3(playerConfigObject.transform.position.x + throwDirection.x * ((collider.bounds.size.magnitude / 1.5f) + handSize.x), playerConfigObject.transform.position.y + handSize.y, playerConfigObject.transform.position.z + handSize.z);
            if (levelManager.IsInGameBounds(cockBackPos))
            {
                if (!isSupering)
                {
                    ball.transform.position = cockBackPos;
                }
                else
                {
                    Vector3 nuVec = cockBackPos * 1.25f;
                    ball.transform.position = nuVec;
                }
            }
		}
		//

	}

    private void ResetBallCaught(float v)
    {
        Invoke("ResetBallCaught", v);
    }
    private void ResetBallCaught()
    {
        ballCaught = false;
    }
    private void Pause(float time)
    {
        isPausing = true;
        Invoke("ResetIsPausing", time);
    }

    private void ResetIsPausing()
    {
        isPausing = false;
    }

    private void CheckHasBallAnim()
    {
        
        if (!ballGrabbed)
        {

            if (animator.GetBool("hasBall") == true)
                Invoke("SetHasBallFalse", .25f);
        }
        else
        {
           
            if (animator.GetBool("hasBall") == false)
                animator.SetBool("hasBall", true);
        }
        
    }

    private void DepleteStamina(float cost)
    {
        if (staminaCool < stamina)     // use zero for depletion in stamina pass           .... matter of inverting everything
        {
            staminaCool += cost;
        }
    }

    private void CheckCatchCool()
    {
        if (!catchReady)
        {
            //  t_catchF = Time.realtimeSinceStartup;
            catchCoolDown -= Time.deltaTime;

            if (catchCoolDown <= 0)
            {
                catchReady = true;
            }
        }
    }

    void SetHasBallFalse()
    {
        animator.SetBool("hasBall", false);
    }

    private void CheckCharge()
    {
        float chargeRate = 1;  // character dependent??
        float chargeCost = .25f;

        if (ballGrabbed && isCharging)
        {

            throwCharge += (chargeRate * Time.deltaTime);
            chargeTime += Time.deltaTime;
            // throwCharge = Mathf.Clamp(throwCharge, 0f, maxStandingThrowPower - standingThrowPower);

            DepleteStamina(chargeCost);

           // animator.SetTrigger("Charge");
            // Invoke("ResetChargeAnimations", .05125f);    //arbs

        }
    }

    private Vector3 GetRandThrowVec(float randomThrowFactor)
    {
        Vector3 randThrowVec = new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(-1f, 1f) * randomThrowFactor / 2, UnityEngine.Random.Range(-1f, 1f) * randomThrowFactor);

        return randThrowVec;
    }

    private void ResetThrowAnimations()
    {
            animator.ResetTrigger("Charge");
       
    }

    private bool ThrownByOpp(GameObject ball, int team)
    {
        if (team == 2)
        {
            if (ball.GetComponent<Ball>().thrownBy1 && gameObject.GetComponentInParent<Player>().team == 2)
            {
                ball.GetComponent<Ball>().thrownBy1 = false;
                return true;
            }

        }
        if (team == 1)
        {
            if (ball.GetComponent<Ball>().thrownBy2 && gameObject.GetComponentInParent<Player>().team == 1)
            {
                ball.GetComponent<Ball>().thrownBy2 = false;     
                return true;
            }

        }
        return false;
    }

    private bool ObjectIsInGrabDistance(GameObject nearest)
    {
        if (playerConfigObject.transform.position.x + grabRadius > nearest.transform.GetChild(0).position.x &&
                 playerConfigObject.transform.position.x - grabRadius < nearest.transform.position.x)
        {
            if (playerConfigObject.transform.position.y + grabRadius > nearest.transform.position.y &&
                playerConfigObject.transform.position.y - grabRadius < nearest.transform.position.y)
            {
                if (playerConfigObject.transform.position.z + grabRadius > nearest.transform.position.z &&
                    playerConfigObject.transform.position.z - grabRadius < nearest.transform.position.z)
                {
                    float angle = 180f;
                    if (Vector3.Angle(playerConfigObject.transform.forward, nearest.transform.position - playerConfigObject.transform.position) < angle)            //hmm lol
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public GameObject GetNearestBall()               //reg ball
	{
		GameObject nearestBall = null;
		Vector3 smallest = new Vector3(10000f, 10000f, 10000f);
		GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball"); 
		foreach ( GameObject ball in balls)
		{
            if (Vector3.Magnitude(playerConfigObject.transform.position - ball.transform.position) < smallest.magnitude)
                if (ball.GetComponent<Ball>().isSupering == false && !ball.GetComponent<Ball>().grabbed)     // and is on my side ..  see GetBall
                {                             
                {
                    smallest = playerConfigObject.transform.position - ball.transform.position;
                    nearestBall = ball;
                }
			}
		}
		return nearestBall;
	}

	

    void OnGUI()
    {
        if (debugMode)
        {
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.fontSize = 24;
            guiStyle.normal.textColor = Color.green;

            // GUI.Label(new Rect(960, 800, 100, 20), "Game State = "+ gameState, guiStyle);
            // GUI.Label(new Rect(1000, 800, 100, 20), "Intensity  = " +intensity, guiStyle);
            GUI.Label(new Rect(100, 450, 100, 20), "~ AI Debug ~", guiStyle);

            GUI.Label(new Rect(75, 500, 100, 20), "AI State: ", guiStyle);
            GUI.Label(new Rect(200, 500, 100, 20), aiState.GetName(), guiStyle);

            GUI.Label(new Rect(75, 550, 100, 20), "Game State: ", guiStyle);
            GUI.Label(new Rect(250, 550, 100, 20), gameState.ToString(), guiStyle);
            GUI.Label(new Rect(75, 600, 100, 20), "Intensity: ", guiStyle);
            GUI.Label(new Rect(200, 600, 100, 20), ""+ intensity, guiStyle);

        }

    }


    private void Randomize()
    {
        horzInput = UnityEngine.Random.Range(-1 *5,5);
        vertInput = UnityEngine.Random.Range(-1 * 5, 5);
        randomizing = true;
    }

    private void TriggerKnockBack(Vector3 ballVelocity)
    {
        rigidbody.AddExplosionForce(ballVelocity.magnitude,ballVelocity,ballVelocity.magnitude/10);
        knockedOutTime = 3f;
        t_k0 = Time.realtimeSinceStartup;
        isKnockedOut = true;
        // animator.SetTrigger("Knock Out");


    }

    public bool InBounds(){
		inBounds = true;
        
        float padding = 5f;

        inBounds = true;
        if (gameObject.GetComponentInParent<Player>().team == 1)
        {
            if (collider.bounds.min.x < levelManager.stage.baseLineLeft + padding)
            {
                playerConfigObject.transform.position = new Vector3(levelManager.stage.baseLineLeft + collider.bounds.extents.x + padding * 1.125f, playerConfigObject.transform.position.y, playerConfigObject.transform.position.z);
                navMeshAgent.velocity = new Vector3(0f, navMeshAgent.velocity.y, navMeshAgent.velocity.z);
                inBounds = false;
                // print("Out of Bounds 1");

            }
            if (collider.bounds.max.x > levelManager.stage.halfCourtLine)
            {

                playerConfigObject.transform.position = new Vector3(levelManager.stage.halfCourtLine - collider.bounds.extents.x, playerConfigObject.transform.position.y, playerConfigObject.transform.position.z);
                navMeshAgent.velocity = new Vector3(0f, navMeshAgent.velocity.y, navMeshAgent.velocity.z);
                inBounds = false;
                // print("Out of Bounds 1");

            }

            if (collider.bounds.max.z > levelManager.stage.farSideLine - padding)
            {
                playerConfigObject.transform.position = new Vector3(playerConfigObject.transform.position.x, playerConfigObject.transform.position.y, levelManager.stage.farSideLine - collider.bounds.extents.z - padding * 1.125f);
                navMeshAgent.velocity = new Vector3(navMeshAgent.velocity.x, navMeshAgent.velocity.y, 0f);
                inBounds = false;
                //  print("Out of Bounds 1");

            }

            if (collider.bounds.min.z < levelManager.stage.nearSideLine + padding)
            {
                playerConfigObject.transform.position = new Vector3(playerConfigObject.transform.position.x, playerConfigObject.transform.position.y, levelManager.stage.nearSideLine + collider.bounds.extents.z + padding * 1.125f);
                navMeshAgent.velocity = new Vector3(navMeshAgent.velocity.x, navMeshAgent.velocity.y, 0f);
                inBounds = false;
                //  print("Out of Bounds 1");

            }

        }

        if (gameObject.GetComponentInParent<Player>().team == 2)
        {
            if (collider.bounds.min.x < levelManager.stage.halfCourtLine)
            {

                playerConfigObject.transform.position = new Vector3(levelManager.stage.halfCourtLine + collider.bounds.extents.x , playerConfigObject.transform.position.y, playerConfigObject.transform.position.z);
                navMeshAgent.velocity = new Vector3(0f, navMeshAgent.velocity.y, navMeshAgent.velocity.z);
                inBounds = false;
                  //  print("Out of Bounds 2");

            }
            if (collider.bounds.max.x > levelManager.stage.baseLineRight - padding)
            {
                playerConfigObject.transform.position = new Vector3(levelManager.stage.baseLineRight - collider.bounds.extents.x - padding * 1.125f, playerConfigObject.transform.position.y, playerConfigObject.transform.position.z);
                navMeshAgent.velocity = new Vector3(0f, navMeshAgent.velocity.y, navMeshAgent.velocity.z);
                inBounds = false;
                //   print("Out of Bounds 2");

            }

            if (collider.bounds.max.z > levelManager.stage.farSideLine - padding)
            {
                playerConfigObject.transform.position = new Vector3(playerConfigObject.transform.position.x, playerConfigObject.transform.position.y, levelManager.stage.farSideLine - collider.bounds.extents.z - padding * 1.125f);
                navMeshAgent.velocity = new Vector3(navMeshAgent.velocity.x, navMeshAgent.velocity.y, 0f);
                inBounds = false;
                //  print("Out of Bounds 2");

            }

            if (collider.bounds.min.z < levelManager.stage.nearSideLine + padding)
            {
                playerConfigObject.transform.position = new Vector3(playerConfigObject.transform.position.x, playerConfigObject.transform.position.y, levelManager.stage.nearSideLine + collider.bounds.extents.z + padding * 1.125f);
                navMeshAgent.velocity = new Vector3(navMeshAgent.velocity.x, navMeshAgent.velocity.y, 0f);
                inBounds = false;
                  // print("Out of Bounds 2");

            }
        }
        
        return inBounds;
	}

    void SuperInput()
    {

        if (level == 3)
        {
            if (superCoolDown > 0)
            {

                t_sF = Time.realtimeSinceStartup;
                superCoolDown -= Time.deltaTime;

                float superTime = 0f;
                if (playerScript.super.GetComponent<SuperScript>().type == 1 || playerScript.super.GetComponent<SuperScript>().type == 2)
                {

                    if (ballSuperPackage)
                    {
                        superTime = ballSuperPackage.GetComponent<SuperBall>().superTime;
                    }
                }
                else
                {
                    if (playerScript.super.GetComponent<SuperScript>().type == 3)
                    {
                        superTime = playerScript.super.GetComponent<SuperSpeed>().superTime;
                    }
                }

                if (t_sF - t_s0 >= superTime && isSupering)
                {
                    isSupering = false;
                    animator.SetBool("Supering", false);

                    if (playerScript.super.GetComponent<SuperScript>().type == 1 || playerScript.super.GetComponent<SuperScript>().type == 2)
                    {
                        foreach (Transform t in ballSupered.transform.GetComponentInChildren<Transform>()) //assumes there is only one ball supered at a time
                        {
                            ballSupered.GetComponent<Ball>().Normalize();
                            if (t.gameObject.tag == "SuperBall")
                                Destroy(t.gameObject);
                        }

                    }
                    else
                    {
                        if (playerScript.super.GetComponent<SuperScript>().type == 3)     //Nina
                        {
                          //  xSpeed = player.GetComponent<Player>().xspeed;
                          //  zSpeed = player.GetComponent<Player>().zspeed;
                        }
                    }
                }
            }

            if (superInput && superCoolDown <= 0 && ballGrabbed)   // super activate
            {
                t_s0 = Time.realtimeSinceStartup;
                isSupering = true;
                animator.SetBool("Supering", true);
                superCoolDown = transform.parent.gameObject.GetComponent<Player>().power;

                if (playerScript.super.GetComponent<SuperScript>().type == 1 || playerScript.super.GetComponent<SuperScript>().type == 2)
                {
                    ballSupered = ball;
                    ballSuperPackage = Instantiate(playerScript.super);
                    ballSupered.GetComponent<Ball>().SuperInit(ballSuperPackage);
                    ballSuperPackage.transform.parent = ballSupered.transform;
                    ballSuperPackage.transform.localPosition = Vector3.zero;
                    float scale = 2f;
                    if (playerScript.super.GetComponent<SuperBall>().type == 1)
                    {
                        scale = 5f;
                    }

                    throww = new Vector3(throwPower * rigidbody.velocity.x * scale, 1f, throwPower * rigidbody.velocity.z * scale);
                    SuperThrow(throww, "Super");
                }
                else
                {
                    if (playerScript.super.GetComponent<SuperScript>().type == 3)
                    {
                        float superTime = playerScript.super.GetComponent<SuperSpeed>().superTime;
                        float superBoost = playerScript.super.GetComponent<SuperSpeed>().speedBoost;
                        SpeedBoost(superTime, superBoost);
                    }
                }

            }
        }
    }

    private void SuperThrow(Vector3 throww, string type)
    {
        float magnetism =0f;
        if (ballSuperPackage || type == "Super")
        {
            if (ballSuperPackage.GetComponent<SuperBall>().type == 1)
            {
                magnetism = ballSuperPackage.GetComponent<SuperBall>().superMagnetism;
            }
            else
            {
                if (ballSuperPackage.GetComponent<SuperBall>().type == 2)
                    magnetism = ballSuperPackage.GetComponent<SuperTechBall>().seekMagnetism;
            }
        }

        Transform targetedOpp = GetTargetedOpp();
        float renderLength = GetRenderLength();

        ball.GetComponent<Ball>().Throw(throww, playerScript.color, true, magnetism,targetedOpp,renderLength, 1f);
        levelManager.AddThrow(ball, parent);
        ballGrabbed = false;
        throwPower = gameObject.GetComponentInParent<Player>().GetThrowPower0();

        if (animator)
        {
            animator.SetBool("hasBall", false);
        }
    }

    private float GetRenderLength()
    {
        
        float clipLength = 0f;
        /*
        foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
        {
           if (ac.name == "Mack.Ball.2.Throw" || ac.name == "King.2.Throw" || ac.name == "Nina.2.Throw"){
                if (ac.name == "King.2.Throw")
                {
                    clipLength = ac.length / 40;
                }
                else
                {
                    clipLength = ac.length / 20;
                }
            }
              
        }
        */
        return clipLength;
    }

    void BlockInput() {
		if (superCoolDown>0) {
			superCoolDown--;
		}

		if (superCoolDown < 40) {
			isBlocking = false;
		}

		if ((blockInput) && superCoolDown <=0 && ballGrabbed) {
			isBlocking = true;;
			superCoolDown = 50f;
		}

		if (isBlocking) {
			float nuBallX = playerConfigObject.transform.position.x + throwDirection.x*handSize.x*2;
			float nuBallY = playerConfigObject.transform.position.y;
			//left handed or right handed
			float nuBallZ = playerConfigObject.transform.position.z;
			ball.GetComponent<Rigidbody> ().useGravity = false;
			ball.transform.position = new Vector3 (nuBallX,nuBallY,nuBallZ);
		}
	}
    public void SetState(AIState nuState) {

		aiState = nuState;
        aiState.SetInAction(true);
	}
   public void EvaluateGameState()
    {
        intensity = GetIntensity();
    
        if (intensity > 11)
        {
            gameState = GameState.safe;
        }
        if (intensity > 6 && intensity <= 10)
        {
            gameState = GameState.mildly_safe;
        }
        if (intensity >= -5 && intensity <= 5)
        {
            gameState = GameState.mild;
        }
        if (intensity >= -10 && intensity < -6)
        {
            gameState = GameState.mildly_dangerous;
        }
        if (intensity < -11)
        {
            gameState = GameState.dangerous;
        }

    }

    internal int GetTeam()
    {
        return team;
    }


    public int GetIntensity()
    {
        int ballIntensity = GetBallIntensity();

        int oppIntensity = GetOppIntensity();

        return oppIntensity + ballIntensity;

    }

   public int GetBallIntensity()
    {
        Vector3 pos = playerConfigObject.transform.position;
        int count = 0;
        int team = gameObject.GetComponentInParent<Player>().team;
        float halfCourt = levelManager.stage.halfCourtLine;

        float ballThrownMult = 1f;

        float ballThrownAwareness = 0;


        if (team == 1)
        {
            foreach (GameObject ball in levelManager.balls)
            {
                if (ball.transform.position.x <= halfCourt && !ball.GetComponent<Ball>().thrownBy2)
                {
                    count++;
                    if (Vector3.Distance(playerConfigObject.transform.position, ball.transform.position) < grabRadius*2)        // *arb
                    {
                        count++;
                    }
                }
                else
                {

                    if (ball.transform.position.x > halfCourt)     // uneccessary check ... tf *2 lol
                    {
                        // ballNegatives -= Mathf.Abs(ball.transform.position.x - halfCourt);
                        //print("negative = " + Mathf.Abs(ball.transform.position.x - halfCourt));
                        count -= Mathf.Clamp ((int)  (Mathf.Abs(ball.transform.position.x - halfCourt)),0,1);

                    }

                    if (ball.GetComponent<Ball>().thrownBy2)
                    {
                        if (didAwarenessRoll && isAware)
                        {
                            float proximity = Vector3.Distance(navMeshAgent.transform.position, ball.transform.position);
                            float maxAwareness = 30f;
                            float minAwareness = 1f;
                            awareness = Mathf.Clamp(awareness / proximity, minAwareness, maxAwareness);
                            ballThrownAwareness = (int)(awareness * level);
                        }

                        else
                        {
                            if (!didAwarenessRoll)
                            {
                                didAwarenessRoll = true;
                                ball.GetComponent<Ball>().SetAwareAI(this); //inefficceint

                                if (DoIsAware())
                                {
                                    awareness = 100f;
                                    print("Is Awaareree!");
                                }

                                else
                                {
                                    awareness = 1f;
                                }
                            }
                        }

                    }

                }
            }
        }

        if (team == 2)
        {
            foreach (GameObject ball in levelManager.balls)
            {
                if (ball.transform.position.x >= halfCourt && !ball.GetComponent<Ball>().thrownBy1)
                {
                    count++;
                    if (Vector3.Distance(playerConfigObject.transform.position, ball.transform.position) < grabRadius * 2)
                    {
                        count++;
                    }
                }
                else
                {

                    if (ball.transform.position.x < halfCourt)
                    {
                        count -= Mathf.Clamp((int)(Mathf.Abs(ball.transform.position.x - halfCourt)), 0, 1);
                    }

                    if (ball.GetComponent<Ball>().thrownBy1)
                    {
                        if (didAwarenessRoll && isAware)
                        {
                            float proximity = Vector3.Distance(navMeshAgent.transform.position, ball.transform.position);
                            float maxAwareness = 30f;
                            float minAwareness = 1f;
                           // print("awareness = " + awareness);
                          //  print("proximity = " + proximity);
                            ballThrownAwareness = Mathf.Clamp(awareness / proximity, minAwareness,maxAwareness);
                            ballThrownAwareness  *= level;

                    
                            
                         //   print("Ball thrown intensity = "+(int)(count - ballThrownAwareness));
                        }

                        else
                        {
                            if (!didAwarenessRoll)
                            {
                                didAwarenessRoll = true;
                                ball.GetComponent<Ball>().SetAwareAI(this); //inefficient

                                if (DoIsAware())
                                {
                                    awareness = 100f;
                                    
                                    print("Is Awaareree!");
                                }

                                else
                                {
                                    awareness = 1f;
                                }
                            }
                        }
                    }
                }
            }
        }

        return (int) (count - ballThrownAwareness);

    }

   public  int GetOppIntensity()
    {
        int returnMe = 0;
        Vector3 pos = playerConfigObject.transform.position;
        int team = gameObject.GetComponentInParent<Player>().team;

        if (team == 1)
        {
            foreach (GameObject opp in levelManager.tm2.players)
            {
                if (!opp.GetComponent<Player>().isOut)
                {
                    if (opp.GetComponent<Player>().hasAI)
                    {
                        if (opp.GetComponentInChildren<AI>().ballGrabbed)
                        {
                            returnMe--;
                            returnMe -= (int)(250 / Vector3.Distance(pos, opp.transform.GetChild(0).transform.position));       //*arbitrary nums
                        }
                        else
                        {
                            returnMe += (int)(200 / Vector3.Distance(pos, opp.transform.GetChild(0).transform.position));    //i dont think this is always true
                        }

                    }
                    else
                    {
                        if (opp.GetComponentInChildren<Controller3D>().ballGrabbed)
                        {
                            returnMe--;
                            returnMe -= (int)(350 / Vector3.Distance(pos, opp.transform.GetChild(0).transform.position));
                        }
                        else
                        {
                            returnMe += (int)(200 / Vector3.Distance(pos, opp.transform.GetChild(0).transform.position));
                        }
                    }
                }
            }
        }

        if (team == 2)
        {
            foreach (GameObject opp in levelManager.tm1.players)
            {
              if (!opp.GetComponent<Player>().isOut) {
                    if (opp.GetComponent<Player>().hasAI)
                    {
                        if (opp.GetComponentInChildren<AI>().ballGrabbed)
                        {
                            returnMe--;
                            returnMe -= (int)(250 / Vector3.Distance(pos, opp.transform.GetChild(0).transform.position));
                        }
                        else
                        {
                            returnMe += (int)(50 / Vector3.Distance(pos, opp.transform.GetChild(0).transform.position));
                        }

                    }
                    else
                    {
                        if (opp.GetComponentInChildren<Controller3D>().ballGrabbed)
                        {
                            returnMe--;
                            returnMe -= (int)(350 / Vector3.Distance(pos, opp.transform.GetChild(0).transform.position));
                        }
                        else
                        {
                            returnMe += (int)(50 / Vector3.Distance(pos, opp.transform.GetChild(0).transform.position));
                        }
                    }
                }
            }
        }
        return returnMe;
    }

    bool DoIsAware()
    {
        float ran = UnityEngine.Random.Range(0, 1f);

        float prob = (level / 4f) + .25f ;



        if (ran < prob)
        {
            isAware = true;
        }
        else
        {
            print("ran = " + ran);
            print("prob = " + prob);
            isAware = false;
        }
        return isAware;

    }

    public void NormalAwareness()
    {
        didAwarenessRoll = false;
        isAware = false;

    }

    internal void TriggerHeadHitAnimation()
    {
        if (animator)
        {
            animator.SetTrigger("Head Hit");
        }
    }

    internal void TriggerHitAnimation()
    {
        if (animator)
        {
            animator.SetTrigger("Hit");
        }
    }


    public void DropBall()
    {
        ballGrabbed = false;
        ball.GetComponent<Ball>().grabbed = false;
        ball.GetComponent<Rigidbody>().useGravity = true;
        ball.GetComponent<SphereCollider>().enabled = true;
        ball.GetComponent<SpriteRenderer>().enabled = true;
        if (animator)
        {
            animator.SetBool("hasBall", false);
        }
    }

    internal bool IsAtNavTarget()
    {
        // Check if we've reached the destination
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    // Done
                  // print("At " +navMeshAgent.destination);
                   // print("position = " + transform.position);
                   // print(navMeshAgent.destination);
                    return true;
                }
            }
        }

        return false;
    }

    internal bool IsAtRetreatPoint() // Deprecated
    {
        float prox = 10.0f;

        if (Vector3.Magnitude(playerConfigObject.transform.position - retreatPoint.position) < prox)
        {
            return true;
        }

        return false;
    }


    internal void DoIdle()
    {
        GameManager gameManager = levelManager.gameObject.GetComponent<GameManager>();   /// todo
       idle_.Action(gameManager,this, 2, Vector3.zero);
    }

    public void SetNavSpeed(float s)
    {
        navSpeed = s;
    }
    public void AddNavSpeed(float s)
    {
         xSpeed += s/100f;
         zSpeed += s/100f;
        navMeshAgent.speed += s * 2f;
        navMeshAgent.acceleration += s *1.5f;
    }

    public void AddThrowPower(float p)
    {
        throwPower += p;
    }


    internal void SpeedBoost(float dur, float mult)
    {
      //  xSpeed = xSpeed * mult;
      //  zSpeed = zSpeed * mult;
    }

    private Transform GetTargetedOpp()
    {

        Transform nearestTargetedOpp = null;
        float nearest = 1000000f;
        if (GetComponentInParent<Player>().team == 1)
        {

            foreach (GameObject player in levelManager.tm2.players)
            {
                if (player.GetComponent<Player>().isOut == false)
                {
                    Vector3 diff = player.transform.GetChild(0).transform.position - playerConfigObject.transform.position;
                    // Vector3 comp = rigidbody.velocity - diff;
                    if (Vector3.Magnitude(diff) < nearest)
                    {
                        nearestTargetedOpp = player.transform.GetChild(0);
                        nearest = diff.magnitude;
                    }

                }

            }
        }
        else
        {
            if (GetComponentInParent<Player>().team == 2)
            {
                foreach (GameObject player in levelManager.tm1.players)
                {
                    if (player.GetComponent<Player>().isOut == false)
                    {
                        Vector3 diff = player.transform.GetChild(0).transform.position - playerConfigObject.transform.position;
                        // Vector3 comp = rigidbody.velocity - diff;
                        if (Vector3.Magnitude(diff) < nearest)
                        {
                            nearestTargetedOpp = player.transform.GetChild(0);
                            nearest = diff.magnitude;
                        }

                    }

                }
            }
        }

        return nearestTargetedOpp;
    }

    internal void LevelIncrease(float x)
    { 
        level++;
        x = x * level;
        DecreaseRandFact(x); 
        AddNavSpeed(x * speedScale);
        AddThrowPower(x * throwScale);
        IncreaseCatchProb(x);
        //IncreaseDodgeProb
        DecreaseCatchLag(x);
        DecreasePanickDelayTime(x);
        toughness += 5f;

    }

    private void DecreasePanickDelayTime(float x)
    {
        panickDelayTime = Mathf.Clamp(panickDelayTime - (x / 1000.0f), .1f, 3.0f);                       // * arb city
    }

    private void DecreaseCatchLag(float x)
    {
        catchLagTime = Mathf.Clamp(catchLagTime - (x / 5000.0f), .1f, 1.0f);
    }

    private void IncreaseCatchProb(float x)
    {
        catchProb = Mathf.Clamp(catchProb + (x / 50.0f), 0, .89f);                        // 
    }

    internal float GetPanickDelayTime()
    {
        return panickDelayTime;
        return panickDelayTime;
    }

    internal int GetLevel()
    {
        return level;
    }

    internal float GetCatchProb()
    {
        return catchProb;
    }

    private void DecreaseRandFact(float x)
    {
        if (randomThrowFactor > 1f)
        {
            randomThrowFactor -= x;
        }
      
    }

    internal void ResetLevel()
    {
        level = 1;
        navMeshAgent.speed = navSpeed;
        navMeshAgent.acceleration = navXceleration;
        throwPower = playerScript.throwPower0;
        randomThrowFactor = randomThrowFactor0;
        catchProb = .001f;
        catchLagTime = .5f;  // *arb
        panickDelayTime = .5f;
    }

    private void DelayPause(float hitPauseDuration, float hitPausePreDelay)
    {
        levelManager.SetHitPauseDuration(hitPauseDuration);
        Invoke("DoHitPause", hitPausePreDelay);

    }

    private void DoHitPause()
    {
        levelManager.HitPause();
    }

    public void FaceOpp()
    {
        bool isFacingRight = !spriteRenderer.flipX;

        if (playerScript.team == 1 && !isFacingRight)
        {
            SpriteFlip();
        }
        else
        {
            if (playerScript.team == 2 && isFacingRight)
            {
                SpriteFlip();
            }
        }
    }


    public void Init()
    {
        GameObject gameManagerObject = GameObject.Find("GameManager");

        if (gameManagerObject)
        {
            levelManager = gameManagerObject.GetComponent<LevelManager>();
        }

        else
        {
            gameManagerObject = GameObject.Find("GameManager(Clone)");

            if (gameManagerObject)
            {
                levelManager = gameManagerObject.GetComponent<LevelManager>();
            }
        }


        playerScript = gameObject.GetComponentInParent<Player>();
        parent = gameObject.transform.parent.gameObject;
        transform = gameObject.transform;

        playerConfigObject = playerScript.playerConfigObject;
        playerConfigObject.transform.position = Vector3.zero;
        navMeshAgent = playerConfigObject.GetComponent<NavMeshAgent>();
        animator = playerConfigObject.GetComponent<Animator>();
        spriteRenderer = playerConfigObject.GetComponent<SpriteRenderer>();
        rigidbody = playerConfigObject.GetComponent<Rigidbody>();
        collider = playerConfigObject.GetComponent<Collider>();
        sizeX = collider.bounds.max.x - collider.bounds.min.x;
        sizeY = collider.bounds.max.y - collider.bounds.min.y;
        sizeZ = collider.bounds.max.z - collider.bounds.min.z;

        team = playerScript.team;
        color = playerScript.color;

        GameManager gameManager = levelManager.gameObject.GetComponent<GameManager>();                        // todo
        retreatPoint = parent.transform;  // or grab from lm 

        idle_.Start(gameManager, this);     // abstract classes don't inherit monobehaviours start
        getBall_.Start(gameManager, this);
        throwBall_.Start(gameManager, this);
        panic_.Start(gameManager, this);
        retreat_.Start(gameManager, this);
        ready_.Start(gameManager, this);

        //   navMeshAgent.enabled = true;

        aiState = idle_;                    //   <-- $ Name me something initty 
                                            //   aiState = ready_;
    }

    public Vector3 GetPosition()
    {
        return playerConfigObject.transform.position;
    }

    public LevelManager GetLevelManager()
    {
        return levelManager;
    }

    private void CheckStamina()
    {
        if (navMeshAgent.velocity.magnitude < 3f)             // *arb = moveThresh
        {

            if (staminaCool > 0.0f)      // should invert ... i.e - cost, as opposed to + cost
            {
                staminaCool -= staminaCoolRate;        //  *Should go time dependent
            }
        }
    }

    void ResetDodgeTrigger()
    {
        if (animator)
        {
            animator.ResetTrigger("Dodge");
        }
    }
}
