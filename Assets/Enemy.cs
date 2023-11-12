using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    LevelManager levelManager;
    Player player;
    void Start()
    {

        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        player = this.GetComponent<Player>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
           GameObject  ballHit = collision.gameObject;
            if (ballHit.GetComponent<Ball>().CheckPlayerHit(player.team))                                                                           // make more module
        {
            print("~Contact~");

            ballHit.GetComponent<Ball>().contact = true;

                float ballHitVelocity = ballHit.GetComponent<Rigidbody>().velocity.magnitude;
            print("ballHitVelocity = " + ballHitVelocity);
         //   bool ballHitISupered = ballHit.GetComponent<Ball>().isSupering;

         //   ballContact = true;         // what if multiple balls


            //TriggerHitAnimation();

            float stallTime = .2f;
            float hitDelay = .0005f;

           // TriggerKnockBack(ballHit.GetComponent<Rigidbody>().velocity, ballHitISupered);
           // SlowDownPlayer(hitDelay, stallTime);



            levelManager.AddHit(ballHit, player.gameObject);

          //  levelManager.TriggerHitFX(gameObject, ballHit);
          //  player.SetHitFX(true);

            float ballHitPauseWeight = 50f;
            float hitPauseDuration = Mathf.Clamp(ballHitVelocity / ballHitPauseWeight, FXManager.min_HitPauseDuration, FXManager.max_HitPauseDuration);
            float hitPausePreDelay = .0125f;

           // DelayPause(hitPauseDuration, hitPausePreDelay);
//
           // levelManager.CamShake(ballHit.GetComponent<Rigidbody>().velocity.magnitude, transform);

            //if gameMode = local
          //  levelManager.PostFX("Player1Hit");
        }


    }
}
}
