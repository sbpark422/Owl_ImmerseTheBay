using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfiguration : MonoBehaviour
{
    GameObject parent;
    LevelManager levelManager;
    GameObject playerAura;
    Player player;
    Controller3D controller3D;
    AI ai;

    static int healthStock = 3;    // Set from gameRule    

    
    public Animator animator;
    public AudioSource audioSource;
    public SpriteRenderer spriteRenderer;
    public Rigidbody rigidbody;
    public SphereCollider headCollider;   // extra points +?, sfx, vfx, etc
    public CapsuleCollider bodyCollider;

    public RuntimeAnimatorController play;
    public RuntimeAnimatorController win;

    public Material out_mat;
    public Material default_mat;

    bool onGround;
    bool isJumping;

    float pushVal = .2f;   // rename and pull to gr

    float t_contact0;

    public bool ballContact;     // what if multiple balls
    GameObject ballHit;





    void Start()
    {
       // print("PlayerConfig start");

        parent = this.transform.parent.gameObject;

        if (!player)
        {
            player = parent.GetComponent<Player>();
        }

        if (!playerAura)
        {

        }
        /*
        if (!controller3D)
        {
            controller3D = player.controller3DObject.GetComponent<Controller3D>();
        }

        if (!ai)
        {
            ai = player.aiObject.GetComponent<AI>();
        }

        */

        if (!levelManager)
        {
            GameObject gameManager = GameObject.Find("GameManager");

            if (gameManager)
            {
              levelManager =  gameManager.GetComponent<LevelManager>();
            }

            else
            {
                 gameManager = GameObject.Find("GameManager(Clone)");

                if (gameManager)
                {
                    levelManager = gameManager.GetComponent<LevelManager>();
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SwitchToWinAnimation()
    {
        throw new NotImplementedException(); 
    }

    public void RemoveContact()
    {
        ballContact = false;     // what if multiple balls?
        ballHit = null;

    }

    void OnCollisionEnter(Collision collision)
    {
      //  if (levelManager)
        {

            if (!player.isOut && levelManager.isPlaying)
            {
                if (collision.gameObject.tag == "Ball")
                {
                    ballHit = collision.gameObject;

                    if (ballHit.GetComponent<Ball>().CheckPlayerHit(player.team))                                                                           // make more module
                    {
                        print("~Contact~");

                        ballHit.GetComponent<Ball>().contact = true;

                        float ballHitVelocity = ballHit.GetComponent<Rigidbody>().velocity.magnitude;
                        print("ballHitVelocity = " + ballHitVelocity);
                  

                        ballContact = true;         // what if multiple balls

                        
                    //    TriggerHitAnimation();

                        float stallTime = .2f;
                        float hitDelay = .0005f;
         
                     //   TriggerKnockBack(ballHit.GetComponent<Rigidbody>().velocity, ballHitISupered);
                     //   SlowDownPlayer(hitDelay, stallTime);



                        levelManager.AddHit(ballHit, parent);

                      //  levelManager.TriggerHitFX(gameObject, ballHit);
                       // player.SetHitFX(true);

                        float ballHitPauseWeight = 50f;
                        float hitPauseDuration = Mathf.Clamp( ballHitVelocity / ballHitPauseWeight, FXManager.min_HitPauseDuration, FXManager.max_HitPauseDuration);
                        float hitPausePreDelay = .0125f;

                      //  DelayPause(hitPauseDuration, hitPausePreDelay);

                       // levelManager.CamShake(ballHit.GetComponent<Rigidbody>().velocity.magnitude, transform);

                        //if gameMode = local
                       // levelManager.PostFX("Player1Hit");
                    }

   
                }


                // TODO doesnt make smoothe as intended 
                if (collision.gameObject.tag == "Wall")
                {
                   // CorrectPosition(collision.gameObject)
                }




            }

        }   
    }

    private void SlowDownPlayer(float delayTime, float stallTime )
    {
       if (player.hasAI)
        {
            ai.SlowDown(delayTime, stallTime);
        }
       else
        {
            controller3D.SlowDownByVelocity(delayTime, stallTime);
        }
    }

    private void CorrectPosition()
    {
        float correctThresh = 1.0f;
        float velMag = rigidbody.velocity.magnitude;
        if (velMag > correctThresh && ballHit)
        {
            Vector3 diff = (transform.position - ballHit.transform.position) * pushVal * (velMag / 100f);
            rigidbody.velocity = Vector3.zero;
            Vector3 nuPos = new Vector3(transform.position.x + diff.x, transform.position.y - .025f, transform.position.z + diff.z);
            rigidbody.MovePosition(nuPos);              // ?? why not Addforce?
        }
    }

    private void CorrectPosition(GameObject collisionObject, float correctionValue)
    {
        float velMag = rigidbody.velocity.magnitude;

            Vector3 diff = (transform.position - collisionObject.transform.position) * pushVal * (velMag / 100f) * correctionValue;
            rigidbody.velocity = Vector3.zero;
            Vector3 nuPos = new Vector3(transform.position.x + diff.x, transform.position.y , transform.position.z + diff.z);
            rigidbody.MovePosition(nuPos);              // ?? why not Addforce?
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
           
        }
        else
        {
          //  inBallCollision = false;
        }

    }


    private void TurnInCollisionFalse()
    {
        controller3D.SetAccelerationRate(.85f);
      //  inBallCollision = false;

    }

    internal void TriggerHitAnimation()
    {
        if (animator)
        {
            animator.SetTrigger("Hit");
        }
    }

    private void TriggerKnockBack(Vector3 ballVelocity, bool ballIsSupered)                                                                    // important to revitalize
    {


        float superMultipliier = 2f;
        Vector3 knockBackForce = new Vector3(.025f, .0125f, .025f);
        float knockBackTimeMult =  1f;

        if (ballIsSupered)
        {
            knockBackForce *= superMultipliier;
        }

        Vector3 knockBackVec = Vector3.Scale(ballVelocity, knockBackForce);


        // Gotta revisit knockedOut and knockback

        if (player.hasAI)
        {
            ai.SetKnockedOut(knockBackVec.magnitude * knockBackTimeMult);
           // ai.SetNavVelocity(ai.navMeshAgent.velocity + knockBackVec);


        }
        else
        {
          controller3D.SetKnockedOut(knockBackVec.magnitude * knockBackTimeMult);
          //  rigidbody.velocity += knockBackVec;
        }
      
    }

    private void DelayPause(float hitPauseDuration, float hitPausePreDelay)
    {
        levelManager.SetHitPauseDuration(hitPauseDuration);
        //Invoke("DoHitPause", hitPausePreDelay);
        levelManager.HitPause();

    }


    private void DoHitPause()
    {
        levelManager.HitPause();
    }

    internal void SetOutAnimation(bool v)
    {
        animator.SetBool("isOut", v);
    }
}
