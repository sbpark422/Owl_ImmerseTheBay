using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.PostProcessing;



public class CamController : MonoBehaviour {


	public GameManager gameManager;
    LevelManager levelManager;


	Vector3 position0 = new Vector3();
    Vector3 rotation0 = new Vector3();
    float zoom0;
    
    private float offsetX;

	private float zoomPadding= -12f;
    private float zoomWeight = 5f;
    float maxZoomSize = 50.0f;
    float smallestZoomSize = 0f;
    public float xMultiplier = 2f;

    float fxMultiplier = 1.125f;
    float fxZoom = 1f;

    private float xDamp;
    private float zoomDamp;
	private float cameraSmoothe = 1.5f;

    public float playerWeight = 5f;
    public float ballWeight = 1f;
     public float aiWeight = 1f;

    public bool movable = false;

    private bool isGlitching;
    private bool isShaking;
    private float glitchIntensity;
    private float shakeIntensity; 
    private float shakeTime;
    private float glitchTime;
    //public float shakeAmp = .125f;
    private float shakeWeight = 2000f;
    float shakeViolence = 3f;
    float shakeSpeedMult = 100f;
    float shakeWeightTime = 300f;

    private bool normaled;

    public enum CameraType
    {
        Perspective,
        Main,
    }

    public CameraType type;

    float size0Persp;

    float size0Main;

    bool isZoomingToSide;

    float maxXpos = 20f;
     float minXpos = -15f;

    //lol

    //...  

    //lolol!

    void Start () {
		position0.x = gameObject.transform.position.x;
		position0.y = gameObject.transform.position.y;
		position0.z = gameObject.transform.position.z;

        rotation0.x = gameObject.transform.eulerAngles.x;
         rotation0.y = gameObject.transform.eulerAngles.y;
          rotation0.z = gameObject.transform.eulerAngles.z;


        gameManager = GlobalConfiguration.Instance.gameManager.GetComponent<GameManager>();

        levelManager = gameManager.levelManager;
        

        if (type == CameraType.Perspective)
        {
            levelManager.SetMainCamera(this);
             size0Persp = this.GetComponent<Camera>().fieldOfView;
        }
        else
        {
            levelManager.SetPerspCamera(this);
            size0Main = this.GetComponent<Camera>().orthographicSize;
        }


    }

    // Update is called once per frame
    void LateUpdate () {


    UpdateGame();

    UpdateShake();
       
    UpdateFX();

    UpdateZoomToSide();
       
		
	} 

    void UpdateGame() {
        if (levelManager.isPlaying)
        {
            if (movable)
            {
                if (isShaking == false && !isZoomingToSide)
                {          
                    Vector3 average = new Vector3(GetAverage(), 0.0f, 0.0f) * xMultiplier;                                                               // blows up if there ant any balls or players
                   // float deltaX = (average.x - transform.position.x) * xWeight;
                    float nuX = Mathf.SmoothDamp(transform.position.x, average.x, ref xDamp, cameraSmoothe/ (fxZoom * fxZoom));
                    nuX = Mathf.Clamp(nuX, minXpos,maxXpos);
                    gameObject.transform.position = new Vector3(nuX, position0.y, position0.z);
                }

                float nuSize = Mathf.Clamp((Mathf.Abs((MaxDistance() / (zoomWeight * fxZoom ))) + zoomPadding), smallestZoomSize, maxZoomSize);
                if (type == CameraType.Perspective)
                {
                    float sizePrev = this.GetComponent<Camera>().fieldOfView;
                    this.GetComponent<Camera>().fieldOfView = Mathf.SmoothDamp(sizePrev, (nuSize + size0Persp) / 2f, ref zoomDamp, cameraSmoothe / Mathf.Pow(fxZoom,fxZoom));
                }
                else
                {
                    float sizePrev = this.GetComponent<Camera>().orthographicSize;
                    //print("size = " +  (nuSize + size0Main)/2f);
                    this.GetComponent<Camera>().orthographicSize = Mathf.SmoothDamp(sizePrev, (nuSize + size0Main) / 2f, ref zoomDamp, cameraSmoothe / Mathf.Pow(fxZoom, fxZoom));
                }

            }
        }
    }

