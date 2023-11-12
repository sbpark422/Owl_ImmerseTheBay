using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    Rigidbody rigidbody;

    float initMass = 85f;

    Vector3 size = new Vector3(1.25f, 1.25f, 1.25f);

    public Sprite defaultSprite;

	public LevelManager levelManager;

	public AudioSource BallAudioSource;
	public AudioClip hit;
	public AudioClip bounce;
	public AudioClip thrownAudioClip;
	public AudioClip superInit;
	public AudioClip superRelease;
	public AudioClip superHit;
    public AudioClip catchh;
    public PhysicMaterial regular;
	public PhysicMaterial super;

    public float chargeAlpha;
	public bool grabbed;
    public bool thrown;
	public bool thrownBy1;   // change to int thrownByTeam
    public bool thrownBy2;   
    public bool grounded =true;

    public bool contact;
   // GameObject playerHit;

    private bool hasMag;
    private float magnetism;
    private Transform target;
    float seekVecCap = 10000f;
    float seekweight0 = 1f;
    float seekWeight;

	public Vector3 pos0;
	public bool isSupering;
	public Vector3 velocity;
	private Vector3 lastVelocity;
	private bool hasWhipped;


    public bool isInPickUpRange = false;

    public GameObject thrownIndicator;
    public GameObject hitIndicator;
    public GameObject Player2BallAura;
    public GameObject shadow;
    public GameObject floorMarker;

     float xSquashFactor = 750f;
     float ySquashFactor = 1500f;

    public GameObject superPackage;

    public String type = "";

    private float renderLength;
    private bool IsRendering;

    Color pColor;

    AI aiAware;
    public bool isBeingPursued;

    public bool isCharging;

    bool didWindFX;


    void Start () {

        type = "Normal";

        rigidbody = GetComponent<Rigidbody>();

        GameObject gameManagerObject = GlobalConfiguration.Instance.gameManager.gameObject;

        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager> ();
      //  thrownIndicator = Sprite.Create()
		size = transform.localScale;

        /*
		if (!BallAudioSource) {
            BallAudioSource = gameObject.transform.Find("Audio Source Ball").GetComponent<AudioSource>();
		}
        if (!thrownIndicator)
        {
            thrownIndicator = gameObject.transform.Find("Thrown Indicator").gameObject;
        }
        if (!hitIndicator)
        {
            hitIndicator = gameObject.transform.Find("Hit Indicator").gameObject;
        }
        if (!Player2BallAura)
        {
            Player2BallAura = gameObject.transform.Find("Player2BallAura").gameObject;
        }
        */
    }

	void Update () {

	/*

		if (!isSupering)
        {
            CheckSquash();

            /*
			if (!isSupering && (thrownBy1 || thrownBy2) && velocity.magnitude> 75f) {
				playWhip ();
			}
        
        }
        
        else
        {
            if (this.GetComponent<Rigidbody>().velocity.magnitude > 0 && !didWindFX)
            {
                float ballSuperVelocity = this.GetComponent<Rigidbody>().velocity.magnitude;

                print("ballSupered velocity = " + ballSuperVelocity);

               superPackage.GetComponentInChildren<SuperBall>().InstantiateSmokeFX();
                didWindFX = true;
            }
        }

        if (isInPickUpRange)
        {
         Check4PlayerProx();
        }
        if (thrown && hasMag & !contact && levelManager.isPlaying)
        {
          Seek(magnetism, target);     
        }

        if (IsRendering) // Fix
        {
            if(renderLength > 0)
            {
                renderLength -= Time.deltaTime;
            }
            else
            {
                shadow.SetActive(true); // methodize

                thrownIndicator.gameObject.SetActive(true);
                thrownIndicator.gameObject.GetComponent<SpriteRenderer>().color = pColor;
                ParticleSystem.MainModule mps = thrownIndicator.gameObject.GetComponent<ParticleSystem>().main;
                mps.startColor = pColor;

                this.GetComponent<SpriteRenderer>().enabled = true;

                IsRendering = false;
            }
        }
        */
    }

    private void CheckSquash()
    {


        velocity = gameObject.GetComponent<Rigidbody>().velocity;

        float squashThresh = 150f;

        //squash

        if (velocity.magnitude > squashThresh) {
            if (velocity.x > velocity.y)
            {
            }
            float nuX = Mathf.Clamp(Mathf.Abs(velocity.magnitude) / xSquashFactor, 0f, 2f);
            float nuY = Mathf.Clamp(Mathf.Abs(velocity.magnitude) / ySquashFactor, 0f, 2f);
            transform.localScale = new Vector3(size.x + nuX, size.y - nuY, transform.localScale.z);
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;


        } else {


            transform.localScale = Vector3.Lerp(transform.localScale, size, .5f);
            if (transform.localScale != size)
            {
              

                if (Vector3.Distance(transform.localScale, size) < .1)
                {
                    transform.localScale = size;
                }

               // GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            }

               
			}
    }

    private void Seek(float magnetism, Transform target)
    {
        float hoverForce = 36f;                 //grab from super
        float groundDistance = Mathf.Clamp((transform.position.y - levelManager.stage.floor), 1f, 100f);
     
        if (target)
        {

          //  print("Ball Seeking");
            Vector3 seekVec = target.transform.position - transform.position;

            seekVec = new Vector3(seekVec.x, seekVec.y + hoverForce / groundDistance, seekVec.z);

            Vector3 nuSeekVec = seekVec.normalized * 100;

            nuSeekVec = nuSeekVec / seekWeight;
         //   print("nuSeekVec = " + nuSeekVec);
            rigidbody.AddForce(nuSeekVec, ForceMode.Acceleration);

            if (seekWeight < 5f && seekWeight > 0f)   // *arbitrary num
                seekWeight += .05f;  // *arbitrary num
        }
    }



    private void Check4PlayerProx()
    {
        isInPickUpRange = false;

        foreach (GameObject player in GlobalConfiguration.Instance.GetPlayers())
        {
            if (player.GetComponent<Player>().hasJoystick && !player.GetComponent<Player>().isOut)   
            {
                Controller3D playerController = player.transform.GetChild(1).gameObject.GetComponent<Controller3D>();

                if ( playerController.ObjectIsInGrabDistance(this.gameObject))
                {
                    isInPickUpRange = true;
                    break;
                }
            }
        }
        if (!isInPickUpRange)
        {
            PickUpDeactivate();
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        // if (type == "Normal")
        /*
         {
             if (collision.gameObject.tag == "Player Sprite" && collision.collider.GetType() == typeof(SphereCollider))     // move to AI/Controller side
             {
               print("Head Collision");

                 if (!isSupering)
                 {
                     if (velocity.magnitude > 80f)
                     {
                         levelManager.PlayDamn();

                         if (collision.gameObject.GetComponent<Controller3D>().enabled == true)
                         {
                           //  collision.gameObject.GetComponent<Controller3D>().TriggerHeadHit();
                         }
                         else
                         {
                         //    collision.gameObject.GetComponent<AI>().TriggerHeadHit();
                         }
                     }
                     BallAudioSource.clip = bounce;
                     BallAudioSource.volume = .5f + Mathf.Clamp(velocity.magnitude / 100, 0f, 1f);
                     BallAudioSource.transform.position = gameObject.transform.position;
                     BallAudioSource.Play();
                 }
                 else
                 {
                     if (velocity.magnitude > 150f)
                     {
                         levelManager.PlayDamn(); // or crowd reaction
                     }
                 }
              
    }
            /*
            if (collision.gameObject.tag == "Player Sprite" && collision.collider.GetType() == typeof(CapsuleCollider))
            {
              //  print("Body Collision");

                if (!isSupering)
                {
                    BallAudioSource.clip = hit;
                    BallAudioSource.volume = .25f + Mathf.Clamp(velocity.magnitude / 100, 0f, 1f);
                    BallAudioSource.transform.position = gameObject.transform.position;
                    BallAudioSource.Play();


                }
            }
            if (collision.gameObject.tag == "Player Sprite" && isSupering)
            {
                BallAudioSource.clip = superPackage.GetComponent<SuperBall>().hitAudioClip;
                BallAudioSource.volume = Mathf.Clamp(velocity.magnitude / 120, 0f, 1f);
                BallAudioSource.transform.position = gameObject.transform.position;
                BallAudioSource.Play();
            }

            *
            
            if (((collision.gameObject.tag == "Playing Level") || (collision.gameObject.tag == "Wall")) || (collision.gameObject.tag == "Gate"))
            {
                
                hasMag = false;

                if (!isSupering)
                {
                    
                    if (collision.gameObject.tag == "Gate") // faulty
                    {
                        collision.gameObject.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(.8f, 1.2f);
                        collision.gameObject.GetComponent<AudioSource>().volume = Mathf.Clamp(velocity.magnitude / 1000, 0f, 1f);
                        collision.gameObject.GetComponent<AudioSource>().Play();
                    }
                    else
                    {
                        BallAudioSource.clip = bounce;
                        BallAudioSource.volume = Mathf.Clamp(velocity.magnitude / 120, 0f, 1f);
                        BallAudioSource.transform.position = gameObject.transform.position;
                        BallAudioSource.Play();
                    }
                }
                else
                {
                    if (!BallAudioSource.isPlaying)
                    {
                        BallAudioSource.clip = superPackage.GetComponent<SuperBall>().releaseAudioClip;
                        BallAudioSource.volume = Mathf.Clamp(velocity.magnitude / 110, 0f, 1f);
                        BallAudioSource.transform.position = gameObject.transform.position;
                        BallAudioSource.Play();
                    }
                }
                
            }
                */
            if (collision.gameObject.tag == "Playing Level")
            {
                DeactivateThrow();

                if (isSupering)
                {
                    //SuperDeactivate();
                }
            }
        /*
        hasWhipped = false;
    }



   if (type == "Speed")
    {
        if (collision.gameObject.tag == "Player Sprite")
        {
            GameObject playerSprite = collision.gameObject;
            if (playerSprite.GetComponent<Controller3D>().enabled)
            {
                SpeedBall speedBallComp = GetComponent<SpeedBall>();
                float dur = speedBallComp.dur;
                float mult = speedBallComp.mult;
                if (speedBallComp.hit == false)
                {
                    speedBallComp.Hit(playerSprite.GetComponent<Controller3D>());
                    playerSprite.GetComponent<Controller3D>().SpeedBoost(dur, mult);
                }
            }    
        }
    }
*/
    }

    public void ActivateTrail(Color terColor, Color teeColor)
    {
        TrailRenderer tr = gameObject.GetComponent<TrailRenderer>();
        tr.enabled = true;
        tr.startColor = terColor;
        tr.endColor = teeColor;


    }

    public void DeactivateThrow()
    {
        if (levelManager.hits != null)
        {
            if (!levelManager.hits.ContainsKey(gameObject))
            {
                levelManager.RemoveThrow(gameObject);
            }
        }
      //  if (contact)
        {
          //  DeactivateTrail();
           // hitIndicator.SetActive(false);
           // floorMarker.SetActive(false); // or inverse of Caught FX
        }


        contact = false;
        thrown = false;
        thrownBy1 = false;
        thrownBy2 = false;
        grounded = true;

        /*
        if (aiAware)
        {
            aiAware.NormalAwareness();
        }
        
        ParticleSystem hit_ps = gameObject.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();
        hit_ps.GetComponent<Renderer>().sortingOrder = 1;
        hit_ps.startSize = 0;



    
        //Add to 

        gameObject.transform.GetChild(1).gameObject.SetActive(false);


        Physics.IgnoreLayerCollision(5, 3,true);
            */


    }

    void DeactivateTrail()
    {
        if (gameObject.GetComponent<TrailRenderer>().enabled == true)
        {
            gameObject.GetComponent<TrailRenderer>().startWidth = 1;
            gameObject.GetComponent<TrailRenderer>().enabled = false;
        }
    }

    public void Throw(Vector3 throww, Color color, bool hasmag, float mag, Transform targ, float renderLength, float chargedThrowAlpha)  {
        //print("hasmag = " + hasmag);
        // print("Magnetism = " + mag);

        if (!isSupering)
        {
            if (hasmag)
            {

                hasMag = false;
                magnetism = mag;
                target = targ;
                seekWeight = seekweight0;
                //print("someHow has mag in Ball script");

            }

            gameObject.GetComponent<SphereCollider>().enabled = true;
            gameObject.GetComponent<Rigidbody>().AddForce(throww.x, throww.y, throww.z, ForceMode.Impulse);
           // gameObject.GetComponent<Rigidbody>().AddTorque(0f, 0f, throww.magnitude);  // *arbitrary num
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            grounded = false;
            grabbed = false;
            thrown = true;
            chargeAlpha = chargedThrowAlpha;

        }
        else
        {
            print("IsSupering");

            if (hasmag)
            {
                hasMag = hasmag;
                magnetism = mag;
                target = targ;
               // print(target);
               // print("target location: " + target.transform.position);
                seekVecCap *= 2f;
                seekWeight = seekweight0;

             //   print("someHow has mag in Ball script");

            }

            gameObject.GetComponent<SphereCollider>().enabled = true;
            gameObject.GetComponent<Rigidbody>().AddForce(throww.x, throww.y, throww.z, ForceMode.Impulse);
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            grounded = false;
            grabbed = false;
            thrown = true;
            BallAudioSource.clip = superRelease;
            BallAudioSource.Play();

            int superType = superPackage.GetComponent<SuperScript>().type;

            if (superType == 2)
            {
                if (thrownBy1)
                {
                    transform.eulerAngles = new Vector3(0f, 0f, 0f);
                    superPackage.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0f, 0f, 0f);
                    superPackage.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }

                rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            
             }

        }


            pColor = color;
            ReRender(renderLength);


    }

    private void ReRender(float rL)         // important for animation purposes but technically isnt sufficient as people can get hit by invisible balls lol
    {
        IsRendering = true;
        renderLength = rL;
    }

    public void playWhip()	{
		if (!hasWhipped) {	
            if (!BallAudioSource.isPlaying){
                BallAudioSource.volume = 0.85f;
                BallAudioSource.clip = thrownAudioClip;
                BallAudioSource.Play();
                hasWhipped = true;
            }
		}
	}

    public void playCatch()
    {

                BallAudioSource.pitch = UnityEngine.Random.RandomRange(1f, 1.3f);
                BallAudioSource.volume = 1f;
                BallAudioSource.clip = catchh;
                BallAudioSource.Play();
    }

    public void SuperInit()	{
		isSupering = true;
		GetComponent<Rigidbody> ().mass = 50f;
	//	GetComponent<PerceptionScaler> ().enabled = false;
		GetComponent<SphereCollider> ().material = super;
		BallAudioSource.volume = 1f;
		BallAudioSource.clip = superInit;
		BallAudioSource.Play ();

	}

	public void Normalize()	{


		isSupering = false;
        GetComponent<SpriteRenderer>().sprite = defaultSprite;
		GetComponent<Rigidbody> ().mass = initMass;  //*arbitrary nums
	//	GetComponent<PerceptionScaler> ().enabled = true;
		GetComponent<SphereCollider> ().material = regular;
        transform.localScale = size;                    
	

	}

    internal void SetThrown(GameObject parent, int team)
    {
        if  (team == 1)
        {
            thrownBy1 = true;
        }
        if (team == 2)
        {
            thrownBy2 = true;
        }
    }

    public void PickUpActivate( Color c)
    {
        isInPickUpRange = true;
        Player2BallAura.SetActive(true);
        ParticleSystem.MainModule mps = Player2BallAura.GetComponent<ParticleSystem>().main;
        mps.startColor = c;

    }

    public void PickUpDeactivate()
    {
        isInPickUpRange = false;
        Player2BallAura.SetActive(false);
    }

    internal void SuperInit(GameObject ballSuperPackage)
    {
        superPackage = ballSuperPackage;

        int type = ballSuperPackage.GetComponent<SuperBall>().type;

        if (type == 2)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.sprite = ballSuperPackage.GetComponent<SuperTechBall>().image;
            isSupering = true;
            GetComponent<Rigidbody>().mass = ballSuperPackage.GetComponent<SuperBall>().mass;
          //  GetComponent<PerceptionScaler>().enabled = false;
            GetComponent<SphereCollider>().material = ballSuperPackage.GetComponent<SuperBall>().superPhysicMaterial;
            BallAudioSource.volume = 1f;
            BallAudioSource.clip = ballSuperPackage.GetComponent<SuperBall>().initAudioClip;
            BallAudioSource.Play();
        }
        if (type == 1)
        {
            isSupering = true;
            GetComponent<Rigidbody>().mass = ballSuperPackage.GetComponent<SuperBall>().mass;
          //  GetComponent<PerceptionScaler>().enabled = false;
            GetComponent<SphereCollider>().material = ballSuperPackage.GetComponent<SuperBall>().superPhysicMaterial;
            BallAudioSource.volume = 1f;
            BallAudioSource.clip = ballSuperPackage.GetComponent<SuperBall>().initAudioClip;
            BallAudioSource.Play();
        }
    }

    private void SuperDeactivate()
    {
        isSupering = false;

        Normalize();
        foreach (Transform t in transform.GetComponentInChildren<Transform>())
            if (t.gameObject.tag == "SuperBall")
                    Destroy(t.gameObject);
            }

    internal bool CheckPlayerHit(int team)
    {
        if (team == 1)
        {
            if (thrownBy2 && grounded == false)
            {
                return true;
            }

        }
        if (team == 2)
        {
            if (thrownBy1 && grounded == false)
            {
                return true;
            }

        }

        return false;
    }

    internal void SetActiveHitFX(bool x)
    {
        hitIndicator.SetActive(x);
    }
    internal void SetActiveFloorMarker(bool v)
    {
        floorMarker.SetActive(v);
    }


    internal void DeRender()
    {
        print("DeRednering");
        this.GetComponent<SpriteRenderer>().enabled = false;
        shadow.SetActive(false);
        this.GetComponent<SphereCollider>().enabled = false;
        this.GetComponent<Rigidbody>().useGravity = false;

        thrownIndicator.SetActive(false);   
    }

    internal void SetAwareAI(AI aI)
    {
        aiAware = aI;
    }



    public void SetBallGrabbed(bool x)
    {
        
    }

}
        
   



