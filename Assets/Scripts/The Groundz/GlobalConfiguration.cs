using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class GlobalConfiguration : MonoBehaviour
{

    public GameManager gameManager;

    public AudioSource audioSourceGM;
    public AudioSource audioSourceMenu;


    public GameObject playerPrefab;

    public GameObject player1KeyPrefab;
    public GameObject aiPrefab;

    public GameObject ballPrefab;

    List<GameObject> players = new List<GameObject>();

    public Dictionary<String, bool> locks = new Dictionary<String, bool>();

    int team1Count;
    int team2Count;

    public int maxTeamCount = 4;  // gr
    int maxPlayerCount = 8;   // gr

    public GameObject team1Object;
    public GameObject team2Object;

    MainMenu mainMenu;  // <-- gotta find reference
    StageSelect stageSlect;  // <-- gotta find reference

    public enum LoadPoint {gameMode, aradeMode, local, stage }

    public static LoadPoint loadPoint;

    public enum GameMode { none, arcade, multiplayer, story };
    public GameMode gameMode = GameMode.none;

    List<MyJoystick> myJoysticks;
    public static string[] joysticks;

    public Color[] stickColorGuide;

    [SerializeField]
    int deviceCount;

    public static int gamepadStarts;

    bool isAtScene;  // dup of lm isAtScene
    //list scenes scenes 
    enum Stage { theGym, theGroundz, theBlock }

    Stage stage;

    public bool gameStarted;

    public static bool isAtQuickCharacterSelect;

    public static bool isAtRevampTeamSelect;
    bool gamePaused;

    bool touchEnabled;

    public MultiplayerEventSystem currentMultiplayerEventSystem;


    void Awake()
    {
        print("Global config Awake");

         DontDestroyOnLoad(this);

        myJoysticks = new List<MyJoystick>();

        stickColorGuide = new Color[maxPlayerCount];   // customizable 
        //team 1 player colors
        stickColorGuide[0] = new Color(.1f,.1f,1f);
        stickColorGuide[1] = new Color(.1f, .3f, 1f);
        stickColorGuide[2] = new Color(.1f, .6f, 1f);
        stickColorGuide[3] = new Color(.1f, .9f, 1f);
        //team 2 player colors
        stickColorGuide[4] = new Color(1f, .1f, .1f);
        stickColorGuide[5] = new Color(1f, .3f, .1f);
        stickColorGuide[6] = new Color(1f, .6f, .1f);
        stickColorGuide[7] = new Color(1f, .9f, .1f);

        //init
        locks.Add("Mack", false);
        locks.Add("King", false);
        locks.Add("Nina", false);

        if (!team1Object)
        {
            team1Object = gameObject.transform.GetChild(0).GetChild(0).gameObject;
        }

        if (!team2Object)
        {
            team1Object = gameObject.transform.GetChild(0).GetChild(0).gameObject;
        }

        TeamManager tm1 = team1Object.GetComponent<TeamManager>();
        TeamManager tm2 = team2Object.GetComponent<TeamManager>();

        tm1.SetNumber(1);
        tm2.SetNumber(2);
    }


    internal void LoadDefaultGame()
    {

    }

    internal void SetInputModule(int playerIndex, InputSystemUIInputModule inputSystemUIInputModule)
    {
     foreach (GameObject player in players)
        {
            PlayerInput pi = player.GetComponent<Player>().controller3DObject.GetComponent<PlayerInput>();
            if (pi.playerIndex == playerIndex)
            {
                pi.uiInputModule = inputSystemUIInputModule;
            }
        }
    }

    private void Start()
    {
       SetGameStarted(true);

    }

    public GameObject CreatePlayer1()       // keyboard
    {

        print("Creating Player 1");

        GameObject playerObject = InstantiatePlayer1KeyPrefab();                 // if you instantiate w pi enabled then you get handleJoin
        Player playerScript = playerObject.GetComponent<Player>();

        playerScript.CreateJoystick(-1);
        myJoysticks.Add(playerScript.GetJoystick());

        AddNewPlayer(playerObject);

        return playerObject;

    }

    internal GameObject InstantiatePlayerPrefab()
    {
        if (playerPrefab)
        {
            GameObject returnMe = Instantiate(playerPrefab);

            return returnMe;
        }

        else
        {
            print("No playerPrefab");
            return null;
        }
    }

    internal GameObject InstantiatePlayer1KeyPrefab()
    {
        if (player1KeyPrefab)
        {
            GameObject returnMe = Instantiate(player1KeyPrefab);

            return returnMe;
        }

        else
        {
            print("No playerPrefab");
            return null;
        }
    }

    internal GameObject InstantiateAIPrefab()
    {
        if (aiPrefab)
        {
            GameObject returnMe = Instantiate(aiPrefab);
            return returnMe;
        }
        else
        {
            print("No aiPrefab");
            return null;
        }


    }
    internal GameObject InstantiateBallPrefab(Vector3 pos)
    {
        if (ballPrefab)
        {
            GameObject returnMe = Instantiate(ballPrefab, pos, Quaternion.identity);

            return returnMe;
        }
        else
        {
            print("No ballPrefab");
            return null;
        }

    }



    public void HandlePlayerJoin(PlayerInput pi)                 // pi on instnatiaded player prefab
    {
        /*
        GetJoysticks();

        deviceCount++;                    // should allign w joySTick count

        //if not isPlaying

        print("Player Joining");

        if (!players.Any(p => p.GetComponent<Player>().GetPlayerIndex() == pi.playerIndex))
        {
            print("New Device @ index: " + pi.playerIndex);

            gamepadStarts++;

            GameObject playerObject = pi.transform.root.gameObject;
            Player playerScript = playerObject.GetComponent<Player>();

            playerScript.CreateJoystick(pi.playerIndex, GetJoystickAt(pi.playerIndex));  // should be obsolete
            myJoysticks.Add(playerScript.GetJoystick());

            AddNewPlayer(playerObject);

        }
        */
        
    }

    public void AddNewPlayer(GameObject playerObject)
    {
        players.Add(playerObject);                    

        Player playerScript = playerObject.GetComponent<Player>();
        playerScript.number = players.Count;      // can be interesting when considering mid game joining

        playerObject.name = "Player " + playerScript.number;


        playerObject.transform.parent = this.gameObject.transform;                 // needed to parent for DontDestroyOnLoadscene sake
    }

    public void HandlePlayerLeave(PlayerInput pi)
    {
        deviceCount--;

        // would also have to tap in w player (hasJoystick and index) and joystick setting

        //   myJoysticks.RemoveAt(pi.playerIndex - 1);   // actually depends if playerIndex's update on leave


        if (isAtQuickCharacterSelect)
        {
           // quickCharacterSelect.SetReadyCount(deviceCount);
            //remove ui stick
        }

      //  RevampTeamSelect.starts--;

        //
    }


    public void SetGameMode(string x)
    {
        switch (x)
        {
            case "arcade":                                         
                gameMode = GameMode.arcade;
                LevelManager lm = gameManager.levelManager;
                lm.SetGameMode("arcade");         
               

                break;
            case "multiplayer":
                gameMode = GameMode.multiplayer;
                lm = gameManager.levelManager;
                lm.SetGameMode("multiplayer");
                break;

            case "story":
                gameMode = GameMode.story;
                break;
        }
    }


    internal void SetTeamCount(int v, int count)
    {
        if (v == 1)
        {
            team1Count = count;
            team1Object.GetComponent<TeamManager>().SetInitPlayerCount(count);
        }

        if (v == 2)
        {
            team2Count = count;
            team2Object.GetComponent<TeamManager>().SetInitPlayerCount(count);
        }
    }

    internal void SetTeamInitAICount(int v, int count)
    {
        if (v == 1)
        {
            
            team1Object.GetComponent<TeamManager>().SetInitAICount(count);
        }

        if (v == 2)
        { 
            team2Object.GetComponent<TeamManager>().SetInitAICount(count);
        }
    }



    internal void SetIsAtQuickCharacterSelect(bool v)
    {
        isAtQuickCharacterSelect = v;
    }

    internal void SetIsAtRevampTeamSelect(bool v)
    {
        isAtRevampTeamSelect = v;
    }
    public void SetStage(string x)
    {

        GetJoysticks();                  // checking per scene

        switch (x)
        {
            case "theGym":
                stage = Stage.theGym;
                break;
            case "theGroundz":
                stage = Stage.theGroundz;
                break;
            case "theBlock":
                stage = Stage.theBlock;
                break;
        }
    }

    public int GetDeviceCount()
    {
        return deviceCount;
    }
    /*
    internal void AddSelectedPlayer(string name, int team, int playerIndex)         // obsolete
    {
      foreach (GameObject player in players)
        {
            if (player.GetComponent<Player>().GetPlayerIndex() == playerIndex)
            {
                Player playerScript = player.GetComponent<Player>();
                playerScript.team = team;
                playerScript.type = name;

                SetPlayerType(player, name);


            }
        }
    }
    */


    internal void SetDefaultJoin(bool v)
    {
        PlayerInputManager pim = gameManager.playerInputManager;

        if (v)
        {
            pim.EnableJoining();
            pim.enabled = true;
        }
        else
        {
            pim.DisableJoining();
            pim.enabled = false;
        }
    }

    public void GetJoysticks()
    {

        joysticks = Input.GetJoystickNames();
        joysticks = GetActualJoysticks(joysticks);


    }

    public string GetJoystickAt(int v)
    {
        if (joysticks.Length > v)
        {
            return joysticks[v];
        }

        return "";

    }
    internal Color GetMyJoystickAt(int v)
    {
        throw new NotImplementedException();
    }

    public List<MyJoystick> GetMyJoysticks()
    {
        return myJoysticks;
    }

    internal void PopulateAIRevamp(int p1team, int ai1Count, int ai2Count)
    {
        if (p1team == 1)
        {
            PopulateAITeamRevamp(2, ai2Count);   //orders important to get opposite char type
            PopulateAITeamRevamp(1, ai1Count);
        }

        if (p1team == 2)
        {
            PopulateAITeamRevamp(1, ai1Count);
            PopulateAITeamRevamp(2, ai2Count);

        }
    }

    internal void PopulateAI(int p1team)
    {
        if (p1team == 1)
        {
            PopulateAITeam(2);   //orders important to get opposite char type
            PopulateAITeam(1);
        }

        if (p1team == 2)
        {
            PopulateAITeam(1);  
            PopulateAITeam(2);

        }

    }

    internal void PopulateArcadeAI(int team, string charName)
    {
       
        if (team == 2)
        {
            PopulateAITeam(2,charName);

        }

        if (team == 1)
        {
            PopulateAITeam(1,charName);

        }

    }

    void PopulateAITeam(int team, string charName)
    {
        if (team == 1)
        {
            TeamManager tm1 = team1Object.GetComponent<TeamManager>();
            List<GameObject> ai1_new = tm1.PopulateAI(1,charName);
            int i = 0;
            foreach (GameObject ai1_ in ai1_new)
            {
                i++;
                Player pScript = ai1_.GetComponent<Player>();
                AddNewPlayer(ai1_);
                AddPlayerToTeamManager(ai1_, 1, false);
                pScript.SetColor(GetPlayerColor(i, pScript));
            }
        }

        if (team == 2)
        {
            TeamManager tm2 = team2Object.GetComponent<TeamManager>();
            List<GameObject> ai2_new = tm2.PopulateAI(2, charName);
            int j = 0;
            foreach (GameObject ai2_ in ai2_new)
            {
                j++;
                Player pScript = ai2_.GetComponent<Player>();
                AddNewPlayer(ai2_);
                AddPlayerToTeamManager(ai2_, 2, false);
                pScript.SetColor(GetPlayerColor(j, pScript));
            }
        }
    }
    void PopulateAITeam(int team)
    {
        if (team == 1)
        {
            TeamManager tm1 = team1Object.GetComponent<TeamManager>();
            List<GameObject> ai1_new = tm1.PopulateAI(1);
            int i = 0;
            foreach (GameObject ai1_ in ai1_new)
            {
                i++;
                Player pScript = ai1_.GetComponent<Player>();
                AddNewPlayer(ai1_);
                AddPlayerToTeamManager(ai1_, 1, false);
                pScript.SetColor(GetPlayerColor(i,pScript));
            }
        }

        if (team == 2)
        {
            TeamManager tm2 = team2Object.GetComponent<TeamManager>();
            List<GameObject> ai2_new = tm2.PopulateAI(2);
            int j = 0;
            foreach (GameObject ai2_ in ai2_new)
            {
                j++;
                Player pScript = ai2_.GetComponent<Player>();
                AddNewPlayer(ai2_);
                AddPlayerToTeamManager(ai2_, 2, false);
                pScript.SetColor(GetPlayerColor(j,pScript));
            }
        }
    }
    void PopulateAITeamRevamp(int team, int count)
    {

        if (team == 1)
        {
            TeamManager tm1 = team1Object.GetComponent<TeamManager>();

                List<GameObject> ai1_new = tm1.PopulateAIRevamp(1, count);
                int i = 0;
                foreach (GameObject ai1_ in ai1_new)
                {
                    i++;
                    Player pScript = ai1_.GetComponent<Player>();
                    AddNewPlayer(ai1_);
                    AddPlayerToTeamManager(ai1_, 1, false);
                    pScript.SetColor(GetPlayerColor(i, pScript));
                }
        }

        if (team == 2)
        {
            TeamManager tm2 = team2Object.GetComponent<TeamManager>();

                List<GameObject> ai2_new = tm2.PopulateAIRevamp(2, count);
            int j = 0;
                foreach (GameObject ai2_ in ai2_new)
                {
                    j++;
                    Player pScript = ai2_.GetComponent<Player>();
                    AddNewPlayer(ai2_);
                    AddPlayerToTeamManager(ai2_, 2, false);
                    pScript.SetColor(GetPlayerColor(j, pScript));
                }
            }
    }

    internal void AddPlayerToTeamManager(GameObject pObject, int team, bool isUser)
    {
        if (team == 1)
        {
            TeamManager tm1 = team1Object.GetComponent<TeamManager>();
            tm1.AddObject(pObject, isUser);
        }

        if (team == 2)
        {
            TeamManager tm2 = team2Object.GetComponent<TeamManager>();
            tm2.AddObject(pObject, isUser);
        }
    }

    public GameObject GetPlayerAtIndex(int index)
    {

        // check list size

        return players[index];
    }

    public List<GameObject> GetPlayers()
    {

        // check list
        return players;
    }

    public List<GameObject> GetPlayers(int team)
    {

      if (team == 1)
        {
            return team1Object.GetComponent<TeamManager>().players;
        }

        if (team == 2)
        {
            return team2Object.GetComponent<TeamManager>().players;
        }

        return players;
    }

    public GameObject GetPlayer(int team, int playerIndex)
    {

        if (team == 1)
        {
            return team1Object.GetComponent<TeamManager>().players[playerIndex - 1];
        }

        if (team == 2)
        {
            return team2Object.GetComponent<TeamManager>().players[playerIndex - 1];
        }

        return null;
    }

    internal void SetIsAtScene(bool v, int sceneIndex)
    {
        isAtScene = v;

        LevelManager lm = gameManager.levelManager;
        lm.SetIsAtScene(v);
        lm.SetSceneIndex(sceneIndex);
    }

    internal void SetGameStarted(bool v)
    {
        gameStarted = v;
    }


    private string[] GetActualJoysticks(string[] stix)
    {
        int j = 0;
        string[] returnMe;
        List<string> addMe = new List<string>();

        for (int i = 0; i < stix.Length; i++)
        {
            if (stix[i] != "")
            {
                j++;
                addMe.Add(stix[i]);
            }
        }
        returnMe = new string[j];

        for (int k = 0; k < addMe.Count; k++)
        {
            returnMe[k] = addMe[k];
        }

        return returnMe;
    }

    public void AdjustGmVolume(float newVolume)
    {
        AudioListener.volume = newVolume;
    }

    public void AdjustMenuVolume(float newVolume)
    {
        audioSourceMenu.volume = newVolume;
    }

    internal void ClearPlayers()
    {
        foreach (GameObject player in players)
        {
            Destroy(player);
        }

        players.Clear();
    }

    internal void ClearPlayers(int team)
    {
        int playerCount = players.Count;

        List<GameObject> toRemove = new List<GameObject>();

        for (int i = 0; i< playerCount;i++)
        {    
                if (players[i].GetComponent<Player>().team == team)
            {
                toRemove.Add(players[i]);

            }
        }

        for (int j =0; j< toRemove.Count; j++)
        {
            players.Remove(toRemove[j]);
            Destroy(toRemove[j]);
        }

    }

    internal void ClearPlayers(int team, bool clearAdded)
    {
        int playerCount = players.Count;

        List<GameObject> toRemove = new List<GameObject>();

        for (int i = 0; i < playerCount; i++)
        {
            if (players[i].GetComponent<Player>().team == team && (clearAdded && players[i].GetComponent<Player>().aiObject.GetComponent<AI>().addedAtStage))
            {
                toRemove.Add(players[i]);

            }
        }

        for (int j = 0; j < toRemove.Count; j++)
        {
            players.Remove(toRemove[j]);
            Destroy(toRemove[j]);
        }

    }

    public Color GetPlayerColor(int index, Player pScript)
    {
        int team = pScript.team;

        if (team == 1)
        {
            TeamManager tm1 = team1Object.GetComponent<TeamManager>();
            return stickColorGuide[index-1];
        }

        if (team == 2)
        {
            TeamManager tm2 = team2Object.GetComponent<TeamManager>();
            return stickColorGuide[3+index];
        }

        return Color.black;
    }

    public bool GetCharIsLocked(string name)
    {
        return locks[name];
    }

    internal void TurnThemeMusic(bool v)
    {

        LobbyMusicScript lms = gameManager.audioManager.GetComponent<LobbyMusicScript>();

        if (v)
        {
            lms.StartTheme();
        }
        else
        {
            lms.EndTheme();
        }

    }

    internal bool GetIsThemeOff()
    {
        return gameManager.audioManager.GetComponent<LobbyMusicScript>().GetIsThemeOff();
    }
    
    #region Singleton

    private static GlobalConfiguration instance;

    public static GlobalConfiguration Instance
    {
        get
        {
            if (instance != null) return instance;

            instance = FindObjectOfType<GlobalConfiguration>();
            if (instance != null) return instance;

            GameObject go = Instantiate(Resources.Load("PreFabs/GameManager/GameManager")) as GameObject;
            instance = go.GetComponent<GlobalConfiguration>();
            DontDestroyOnLoad(go);  // move to GameManager

            return instance;
        }
    }

    #endregion
}