    void UpdateShake() {
         if (isShaking)
        {
            Shake(shakeTime);
            shakeTime-= Time.deltaTime;
        }

        if (shakeTime <=0 )
        {
            isShaking = false;

        }
    }


    void UpdateFX() {
    if (isGlitching)
        {
            
        }
    }


     void UpdateZoomToSide() {
        if (isZoomingToSide){
            if (levelManager.isPlaying == true) {
                isZoomingToSide = false;
            }
        }
     }

	float MaxDistance() {
		float min = 1000000.0f;
		float max = -1000000.0f;

        float fxCount = 0;

        foreach (GameObject player in levelManager.GetPlayers()) {
            
            Player playerComp = player.GetComponent<Player>();


			if (player.transform.GetChild (0).position.x < min) {
				min = player.transform.GetChild (0).position.x;
                 if (!playerComp.hasAI)
            {
                min *= playerWeight;
            }
            else {
                min *= aiWeight;
            }

			}

			if (player.transform.GetChild (0).position.x > max) {
				max = player.transform.GetChild (0).position.x;
                if (!playerComp.hasAI)
            {
                max *= playerWeight;
            }
            else {
                max *= aiWeight;
            }
			}

            
            if (!playerComp.hasAI)
            {
                if (playerComp.controller3DObject.GetComponent<Controller3D>().ballCaught)
                {
                    fxCount += 2;
                   // print("fxMultiplier = " + fxMultiplier);
                }
            }
            else
                {
                    if (playerComp.aiObject.GetComponent<AI>().ballCaught)
                    {
                        fxCount += 2;
                    }
            }
        }

		foreach (GameObject ball in gameManager.levelManager.balls) {
			if (ball.transform.position.x < min) {
				min = ball.transform.position.x;
			}
			if (ball.transform.position.x > max) {
				max = ball.transform.position.x;
			}

            Ball ballComp = ball.GetComponent<Ball>();
            if (ballComp.contact || ballComp.isSupering)
            {
                //fxCount += 1;   
                // print("fxMultiplier = " + fxMultiplier);
            }

        }

        fxZoom = (fxCount * fxMultiplier) + 1;

        //print("fxZoom = " + fxZoom);


        float maxDistance = (max - min) * xMultiplier;
       // print("maxDistance = " + maxDistance);

        return (maxDistance);
	}

	public float GetAverage(){
		float sum = 0.0f;
        float charWeight;
		float objectsCount = gameManager.levelManager.balls.Count +  levelManager.GetPlayers().Count;

        List<float> contactedBallsX = new List<float>();
        List<float> ballCaughtPos = new List<float>();

        foreach (GameObject ball in gameManager.levelManager.balls) {

            Ball ballComp = ball.GetComponent<Ball>();

            if (ballComp.contact || (ballComp.isSupering && ballComp.isCharging))
            {
                contactedBallsX.Add(ball.transform.position.x);
                sum += ball.transform.position.x * ballWeight *2f;
            }
            else
            {
                sum += ball.transform.position.x * ballWeight;
            }

            

        }
		foreach (GameObject player in levelManager.GetPlayers()) {

            float fxWeight = 2f;

            Player playerComp = player.GetComponent<Player>();

            if (playerComp.hasAI)
            {
                charWeight = aiWeight;
                if (playerComp.aiObject.GetComponent<AI>().ballCaught)
                {

                    ballCaughtPos.Add(playerComp.playerConfigObject.transform.position.x);
                    charWeight += fxWeight;
                }
            }
            else
            {
              
                charWeight = playerWeight ;
                if (playerComp.controller3DObject.GetComponent<Controller3D>().ballCaught)
                {
                    ballCaughtPos.Add(playerComp.playerConfigObject.transform.position.x);
                    charWeight += fxWeight;
                }
            }
            sum += playerComp.playerConfigObject.transform.position.x * charWeight;
		}

        float averageXPosition = sum / (objectsCount);


        if (ballCaughtPos.Count > 0)
        {
            foreach (float ballPositionX in ballCaughtPos)
            {
                averageXPosition = (averageXPosition + ballPositionX) / 2;
            }
        }


        if (contactedBallsX.Count > 0)
        {
            foreach (float ballPositionX in contactedBallsX)
            {
                averageXPosition = (averageXPosition + ballPositionX) / 2;
            }
        }



        return averageXPosition;

    }

