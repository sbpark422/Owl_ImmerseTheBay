using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //check if came from main menu basically
    // if not instantiate default
    //instantiate rest of players if needed..  asign appropriate player numbers and object names ... i.e "Player 1,2,3,4" etc.. 
    // get referee from game rule
    // handle spawn 
    // handle UI locations

    GameManager gameManager;
    FXManager fXManager;
    public AudioManager audioManager;

    string gameMode;

    GameRule gameRule;

    public Stage stage;                // make privates
    public CanvasGeneralGame cgg;

    CamController mainCamController;
    CamController perspCamController;

    bool isAtScene;

    int sceneIndex = 0;

    Dictionary<int, bool> scenesVisited = new Dictionary<int,bool>();    // populate w indexes initialized w false values


   public TeamManager tm1;
   public TeamManager tm2;

    public List<GameObject> balls = new List<GameObject>();
    List<Vector3> ballSpwanLocations;

    List<GameObject> tm1UIs;
    List<GameObject> tm2UIs;


    GameObject logFeed;

    GameObject timerObj;
    int timer;

    bool start;

    int round =1 ;

    private bool ready;
    public bool isPlaying;
    public bool gameOver;

    bool isCelebrating;
    float celebrationTime = 5f;
    int countDownNum = 3;
    float countDown;

    public GameObject goFX;

    GameObject tm1ScoreObj;
    GameObject tm2ScoreObj;

    public bool team1Wins;
    public bool team2Wins;

    public int team1Points;
    public int team2Points;

    bool team1Scored;
    bool team2Scored;


    //Arcade Mode

    ArcadeMode arcadeScript;
    public int roundLevel;

    float difficultyScaler = 3f;
    int throwMag;
    int throwDecScalar;


    public Dictionary<GameObject, GameObject> throws = new Dictionary<GameObject, GameObject>();
    public Dictionary<GameObject, HashSet<GameObject>> hits = new Dictionary<GameObject, HashSet<GameObject>>();
    public Dictionary<GameObject, GameObject> catches = new Dictionary<GameObject, GameObject>();
    public List<Dictionary<int[], string>> outLog = new List<Dictionary<int[], string>>();

     GameObject[] powerBalls;
     GameObject speedBall;

    PostGameScreen postGameScript;


    void Start()
    {
        print("LevelManager start");

        {
            gameManager = GlobalConfiguration.Instance.gameManager.GetComponent<GameManager>();
        }

        // if not fx
        fXManager = GameObject.Find("FXManager").GetComponent<FXManager>();
         audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        
        // if not tms
        tm1 = GlobalConfiguration.Instance.team1Object.GetComponent<TeamManager>();
        tm2 = GlobalConfiguration.Instance.team2Object.GetComponent<TeamManager>();

        countDown = countDownNum;


        /* P1Icon.GetComponent<Image>().sprite = Player1.GetComponent<Player>().playerIconImage;
         P2Icon.GetComponent<Image>().sprite = Player2.GetComponent<Player>().playerIconImage;
         P3Icon.GetComponent<Image>().sprite = Player3.GetComponent<Player>().playerIconImage;
         P4Icon.GetComponent<Image>().sprite = Player4.GetComponent<Player>().playerIconImage;

         if (Player1.GetComponent<Player>().staminaBarObject)
         {
             P1StaminaBar = Player1.GetComponent<Player>().staminaBarObject;
         }

         P1StaminaBar.SetActive(true);
         P1PowerBar = Player1.GetComponent<Player>().powerBarObject;
         P1PowerBar.SetActive(true);
         P2StaminaBar = Player2.GetComponent<Player>().staminaBarObject;
         P2StaminaBar.SetActive(true);
         P2PowerBar = Player2.GetComponent<Player>().powerBarObject;
         P2PowerBar.SetActive(true);
         P3StaminaBar = Player3.GetComponent<Player>().staminaBarObject;
         P3StaminaBar.SetActive(true);
         P3PowerBar = Player3.GetComponent<Player>().powerBarObject;
         P3PowerBar.SetActive(true);
         P4StaminaBar = Player4.GetComponent<Player>().staminaBarObject;
         P4StaminaBar.SetActive(true);
         P4PowerBar = Player4.GetComponent<Player>().powerBarObject;
         P4PowerBar.SetActive(true);
        */
    }


    void Update()
    {

        if (ready && !isPlaying)
        {
            countDown =  (countDown - Time.deltaTime);

           // tm1.MoveToSpawnPoints(stage.tm1_spawnPoints);
           // tm2.MoveToSpawnPoints(stage.tm2_spawnPoints);

            if (countDown <= 0)
            {
                isPlaying = true;
             //   InstantiateGoFX();
              //  PlayWhistle();
            }
        }

        if (start)
        {
           
        //    Text tm1ScoreText = tm1ScoreObj.GetComponent<Text>();
        //    Text tm2ScoreText = tm2ScoreObj.GetComponent<Text>();

         //   tm1ScoreText.text = ((int)team1Points).ToString();
       //     tm2ScoreText.text = ((int)team2Points).ToString();

            if (!GameOver())
            {
                Referee();
              //   UpdateDamMeter();

            }

            else    //gameOver
            {
                
                ready = false;

                 Invoke("SetIsPlayingFalse", 1f);

                if (team1Wins == false && team2Wins == false)
                {

                    celebrationTime = (celebrationTime - Time.deltaTime);

                    if (team1Scored)
                    {
      
                    }
                    else
                    {
                        if (team2Scored)
                        {
                           
                        }
                    }

                    if (celebrationTime <= 0)
                    {
                       //  Invoke("GameRestart", .5f);
                        GameRestart();

                    }
                }

                else
                {
                    if (gameOver)
                    {
                        start = false;                     

                        if (gameMode == "multiplayer")
                        {                       
                            Invoke("EndGameMenu", 3f);
                        }

                        if (gameMode == "arcade") { 
                                                 

                            if (team1Wins)
                            {
                                
                            }

                            else
                            {

                                // continue gameMode
                                // yes -> restart , no gameMode
                                //  Invoke("EndGameMenu", 3f); //< - temp
                                Invoke("PostGameLoseScreen", .5f);
                            }

                        }
                        team1Wins = false;
                        team2Wins = false;
                    }
                }

            }

        }       
           
    }
    void PostGameWinScreen()
    {
        postGameScript.arcadeWin(tm1.players[0].GetComponent<Player>().type);
    }

    void PostGameLoseScreen()
    {
        postGameScript.arcadeDefeat(tm1.players[0].GetComponent<Player>().type);
        postGameScript.SelectRestartArcadeDefeatButton();
    }

    void LoadNextArcadeScene()
    {
        SceneManager.LoadScene(GetArcadeSceneName());
    }
    internal bool GetSceneVisited(int buildIndex)
    {
       if (scenesVisited.ContainsKey(buildIndex))
        {
            return true;
        }
       else
        {
            return false;
        }
    }

    private void ResetTeam1Animators()
    {   
            foreach (GameObject player in tm1.players)
            {
                
            Animator playerAnim = player.transform.GetChild(0).gameObject.GetComponent<PlayerConfiguration>().animator;

            playerAnim.SetBool("Running", false);
            playerAnim.SetBool("hasBall", false);

        }
    }

    private void ResetTeam2Animators()
    {
        foreach (GameObject player in tm2.players)
        {
            Animator playerAnim = player.transform.GetChild(0).gameObject.GetComponent<PlayerConfiguration>().animator;
            playerAnim.SetBool("Running", false);
            playerAnim.SetBool("hasBall", false);

        }
    }


        internal void SetStart(bool v)
    {
        start = v;
    }


    private void Referee()
    {
      	CheckHits();

       // if (Gamepad.all.Count <= 1)    // doesn't take into account reconnectivity
        {
            CheckTeamHasPlayer();
        }
   


    }
    private void CheckHits()
    {


        if (hits.Count > 0)
        {
            List<GameObject> toRemove = new List<GameObject>();
            foreach (GameObject ball in hits.Keys)
            {
                if (ball.GetComponent<Ball>().grounded)
                {
                    foreach (GameObject player in hits[ball])
                    {
                        GameObject throwerObject = throws[ball];
                        Player throwerPlayerScript = throwerObject.GetComponent<Player>();
                        throwerPlayerScript.TriggerWinFX();

                        player.GetComponent<Player>().isOut = true;                          // done twixce in remove... not sure where to put
                        // OutDisplayX2 (player.transform.GetChild (0).transform,ball);

                        String outType = "h";
                        Remove(player, outType, ball);
                        audioManager.PlayOuts();
                        fXManager.InstantiateReboundFX(ball, throwerObject);

                    }
                    if (hits.ContainsKey(ball))
                    {
                        toRemove.Add(ball);
                    }

                    ClearContacts(ball);
                }
            }
            foreach (GameObject ball in toRemove)
            {
                hits.Remove(ball);
                RemoveThrow(ball);
            }
        }
    }

    internal void SetCurrentOpp(string firstOppChar)
    {
        arcadeScript.SetCurrentOpp(firstOppChar);  // should be the last in facedOpps index
    }

    internal List<GameObject> GetPlayers()
    {
        if (!tm1|| !tm2)
        {
            tm1 = GlobalConfiguration.Instance.team1Object.GetComponent<TeamManager>();
            tm2 = GlobalConfiguration.Instance.team2Object.GetComponent<TeamManager>();
        }

        List<GameObject> returnMe = new List<GameObject>();

        returnMe.AddRange(tm1.players);
        returnMe.AddRange(tm2.players);

      //  print("players.Size = " + returnMe.Count);

        return returnMe;
    }


    internal List<GameObject> GetUsers()
    {
        List<GameObject> returnMe = new List<GameObject>();

        foreach (GameObject player in tm1.players)
        {
            if (player.GetComponent<Player>().hasJoystick)
            {
                returnMe.Add(player);
            }
        }

        //  print("players.Size = " + returnMe.Count);

        return returnMe;
    }
    private void InstantiateGoFX()
    {
        Instantiate(goFX);
    }

    public void LoadLevel()
    {
        
        SetTeamSpawnLocations(1, stage.GetSpawnLocations(1, tm1.GetPlayerCount()));
        SetTeamSpawnLocations(2, stage.GetSpawnLocations(2, tm2.GetPlayerCount()));
       // SetTeamRetreatPoints(1, stage.GetSpawnLocations(1, tm1.playerCount));
      //  SetTeamRetreatPoints(1, stage.GetSpawnLocations(2, tm2.playerCount));
        SetPlayerUI(1, tm1.GetPlayerCount());
        SetPlayerUI(2, tm2.GetPlayerCount());
        SetTeamScoreUI(1);
        SetTeamScoreUI(2);

        SetLogFeed();
        SetTimer();

        InstantiateBalls();

        tm1.StandByPlayers(false);
        tm2.StandByPlayers(false);

        //GetReferee or game rule

        start = true;
        ready = true;                                         // just needed for a few older references.. should strike to see when updating

        postGameScript = GameObject.Find("PostGamePanelContainer").GetComponent<PostGameScreen>();

        print("Level Loaded");
    }

    internal GameRule CreateGameRule(string ruleType)
    {
        if (ruleType == "basic")
        {
            gameRule = new GR_Basic(gameMode, this);
        }

        if (ruleType == "stock")
        {
            gameRule = new GR_Stock(gameMode, this);
        }

        if (ruleType == "arcade")
        {
            gameRule = new GR_Basic(gameMode, this);
            gameRule.SetRoundsToWin(3);
        }

        return gameRule;
    }

    private void SetTeamScoreUI(int team)
    {
       if (team == 1)
        {
            tm1ScoreObj = cgg.GetTeamScoreObj(1);
        }


        if (team == 2)
        {
            tm2ScoreObj = cgg.GetTeamScoreObj(2);
        }
    }

    private void SetLogFeed()
    {
        logFeed = cgg.GetLogFeedObj();
    }

    private void SetTimer()
    {
        timerObj = cgg.GetTimerObj();
    }

    private void SetPlayerUI(int team, int playerCount)
    {
        

        if (team == 1)
        {
            List<GameObject> playerUIs = cgg.GetPlayerUIs(1);

            for (int i=0; i < playerCount; i++)
            {
                playerUIs[i].GetComponent<Image>().sprite = tm1.players[i].GetComponent<Player>().playerIconImage;
                playerUIs[i].SetActive(true);

                GameObject playerPower = playerUIs[i].transform.GetChild(0).gameObject;
                playerPower.SetActive(true);
                tm1.players[i].GetComponent<Player>().powerBarObject = playerPower ;
               

                GameObject playerStamina = playerUIs[i].transform.GetChild(1).gameObject;
                playerStamina.SetActive(true);
                tm1.players[i].GetComponent<Player>().staminaBarObject  =   playerStamina ;
            
            }
        }
        if (team == 2)
        {
            List<GameObject> playerUIs = cgg.GetPlayerUIs(2);

            for (int i = 0; i < playerCount; i++)
            {
                playerUIs[i].GetComponent<Image>().sprite = tm2.players[i].GetComponent<Player>().playerIconImage;
                playerUIs[i].SetActive(true);

                GameObject playerPower = playerUIs[i].transform.GetChild(0).gameObject;
                playerPower.SetActive(true);
                tm2.players[i].GetComponent<Player>().powerBarObject = playerPower;


                GameObject playerStamina = playerUIs[i].transform.GetChild(1).gameObject;
                playerStamina.SetActive(true);
                tm2.players[i].GetComponent<Player>().staminaBarObject = playerStamina;
            }

        }
        
    }

    private void InstantiateBalls()
    {
        List<Vector3> ballSpwanLocations = stage.GetBallSpawnLocations(gameRule.ballCount);
        
        balls = new List<GameObject>();

        for (int i =0; i< gameRule.ballCount; i++)
        {
         //   print("location = " + ballSpwanLocations[i]);
            GameObject ball = GlobalConfiguration.Instance.InstantiateBallPrefab(ballSpwanLocations[i]);
            ball.GetComponent <Ball>().floorMarker.GetComponent<FloorMarker>().SetGroundObject(stage.playingLevelPlane);
            balls.Add(ball);

        }
    }

    public void SetStage(Stage x)
    {
        stage = x;
    }

    public void SetUICanvas(CanvasGeneralGame x)
    {
        cgg = x;
    }

    internal void SetMainCamera(CamController cC)
    {
        mainCamController = cC;
    }

    internal void SetPerspCamera(CamController cC)
    {
        perspCamController = cC;
    }

    public void SetMode(string mode)
    {
        /*
       // if (mode == "Basic")
        {
            MyJoystick.mode = mode;
            Controller3D.easyMove = true;
            Controller3D.hasThrowMag = true;
            Controller3D.throwMagnetism = throwMag;
            Controller3D.hasGrabMag = true;
            Controller3D.grabMag = grabMag;
            Controller3D.maxSeekVec = maxseekVec;
        }
        */

    }

    internal void SetIsAtScene(bool v)
    {
        isAtScene = v;


    }

    internal void SetSceneIndex(int x)
    {
        sceneIndex = x;

      if( scenesVisited.ContainsKey(sceneIndex)) {
            print("Scene  index " + sceneIndex + " visited");
        }
      else
        {
            scenesVisited.Add(sceneIndex, true);
        }

        print("Scene Index: " + x + " loaded..");
    }

    internal void SetTeamSpawnLocations(int team, List<Vector3> spawnPoints)
    {
        if (team == 1)
        {
            tm1.MoveToSpawnPoints(spawnPoints);
            tm1.FaceOpp(1);
        }
        if (team == 2)
        {
            tm2.MoveToSpawnPoints(spawnPoints);
            tm2.FaceOpp(2);
        }
    }

    internal void SetTeamRetreatPoints(int team, List<Vector3> spawnPoints)
    {
        if (team == 1)
        {
            tm1.SetRetreatPoints(spawnPoints);
        }
        if (team == 2)
        {
            tm2.SetRetreatPoints(spawnPoints);

        }
    }

    internal bool IsInGameBounds(Vector3 cockBackPos)
    {
        return stage.IsInGameBounds(cockBackPos);
    }

    internal void SetGameMode(string v)
    {
        gameMode = v;

        if (gameMode == "arcade")
        {
            CreateGameRule("arcade");
            arcadeScript = new ArcadeMode(this);
            print("Arcade");
        }

        if (gameMode == "multiplayer")
        {
            CreateGameRule("basic");
            print("multiplayer");
        }
    }

    internal void SetGameRule(string v)
    {
      //  gameRule = v;
    }

    public GameRule GetGameRule()
    {
        return gameRule;
    }

    public void AddHit(GameObject ball, GameObject player)
    {
        // TODO Chain hits
        // i think a new world of logic presents itself when a ball hits multiplayers or multiple balls hit a player

        print("Adding hit");
        if (!hits.ContainsKey(ball))
        {
            HashSet<GameObject> players = new HashSet<GameObject>();
            players.Add(player);
            hits.Add(ball, players);
        }
        else
        {
            hits[ball].Add(player);
        }
    }

    public void AddThrow(GameObject ball, GameObject player)
    {
        if (!throws.ContainsKey(ball))
        {
            throws.Add(ball, player);
        }
    }


    internal void AddCatch(GameObject ball, GameObject parent)
    {
        catches.Add(ball, parent);
    }

    public void RemoveHit(GameObject ball)
    {
        if (hits.ContainsKey(ball))
        {
            hits.Remove(ball);

        }
    }

    public void RemoveThrow(GameObject ball)
    {
        if (throws.ContainsKey(ball))
        {
            throws.Remove(ball);
        }
    }

    public void LastThrowerOut(GameObject ball)
    {
        throws[ball].GetComponent<Player>().isOut = true;
        String outType = "c";
        Remove(throws[ball], outType, ball);
        ball.GetComponent<Ball>().DeactivateThrow();
        audioManager.PlayCatches();
    }

    public void Remove(GameObject player, String outType, GameObject ball)
    {
       
        if (outType == "c")
        {
            int throwerNumber = player.GetComponent<Player>().number;
            int catcherNumber = catches[ball].GetComponent<Player>().number;
            int[] array = new int[2];
            array[0] = catcherNumber;
            array[1] = throwerNumber;
            Dictionary<int[], String> dict = new Dictionary<int[], string>();
            dict.Add(array, outType);
            outLog.Add(dict);
            LogOuts();
            RemoveCatch(ball);
        }
        else
                if (outType == "h")
        {
            int throwerNumber = throws[ball].GetComponent<Player>().number;
            int outNumber = player.GetComponent<Player>().number;
            int[] array = new int[2];
            array[0] = throwerNumber;
            array[1] = outNumber;
            Dictionary<int[], String> dict = new Dictionary<int[], string>();
            dict.Add(array, outType);
            outLog.Add(dict);
            LogOuts();
        }

        player.GetComponent<Player>().DisablePlayer();
        player.GetComponent<Player>().PlayOutSound();



    }

    private void RemoveCatch(GameObject ball)
    {
        catches.Remove(ball);
    }

    public void GetAnotherPlayer(int team)           // GR check
    {
        
        bool somebodyOut = false;

        if (team == 1)
        {
            foreach (GameObject player in tm1.players)
            {
                if (player.GetComponent<Player>().isOut)
                {
                    somebodyOut = true;
                }
            }
            if (somebodyOut == true)
            {
                BringPlayerIn(1);
            }
        }
        if (team == 2)
        {
            foreach (GameObject player in tm2.players)
            {
                if (player.GetComponent<Player>().isOut)
                {
                    somebodyOut = true;
                }
            }
            if (somebodyOut == true)
            {
                BringPlayerIn(2);
            }
        }
        
    }

    public void BringPlayerIn(int team)
    {
        int index = 0;

        if (team == 1)
        {
            foreach (GameObject player in tm1.players)
            {
                if (player.GetComponent<Player>().isOut)
                {
                    index = tm1.players.IndexOf(player);
                    break;
                }
            }
            if (tm1.players[index].GetComponent<Player>().hasAI)
            {
                tm1.players[index].GetComponentInChildren<AI>().enabled = true;
                tm1.players[index].GetComponentInChildren<NavMeshAgent>().enabled = true;
                tm1.players[index].GetComponentInChildren<Rigidbody>().isKinematic = true;
            }
            else
            {
                tm1.players[index].GetComponentInChildren<Controller3D>().enabled = true;
                tm1.players[index].GetComponent<Player>().playerAura.SetActive(true);
                tm1.players[index].GetComponentInChildren<Rigidbody>().isKinematic = false;
            }

            tm1.players[index].transform.GetChild(1).gameObject.SetActive(true);
            tm1.players[index].transform.GetChild(0).transform.position = tm1.players[index].GetComponent<Player>().childPos0;
            tm1.players[index].GetComponentInChildren<SpriteRenderer>().enabled = true;
            tm1.players[index].GetComponentInChildren<CapsuleCollider>().enabled = true;
            tm1.players[index].GetComponentInChildren<SphereCollider>().enabled = true;
            tm1.players[index].GetComponentInChildren<Rigidbody>().useGravity = true;
            tm1.players[index].GetComponent<Player>().isOut = false;
            tm1.players[index].GetComponent<Player>().shadow.SetActive(true);
            print(" bringing 1 in");
        }
        if (team == 2)
        {
            foreach (GameObject player in tm2.players)
            {
                if (player.GetComponent<Player>().isOut)
                {
                    index = tm2.players.IndexOf(player);
                    break;
                }
            }
            if (tm2.players[index].GetComponent<Player>().hasAI)
            {
                tm2.players[index].GetComponentInChildren<AI>().enabled = true;
                tm2.players[index].GetComponentInChildren<NavMeshAgent>().enabled = true;
                tm2.players[index].GetComponentInChildren<Rigidbody>().isKinematic = true;
            }
            else
            {
                tm2.players[index].GetComponentInChildren<Controller3D>().enabled = true;
                tm2.players[index].GetComponentInChildren<Rigidbody>().isKinematic = false;
                tm2.players[index].GetComponent<Player>().playerAura.SetActive(true);
            }

            tm2.players[index].transform.GetChild(1).gameObject.SetActive(true);
            tm2.players[index].transform.GetChild(0).transform.position = tm2.players[index].GetComponent<Player>().childPos0;
            tm2.players[index].GetComponentInChildren<SpriteRenderer>().enabled = true;
            tm2.players[index].GetComponentInChildren<CapsuleCollider>().enabled = true;
            tm2.players[index].GetComponentInChildren<SphereCollider>().enabled = true;
            tm2.players[index].GetComponentInChildren<Rigidbody>().useGravity = true;
            tm2.players[index].GetComponent<Player>().isOut = false;
            tm2.players[index].GetComponent<Player>().shadow.SetActive(true);
            print(" bringing 2 in");
        }
    }

    internal void ClearContacts(GameObject ball)
    {
        if (hits.ContainsKey(ball))
        {
            foreach (GameObject player in hits[ball])
            {
                Player playerComp = player.GetComponent<Player>();
                PlayerConfiguration pConfig = playerComp.playerConfigObject.GetComponent<PlayerConfiguration>();
                pConfig.RemoveContact();

                playerComp.SetHitFX(false);

                    /*
                    ParticleSystem ps = player.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();                    //gr land ... I dont even think this gets unnormalized
                    ParticleSystem.MainModule ring_ps = ps.main;
                    ParticleSystem ps_2 = player.transform.GetChild(0).gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule spikes_ps = ps_2.main;
                    spikes_ps.simulationSpeed = 1;
                    ring_ps.simulationSpeed = 1;
                    */
                
            }
        }
    }

    private void LogOuts()
    {
        int outer = 0;
        int scorer = 0;
        string type = "";
        foreach (var group in outLog[outLog.Count - 1])
        {
            scorer = group.Key[0];
          //  print("scorer = " + scorer);
            outer = group.Key[1];
          //  print("outer = " + outer);
            type = (group.Value);
          //  print("type = " + type);
        }

        logFeed.GetComponent<GameLog>().AddToFeed(scorer, outer, type);
    }

            public void PlayWhistle()
            {
        audioManager.PlayWhistle();
                }

    public void PlayCheer()
    {
        audioManager.PlayCheer();
    }



    public void PlayOuts()
    {
        audioManager.PlayOuts();
    }

    public void PlayCatches()
    {
        audioManager.PlayCatches();
    }



    public void PlayDamn()
    {
        audioManager.PlayDamn();
    }





    public void TriggerHitFX(GameObject hittee, GameObject ball)
    {
        fXManager.HitDisplay(hittee, ball);

        GameObject hitter = throws[ball].transform.GetChild(0).gameObject;

        Player hitterPlayerScript = hitter.GetComponentInParent<Player>();


        Color terColor = hitterPlayerScript.color;

        Color teeColor = hittee.GetComponentInParent<Player>().color;

        Ball ballScript = ball.GetComponent<Ball>();

        ballScript.ActivateTrail(terColor, teeColor);

        ballScript.SetActiveHitFX(true);

        ballScript.SetActiveFloorMarker(true);

    }

    public void CatchDisplay(Vector3 position)
    {
        fXManager.CatchDisplay(position);
    }
    internal void CatchDisplay(Color c, Vector3 position, float velocity)
    {
        fXManager.CatchDisplay(c,position, velocity);
    }

    internal void OutDisplay(GameObject gameObject)
    {
       fXManager.OutDisplay(gameObject);
    }

    public void WinDisplay(Vector3 position)
    {
        fXManager.WinDisplay(position);
    }
    public void HitPause(float duration)
    {
        fXManager.HitPause(duration);
    }

    internal void HitPause()
    {
        fXManager.HitPause();
    }

    internal void SetHitPauseDuration(float hitPauseDur)
    {
        fXManager.SetHitPauseDuration(hitPauseDur);
    }
    

    public GameObject GetLogFeed()
    {
        return logFeed;
    }

    public void TeamScored(int team)
    {
        if (team == 1)
        {
            team1Points++;
        }

        if (team == 2)
        {
            team2Points++;
        }
    }
    internal void PostFX(string type)
    {
        if (type == "Player1Hit")
        {

        }
    }

    public void GameRestart(string reset)
    {

        round = 1; 
        ready = true;
        countDown = countDownNum;
        timer = 0;
        isCelebrating = false;
        mainCamController.GetComponent<CamController>().Normal();
        perspCamController.GetComponent<CamController>().Normal();
        hits.Clear();
        throws.Clear();
        team1Scored = false;
        team2Scored = false;


        // List<GameObject> players = new List<GameObject>();
        //  players.AddRange(tm1.players);
        //   players.AddRange(tm2.players);




        foreach (GameObject player in GetPlayers())
        {


            player.GetComponentInChildren<Rigidbody>().isKinematic = true;
            player.GetComponentInChildren<Animator>().runtimeAnimatorController = player.GetComponentInChildren<PlayerConfiguration>().play;

            if (player.GetComponent<Player>().hasJoystick)
            {

                if (player.GetComponentInChildren<Controller3D>().ballGrabbed == true)
                {
                    player.GetComponentInChildren<Controller3D>().DropBall();
                    player.GetComponentInChildren<Controller3D>().NormalAccelerationRate();
                }
            }
            else
            {
                if (player.GetComponentInChildren<AI>())
                {
                    if (player.GetComponentInChildren<AI>().ballGrabbed)
                    {
                        player.GetComponentInChildren<AI>().DropBall();
                    }
                }
            }

            player.transform.GetChild(1).gameObject.SetActive(true);
            player.transform.GetChild(0).transform.position = Vector3.zero;
            player.GetComponent<Player>().isOut = false;
            player.GetComponentInChildren<SpriteRenderer>().enabled = true;
            player.GetComponentInChildren<CapsuleCollider>().enabled = true;
            player.GetComponentInChildren<SphereCollider>().enabled = true;
            player.GetComponentInChildren<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            player.GetComponentInChildren<Rigidbody>().useGravity = true;
            player.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);

            if (player.GetComponent<Player>().hasAI)
            {

                player.GetComponentInChildren<AI>().enabled = true;
                player.GetComponentInChildren<UnityEngine.AI.NavMeshAgent>().enabled = true;
            }

            else
            {
                  player.GetComponentInChildren<Animator>().runtimeAnimatorController = player.GetComponentInChildren<PlayerConfiguration>().play;
                player.GetComponentInChildren<Rigidbody>().isKinematic = false;
                player.GetComponentInChildren<Controller3D>().enabled = true;
                Controller3D.throwMagnetism = throwMag;
              

            }

        }

        if (gameMode == "arcade")
        {
            
            bool clearAdded = true;
            GlobalConfiguration.Instance.ClearPlayers(2, clearAdded);
            tm2.ClearAdded();
            stage.ClearSpawnpoints(2);
            SetPlayerUI(2, tm2.GetPlayerCount());
            
            

        }

        SetTeamSpawnLocations(1, stage.GetSpawnLocations(1, tm1.GetPlayerCount()));
        SetTeamSpawnLocations(2, stage.GetSpawnLocations(2, tm2.GetPlayerCount()));




        List<Vector3> ballSpwanLocations = stage.GetBallSpawnLocations(gameRule.ballCount);
        int i = 0;

        foreach (GameObject ball in balls)
        {
            ball.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            ball.GetComponent<SphereCollider>().enabled = true;
            ball.GetComponent<Ball>().DeactivateThrow();
            ball.transform.GetChild(2).gameObject.SetActive(false);
            ball.transform.position = ballSpwanLocations[i];

            i++;

        }

        if (round == 1)
        {
            // InstantiateSpeedBall();
        }

        PlayWhistle();
    }

    private void RemoveAI(GameObject player)
    {
        tm2.players.Remove(player);
        GlobalConfiguration.Instance.GetPlayers().Remove(player);
        Destroy(player);
    }

    private void IncreaseLevelDifficulty()   //  one sided, not modular
    {
        roundLevel++;

        // GR check 

        
        if (team1Scored)
        {
            // DecreaseThrowMag(difficultyScaler);
            if (tm2.GetPlayerCount() < GlobalConfiguration.Instance.maxTeamCount)
            {
              //  AddAI(2,arcadeScript.GetCurrentOppName());
            }

            foreach (GameObject player in tm2.players)
            {
                if (player.GetComponent<Player>().hasAI)
                {
                        {
                            IncreaseAIIntensity(player.GetComponentInChildren<AI>(), difficultyScaler);
                        }
                    }
                }
            
        }
/*
        else
        {

            AddAI(1);

            foreach (GameObject player in tm1.players)
            {
                if (player.GetComponent<Player>().hasAI)
                {
                    {
                        IncreaseAIIntensity(player.GetComponentInChildren<AI>(), difficultyScaler);
                    }
                }
            }
        }
*/
        
    }

    private void AddAI(int team,string charName)
    {
        int count = 1;

       if (team == 1)
        {
            List<GameObject> ai1_new = tm1.CreateAI(count, charName);
            int i = 0;
            foreach (GameObject ai1_ in ai1_new)
            {
                i++;
                Player pScript = ai1_.GetComponent<Player>();
                AI aiAIScript = pScript.aiObject.GetComponent<AI>();
                GlobalConfiguration.Instance.AddNewPlayer(ai1_);
                GlobalConfiguration.Instance.AddPlayerToTeamManager(ai1_, 1, false);
                pScript.SetColor(GlobalConfiguration.Instance.GetPlayerColor(i, pScript));
                aiAIScript.Init();
                aiAIScript.addedAtStage = true;
                aiAIScript.type = AI.Type.aggresive;
            }
        }

        if (team == 2)
        {
            List<GameObject> ai2_new = tm2.CreateAI(count,charName);
            int j = 0;
            foreach (GameObject ai2_ in ai2_new)
            {
                j++;
                Player p2Script = ai2_.GetComponent<Player>();
                AI ai2AIScript = p2Script.aiObject.GetComponent<AI>();
                GlobalConfiguration.Instance.AddNewPlayer(ai2_);
                GlobalConfiguration.Instance.AddPlayerToTeamManager(ai2_, 2, false);
                p2Script.SetColor(GlobalConfiguration.Instance.GetPlayerColor(j, p2Script));
                ai2AIScript.Init();
                ai2AIScript.addedAtStage = true;
                ai2AIScript.type = AI.Type.timid;
            }
        }


       
    }

    private void IncreaseAIIntensity(AI ai, float n)
    {
        ai.LevelIncrease(n);

    }

    private void DecreaseThrowMag(float difficultyScaler)
    {
        // GR check

       // if (mode == "Basic" && (Controller3D.throwMagnetism - throwDecScalar) >= 0.0)
        {
            Controller3D.throwMagnetism -= throwDecScalar;
        }

    }


    public void CamShake(float intensity, Transform playerT)
    {
        mainCamController.TrigCamShake(intensity, playerT);
        perspCamController.TrigCamShake(intensity, playerT);
    }

    internal void CamGlitch(float ballVelocity)
    {
        mainCamController.GetComponent<CamController>().ActivateGlitch(ballVelocity);
       // mainCamController.GetComponent<CamController>().ActivateGlitch(ballVelocity);
    }
    public void GameRestart()
    {

        round++;
        ready = true;
        countDown = countDownNum;
        timer = 0;
        celebrationTime = 5.0f;
        isCelebrating = false;
        mainCamController.Normal();
        perspCamController.Normal();
        hits.Clear();
        throws.Clear();

        if (gameMode == "arcade")
        {
            IncreaseLevelDifficulty();
        } 

        team1Scored = false;
        team2Scored = false;

        foreach (GameObject player in GetPlayers())                                       //   methodize
        {
            GameObject playerconfigObject = player.transform.GetChild(0).gameObject;

            playerconfigObject.SetActive(true);
            player.GetComponent<Player>().isOut = false;

            Animator playerAnim = playerconfigObject.GetComponent<PlayerConfiguration>().animator;

             playerAnim.Rebind();
             playerAnim.Update(0f);
         //   print("ReEnabling player");

            playerconfigObject.GetComponent<SpriteRenderer>().enabled = true;                  
            playerconfigObject.GetComponent<CapsuleCollider>().enabled = true;
            playerconfigObject.GetComponent<SphereCollider>().enabled = true;
            playerconfigObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            playerconfigObject.GetComponent<Rigidbody>().useGravity = true;
            playerconfigObject.GetComponent<Rigidbody>().isKinematic = false;
            player.GetComponent<Player>().shadow.SetActive(true);
            player.GetComponent<Player>().playerAura.SetActive(true);  //aura


            if (player.GetComponent<Player>().hasAI)
            {
                if (player.GetComponentInChildren<AI>().ballGrabbed)
                {
                    player.GetComponentInChildren<AI>().DropBall();
                }

                player.GetComponentInChildren<AI>().enabled = true;
                player.GetComponentInChildren<NavMeshAgent>().enabled = true;
                player.GetComponentInChildren<Rigidbody>().isKinematic = true;
                //     player.GetComponentInChildren<AI>().animator.runtimeAnimatorController = player.GetComponentInChildren<AI>().play;  
                player.GetComponentInChildren<AI>().FaceOpp();
            }
            else
            {
                if (player.GetComponentInChildren<Controller3D>().ballGrabbed == true)
                {
                    player.GetComponentInChildren<Controller3D>().DropBall();
                    player.GetComponentInChildren<Controller3D>().NormalAccelerationRate();
                }

                player.GetComponentInChildren<Rigidbody>().isKinematic = false;
                player.GetComponentInChildren<Controller3D>().enabled = true;

                // if mode == virtual joystick
             //   player.GetComponentInChildren<Controller3D>().SetTapThrowReadyToFalse();
             //   player.GetComponentInChildren<Controller3D>().ResetTouch1PhasePrev();
            //    player.GetComponentInChildren<Controller3D>().SetTouch0FXActivate(false);

                //   player.GetComponentInChildren<Controller3D>().animator.runtimeAnimatorController = player.GetComponentInChildren<Controller3D>().play;
                player.GetComponentInChildren<Controller3D>().FaceOpp();
            }

        }

        SetTeamSpawnLocations(1, stage.GetSpawnLocations(1, tm1.GetPlayerCount()));
        SetTeamSpawnLocations(2, stage.GetSpawnLocations(2, tm2.GetPlayerCount()));

        if (gameMode == "arcade" /* && PlayersAdded */)
        {
            SetPlayerUI(2, tm2.GetPlayerCount());
        }



        List<Vector3> ballSpwanLocations = stage.GetBallSpawnLocations(gameRule.ballCount);
        int i = 0;

        foreach (GameObject ball in balls)
        {
            ball.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            ball.GetComponent<SphereCollider>().enabled = true;
            ball.GetComponent<Ball>().DeactivateThrow();
            ball.transform.GetChild(2).gameObject.SetActive(false);
            ball.transform.position = ballSpwanLocations[i];

            float xBallDropForce = 1000 * UnityEngine.Random.Range(-1.0f, 1.0f);
            float zBallDropForce = 1000 * UnityEngine.Random.Range(-1.0f, 1.0f);


            ball.GetComponent<Rigidbody>().AddForce(new Vector3(round * xBallDropForce, 0f, round * zBallDropForce));

            i++;

        }


        if (round == 1)
        {
            // InstantiateSpeedBall();
        }

        PlayWhistle();
    }



    internal void EndGame()
    {
        start = false;
        round = 0;
        ready = false;
        countDown = countDownNum;
        timer = 0;
        celebrationTime = 5.0f;
        isCelebrating = false;
        mainCamController.Normal();
        perspCamController.Normal();
        hits.Clear();
        throws.Clear();
        team1Scored = false;
        team2Scored = false;

        gameOver = false;
        isPlaying = false;
        team1Wins = false;
        team2Wins = false;
        team1Points = 0;
        team2Points = 0;
        team1Scored = false;
        team2Scored = false;
        roundLevel = 0;
        isAtScene = false;

        Controller3D.hasGrabMag = false;
        Controller3D.grabMag = 10f;
        Controller3D.hasThrowMag = false;
        Controller3D.hasSeekVec = false;
        Controller3D.throwMagnetism = 5.65f;
        Controller3D.maxSeekVec = 100f;

        tm1.Clear();
        tm2.Clear();
        GlobalConfiguration.Instance.ClearPlayers();
        GlobalConfiguration.Instance.SetDefaultJoin(true);
    }

    internal void EndGameArcade()
    {
        print("EndGame Arcade");
        start = false;
        round = 0;
        ready = false;
        countDown = countDownNum;
        timer = 0;
        celebrationTime = 5.0f;
        isCelebrating = false;
        mainCamController.Normal();
        perspCamController.Normal();
        hits.Clear();
        throws.Clear();
        team1Scored = false;
        team2Scored = false;

        gameOver = false;
        isPlaying = false;
        // team1Wins = false; 
        // team2Wins = false;
        team1Points = 0;
        team2Points = 0;
        team1Scored = false;
        team2Scored = false;
        roundLevel = 0;
        isAtScene = false;

        foreach (GameObject player in tm1.players)
        {
            Controller3D pControl = player.GetComponent<Player>().controller3DObject.GetComponent<Controller3D>();
            if (pControl.ballGrabbed)
            {
                pControl.DropBall();
            }

        }

        tm2.Clear();

        GlobalConfiguration.Instance.ClearPlayers(2);

       // GlobalConfiguration.instance.SetDefaultJoin(true);
    }

    private bool GameOver()
    {
      //  if (TimeIsOut())
        {
          //  return true;
        }

        int i = 0;
        int j = 0;



        foreach (GameObject player in tm1.players)
        {
            if (player.GetComponent<Player>().isOut)
            {
                i++;
            }
        }

        foreach (GameObject player in tm2.players)
        {
            if (player.GetComponent<Player>().isOut)
            {
                j++;
            }
        }

        if (i == tm1.players.Count)
        {

            if (!team2Scored)
            {
                team2Points++;
                team2Scored = true;
               // float rand = UnityEngine.Random.Range(-10.0f, 10.0f);
                WinDisplay(new Vector3(100, 0, 0));
                PlayCheer();
                PlayWhistle();
                print("~!!! Team 2 WiNS !!!~");
            }
            if (team2Points > gameRule.roundsToWin)
            {
                team2Wins = true;
            }
            gameOver = true;
            return true;
        }
        if (j == tm2.players.Count)
        {

            if (!team1Scored)
            {
                team1Points++;
                team1Scored = true;
                //float rand = UnityEngine.Random.Range(-10.0f, 10.0f);
                WinDisplay(new Vector3(-100, 0,0));
                audioManager.PlayCheer();
                audioManager.PlayWhistle();
                print("~!!! Team 1 WiNS !!!~");
            }
            if (team1Points > gameRule.roundsToWin)
            {
                gameOver = true;
                team1Wins = true;
            }
            return true;
        }
        return false;
    }

    public void GameReset()
    {
        gameOver = false;
        ready = true;
        isPlaying = false;
        team1Wins = false;
        team2Wins = false;
        team1Points = 0;
        team2Points = 0;
        team1Scored = false;
        team2Scored = false;
        roundLevel = 0;
        GameRestart("reset");

     //   GameRestart();
    }

    void SetIsPlayingFalse()
    {
        isPlaying = false;
    }

    internal void EndGameMenu()
    {


        // adManager.LoadsSuperAwesomeVideo();
        //adManager.PlaySuperAwesomeVideo();

       cgg.GetComponent<EndGameMenuScript>().Pause();
        print("gameOver = " + gameOver);

    }

    private void CheckTeamHasPlayer()           //  do for key
    {                                                
        foreach (GameObject player in tm1.players)
        {
            if (player.GetComponent<Player>().isOut && player.GetComponent<Player>().hasJoystick)
            {
               
                foreach (GameObject other in tm1.players)
                {
                    if (other.GetComponent<Player>().hasAI && other.GetComponent<Player>().isOut == false)
                    {
                        if (GlobalConfiguration.Instance.GetMyJoysticks().Count >= 1)
                        {
                            other.GetComponent<Player>().ControlSwap(player);
                        }

                        player.GetComponent<Player>().enableAI();
                        player.GetComponent<Player>().DisablePlayer();

                        break;
                    }
                }
            }
        }
        foreach (GameObject player in tm2.players)
        {
            if (player.GetComponent<Player>().isOut && player.GetComponent<Player>().hasJoystick)
            {
                int joystickNumber = player.GetComponent<Player>().joystick.number;
                foreach (GameObject other in tm2.players)
                {
                    if (other.GetComponent<Player>().hasAI && other.GetComponent<Player>().isOut == false)
                    {

                        if (GlobalConfiguration.Instance.GetMyJoysticks().Count >= 1)
                        {
                            other.GetComponent<Player>().ControlSwap(player);
                        }

                        player.GetComponent<Player>().enableAI();
                        player.GetComponent<Player>().DisablePlayer();

                        break;
                    }
                }
            }
        }
    }

    public string GetArcadeSceneName()
    {
        return arcadeScript.GetScene();
    }
    internal int GetArcadeSceneIndex()
    {
        return arcadeScript.GetSceneIndex();
    }


    public void IncreaseDifficultyScalar(float x)
    {
        difficultyScaler += x;
    }

    public void AddOppsFaced(string x)
    {
        arcadeScript.AddOppCharacter(x);
    }


}
