using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    // variables
    // Rebounds [] : ball -> (isSupered, velocity, groundedTime, isHeadHit), playersHit[] - > (color), playerThrown -> (color, FX type)


    // FX

    // Hit FX
    // Player Catch FX
    // Player Out Fx
    // Ball Grounded FX

    // Externals

    // Player Hit FX
    // Player Thrown FX
    // ball trail renderer
    // ball flash indication
    // isSuperBall Trail FX

    LevelManager levelManager;
    AudioSource audioSource;


    public GameObject HitSmallFX;
    public GameObject HitMedFX;
    public GameObject HitSuperFX;

    public GameObject outFX;
    public GameObject outFX2;
    public GameObject catchFX;
    public GameObject winFX;                  // should be stage specific
    public GameObject plusOneFX;

    public HitPause hitPause;
    private float hitPauseDuration;

    public static float min_HitPauseDuration = 0.25f;
    public static float max_HitPauseDuration = 2f;

    public static float min_CatchPauseDuration = 0.25f;
    public static float max_CatchPauseDuration = 2f;

    public GameObject reboundPlayerFX;
    public GameObject reboundBallFX;
    public GameObject reboundTrackFX;



    void Start()
    {
        levelManager = transform.parent.gameObject.GetComponent<LevelManager>();
        hitPause = gameObject.GetComponent<HitPause>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {


        /*
        if (!(ballHit.GetComponent<TrailRenderer>().startWidth == 2))     // multiplier based
        {
            if (ballHit.GetComponent<TrailRenderer>().enabled == false)
            {
                ballHit.GetComponent<TrailRenderer>().enabled = true;
            }
            else
            {
                ballHit.GetComponent<TrailRenderer>().startWidth = 2f;
            }
        }
        */
        /*

        ParticleSystem ball_hit_ps = ballHit.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();  // ball fx stuff, to remove

        ball_hit_ps.GetComponent<Renderer>().sortingOrder = 3; ballHit = collision.gameObject;   // ball fx stuff, to remove

        ball_hit_ps.startSize = ballVelocity; ballHit = collision.gameObject; // ball fx stuff, to remove

        */



        /*
            ParticleSystem.MainModule sup_main_ps = collision.gameObject.GetComponentInChildren<ParticleSystem>().main;
                            sup_main_ps.simulationSpeed = 20f;
                            sup_main_ps.startSizeX = 10f;
                            sup_main_ps.startSizeMultiplier = 10f;

                        levelManager.CamGlitch(ballVelocity);    // super fx stuff, to remove
        */

        /*
         private void HitFX(Ball ball, float hitPauseDuration, float hitPausePreDelay)
    {
        GameObject hitFX = levelManager.HitSmall;
        if (ball.chargeAlpha > .25f) hitFX = levelManager.HitMed;
        if (ball.chargeAlpha == 1f) hitFX = levelManager.HitMax;
        GameObject hfx = Instantiate(hitFX, ball.transform.position, Quaternion.identity);
        levelManager.CamShake(1f, transform);

        levelManager.SetHitPauseDuration(hitPauseDuration);
        Invoke("DoHitPause", hitPausePreDelay);
         
    }

        */
    }

    public void HitDisplay(GameObject hittee, GameObject ball)
    {
        
        bool ballIsSupering = ball.GetComponent<Ball>().isSupering;
             GameObject hfx;

            if (HitSmallFX != null && HitSuperFX)
            {
            GameObject hitter = levelManager.throws[ball].transform.GetChild(0).gameObject;
            Vector3 terPosition = hitter.transform.position;
            Color terColor = hitter.GetComponentInParent<Player>().color;

            Vector3 teePosition = hittee.transform.position;
            Color teeColor = hittee.GetComponentInParent<Player>().color;

            if (ballIsSupering)
            {
                hfx = Instantiate(HitSuperFX, teePosition, Quaternion.identity);
            }

            else
            {
                hfx = Instantiate(HitSmallFX, teePosition, Quaternion.identity);
            }

            ParticleSystem ps = hfx.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule psMain = ps.main;
            ParticleSystem.ColorOverLifetimeModule pscol = ps.colorOverLifetime;

         //   Gradient grad = new Gradient();
           // grad.SetKeys(new GradientColorKey[] { new GradientColorKey(teeColor, 0.0f), new GradientColorKey(terColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 1.0f), new GradientAlphaKey(.5f, 0.0f) });

            //  pscol.color = grad;
            // main.startColor = new ParticleSystem.MinMaxGradient(new Color(R, G, B, A));

            float ballVelocity = (ball.GetComponent<Rigidbody>().velocity.magnitude);
            float bv4s = Mathf.Clamp(ballVelocity / 360f, 1f, 2f);
            hfx.transform.localScale = new Vector3(bv4s, bv4s, bv4s);
            // psMain.simulationSpeed = Mathf.Clamp(20 / (ballVelocity / 40), 15, 25);
        }
        
    }

    public void CatchDisplay(Vector3 position)
    {
        if (catchFX != null)
        {
            Instantiate(catchFX, position, catchFX.transform.rotation);
        }
    }

    internal void CatchDisplay(Color c,Vector3 position, float velocity)
    {
        if (catchFX != null)
        {
            GameObject catchFXObject1 = Instantiate(catchFX, position, catchFX.transform.rotation);

            /*

            ParticleSystem.MainModule catchPsMain1 = catchFXObject1.GetComponent<ParticleSystem>().main;
            ParticleSystem.VelocityOverLifetimeModule catchPsVelMod = catchFXObject1.GetComponent<ParticleSystem>().velocityOverLifetime;
            ParticleSystem.ColorOverLifetimeModule catchPsColMod = catchFXObject1.GetComponent<ParticleSystem>().colorOverLifetime;

            Gradient grad = new Gradient();

            grad.SetKeys(new GradientColorKey[] { new GradientColorKey(c, 1.0f), new GradientColorKey(c, 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

            catchPsColMod.color = grad;

            catchPsVelMod.orbitalZMultiplier = 1 + velocity;

            GameObject catchFXObject2 = catchFXObject1.transform.GetChild(0).gameObject;
            ParticleSystem.MainModule catchPsMain2 = catchFXObject2.GetComponent<ParticleSystem>().main;
            ParticleSystem.ColorOverLifetimeModule catchPs2ColMod = catchFXObject2.GetComponent<ParticleSystem>().colorOverLifetime;

            Gradient grad1 = new Gradient();

            grad1.SetKeys(new GradientColorKey[] { new GradientColorKey(c, 1.0f), new GradientColorKey(Color.white, 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

            catchPs2ColMod.color = grad;

            catchPsMain2.startLifetime = Mathf.Clamp((velocity / 50f), .5f, 2f);

            catchFXObject1.transform.localScale = Mathf.Clamp((velocity / 100f), 1f, 2f) * Vector3.one;

            Gradient grad2 = new Gradient();

            grad2.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.white, 1.0f), new GradientColorKey(c, 0.5f), new GradientColorKey(Color.white, 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(1f, 0.5f), new GradientAlphaKey(0.0f, 1.0f) });

            Material myMat = catchFXObject1.GetComponent<Renderer>().material;
            int intensity = 6;
            myMat.SetColor("_EmissionColor", new Color(c.r, c.g, c.b , .36f) * intensity * 3);

            Material myMat1 = catchFXObject2.GetComponent<Renderer>().material;
            myMat1.SetColor("_EmissionColor", new Color(c.r, c.g, c.b, .36f) * intensity*4);

            */

        }
    }


    internal void OutDisplay(GameObject playerOut)
    {
        Transform playerT = playerOut.transform;
        Color color = playerOut.GetComponentInParent<Player>().color;

        Instantiate(outFX, playerT.position, outFX.transform.rotation);
        ParticleSystem psOut = outFX.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule psMain = psOut.main;
        psMain.startColor = new Color(color.r,color.g,color.b,.4f);

        ParticleSystem.ColorOverLifetimeModule pscolm = psOut.colorOverLifetime;
        pscolm.color = new ParticleSystem.MinMaxGradient(Color.gray, new Color(color.r, color.g, color.b, .1f));

    }



    public void WinDisplay(Vector3 position)
    {
        GameObject canvasGameGeneral = levelManager.cgg.gameObject;

        GameObject oneUp = Instantiate(plusOneFX, position, Quaternion.identity, canvasGameGeneral.transform);
        oneUp.GetComponent<RectTransform>().localPosition = new Vector3(position.x, 410);
        /*
        if (!winFX)
        {
           
        }
           
        winFX.SetActive(true);
        winFX.transform.position = position;
        Invoke("DestroyWinFX", 3);
        */
    }

    public void DestroyWinFX()
    {
        //  Destroy(winFX);
    }


    public void HitPause(float duration)
    {
        hitPause.Freeze(duration);
    }

    internal void HitPause()
    {
        hitPause.Freeze(hitPauseDuration);
    }

    internal void SetHitPauseDuration(float hitPauseDur)
    {
        hitPauseDuration = hitPauseDur;
    }

    internal void InstantiateReboundFX(GameObject ball, GameObject player)
    {
        GameObject currentBallPlayerFX = Instantiate(reboundBallFX, ball.transform.position, Quaternion.identity);
        GameObject currentReboundPlayerFX = Instantiate(reboundPlayerFX, player.transform.GetChild(0));
         currentReboundPlayerFX.transform.position = new Vector3(currentReboundPlayerFX.transform.position.x, 0, currentReboundPlayerFX.transform.position.z);
        GameObject linkFx = Instantiate(reboundTrackFX, ball.transform.position, Quaternion.identity);
        linkFx.GetComponent<ReboundLinkFX>().Track(currentReboundPlayerFX.transform.GetChild(0).transform.position, currentBallPlayerFX.transform.position);


    }




    /*
    public void CamShake(float intensity, Transform playerT)
    {
       camController.TrigCamShake(intensity, playerT);
    }

    internal void CamGlitch(float ballVelocity)
    {
        camController.GetComponent<CamController>().ActivateGlitch(ballVelocity);
    }
    */
}