    public void Shake( float time) 
    {
        float nuX = transform.position.x + shakeIntensity * Mathf.Sin(Time.realtimeSinceStartup * shakeSpeedMult);
         transform.position = new Vector3 (Mathf.Lerp(transform.position.x,nuX,cameraSmoothe * shakeViolence), transform.position.y, transform.position.z);
           
    }

    public void ZoomToSide(int side)
    {

        isZoomingToSide = true;


        if (isShaking)
        {
            isShaking = false;
        }

        if (side == 1)
        {
           Quaternion camRot =  gameObject.GetComponent<Camera>().transform.localRotation;
            //camRot.y = -5;
            float nuX = Mathf.SmoothDamp(transform.position.x, GetTeamAverageDistance(side), ref xDamp, cameraSmoothe);
            gameObject.transform.position = new Vector3(nuX, position0.y, position0.z);
            if (type == CameraType.Main) {
            float size0 = this.GetComponent<Camera>().orthographicSize;
           
            this.GetComponent<Camera>().orthographicSize  = Mathf.SmoothDamp(size0, 16, ref zoomDamp, cameraSmoothe);
            }
            else {
                 float size0 = this.GetComponent<Camera>().fieldOfView;
                this.GetComponent<Camera>().fieldOfView = Mathf.SmoothDamp(size0, 16, ref zoomDamp, cameraSmoothe);
            }
        }

        if (side == 2)
        {
            Quaternion camRot = gameObject.GetComponent<Camera>().transform.localRotation;
           // camRot.y = 10;
             float nuX = Mathf.SmoothDamp(transform.position.x, GetTeamAverageDistance(side), ref xDamp, cameraSmoothe);
            gameObject.transform.position = new Vector3(nuX, position0.y, position0.z);

           if (type == CameraType.Main) {
            float size0 = this.GetComponent<Camera>().orthographicSize; 
           this.GetComponent<Camera>().orthographicSize  = Mathf.SmoothDamp(size0, 16, ref zoomDamp, cameraSmoothe);
            }
            else {
                 float size0 = this.GetComponent<Camera>().fieldOfView;
                this.GetComponent<Camera>().fieldOfView = Mathf.SmoothDamp(size0, 16, ref zoomDamp, cameraSmoothe);
            }
        }
        }

    internal void Normal()
    {
        if (type == CameraType.Main) {
                gameObject.GetComponent<Camera>().orthographicSize = size0Main; 
        }
        else {
            gameObject.GetComponent<Camera>().fieldOfView = size0Persp; 
        }

        gameObject.transform.position = position0;
        gameObject.transform.eulerAngles = rotation0;
         
        
    }

    internal void TrigCamShake(float intensity, Transform playerTrans)
    {
        isShaking = true;
        shakeTime = Mathf.Clamp(intensity/ shakeWeightTime, 0f,8f);
        shakeIntensity = Mathf.Clamp(intensity/shakeWeight,0f,2f);
        //print("shakeIntensity = " + shakeIntensity);
       // print("shakeTime = " + shakeTime);

    }

    internal void ActivateGlitch(float intensity)
    {
        isGlitching = true;
        glitchIntensity = intensity;

    }

    float GetTeamAverageDistance(int team) {
        float averageDistance = 0f;
        int playerCount = 0;
         float distance = 0f;

        foreach (GameObject player in levelManager.GetPlayers()) {
           
            if (player.GetComponent<Player>().team == team)
            {
                distance += player.GetComponent<Player>().playerConfigObject.transform.position.x;
                playerCount ++;
            }
		}
         averageDistance =  distance / (playerCount);
        // print("distance = "+ distance);
         //print("playerCount = "+ playerCount);

		return averageDistance;
	}

    
}
