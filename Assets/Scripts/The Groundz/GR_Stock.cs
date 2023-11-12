using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GR_Stock : GameRule
{
    LevelManager levelManager;

    string gameMode;

    int team1Count;
    int team2Count;

    int roundsToWin =5;

    bool hasTimer = false;


    bool hasVFX = true;
    bool hasAIIntensity =  true;

    int playerHealthStockCount = 3;

    bool hasHalfCourtLine = true;
    bool hasCatchToGetIn = true;

    bool hasSlowDownAssist = true;
    //sliders
    bool hasGrabMag = true ;
    //sliders
    bool hasThrowAssist = true;
    //sliders

    bool hasRandomBall;

    bool hasStaminia= true;

    public float throwStaminaCost;
    public float moveStaminaCost;
    public float catchStaminaCost;
    public float pickUpStaminaCost;
    public float staminaDodgeCost = 10.0f;
    public float staminaReadyCost = 25.0f;


    float throwMag = 2;
    float maxseekVec = 25f;
    float difficultyScaler = 3f;
    float throwDecScalar = 0.25f;
    float grabMag = 10f;


    public Dictionary<GameObject, GameObject> throws = new Dictionary<GameObject, GameObject>();
    public Dictionary<GameObject, HashSet<GameObject>> hits = new Dictionary<GameObject, HashSet<GameObject>>();
    public Dictionary<GameObject, GameObject> catches = new Dictionary<GameObject, GameObject>();
    public List<Dictionary<int[], string>> outLog = new List<Dictionary<int[], string>>();

    bool team1Scored;
    bool team2Scored;


    public GR_Stock(string gameMode, LevelManager lm)
    {
        Reset();

        levelManager = lm;

        this.gameMode = gameMode;

        if (gameMode == "local")
        {

        }

    }

    void Reset()
    {
        ballCount = 5;
        roundsToWin = 5;

        hasTimer = false;


        hasVFX = true;
        hasAIIntensity = true;

        playerHealthStockCount = 3;

        hasHalfCourtLine = true;
        hasCatchToGetIn = true;

        hasSlowDownAssist = true;
        //sliders
        hasGrabMag = true;
        //sliders
        hasThrowAssist = true;
        //sliders

        hasRandomBall = false;

        hasStaminia = true;

        throwStaminaCost = 1.0f;
        moveStaminaCost = 1.0f;
        catchStaminaCost = 1.0f;
        pickUpStaminaCost = 1.0f;
        staminaDodgeCost = 10.0f;
        staminaReadyCost = 25.0f;


        throwMag = 2;
        maxseekVec = 25f;
        difficultyScaler = 3f;
        throwDecScalar = 0.25f;
        grabMag = 10f;

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
                        player.GetComponent<Player>().isOut = true;
                        //   OutDisplayX2(player.transform.GetChild(0).transform, ball);

                        string outType = "h";
                        Remove(player, outType, ball);
                        levelManager.PlayOuts();

                    }
                    if (hits.ContainsKey(ball))
                    {
                        toRemove.Add(ball);
                    }
                }
            }
            foreach (GameObject ball in toRemove)
            {
                hits.Remove(ball);
                RemoveThrow(ball);
            }
        }
    }

    public void RemoveThrow(GameObject ball)
    {
        if (throws.ContainsKey(ball))
        {

            throws.Remove(ball);

        }
    }

    public void RemoveHit(GameObject ball)
    {
        if (hits.ContainsKey(ball))
        {
            hits.Remove(ball);

        }
    }

    private void RemoveCatch(GameObject ball)
    {
        catches.Remove(ball);
    }

    /*
    private bool GameOver()
    {
        if (TimeIsOut())
        {
            return true;
        }

        int i = 0;
        int j = 0;



        foreach (GameObject player in levelManager.tm1.players)
        {
            if (player.GetComponent<Player>().isOut)
            {
                i++;
            }
        }

        foreach (GameObject player in levelManager.tm2.players)
        {
            if (player.GetComponent<Player>().isOut)
            {
                j++;
            }
        }

        if (i == levelManager.tm1.players.Count)
        {
            if (!team2Scored)
            {
                team2Points++;
                team2Scored = true;
                float rand = UnityEngine.Random.Range(-10.0f, 10.0f);
                WinDisplay(new Vector3(10 + rand / 5, 20, rand / 5));
                PlayCheer();
                PlayWhistle();
                print("~!!! Team 2 WiNS !!!~");
            }
            if (team2Points > 5)
            {
                team2Wins = true;
            }
            gameOver = true;
            return true;
        }
        if (j == Team2.Count)
        {

            if (!team1Scored)
            {
                team1Points++;
                team1Scored = true;
                float rand = UnityEngine.Random.Range(-10.0f, 10.0f);
                WinDisplay(new Vector3(-10 + rand / 5, 20, rand / 5));
                PlayCheer();
                PlayWhistle();
                print("~!!! Team 1 WiNS !!!~");
            }
            if (team1Points > 5)
            {
                gameOver = true;
                team1Wins = true;
            }
            return true;
        }
        return false;
    }

    */

    internal void ClearContacts(GameObject ball)
    {
        if (hits.ContainsKey(ball))
        {
            foreach (GameObject player in hits[ball])
            {
                if (player.GetComponent<Player>().hasAI)
                {
                    player.transform.GetChild(0).gameObject.GetComponent<AI>().playerConfigObject.GetComponent<PlayerConfiguration>().ballContact = false;
                    player.transform.GetChild(0).gameObject.GetComponent<AI>().ballHit = null;
                }
                else
                {
                    player.transform.GetChild(0).gameObject.GetComponent<Controller3D>().playerConfigObject.GetComponent<PlayerConfiguration>().ballContact = false;
                    player.transform.GetChild(0).gameObject.GetComponent<Controller3D>().ballHit = null;

                    ParticleSystem ps = player.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule ring_ps = ps.main;
                    ParticleSystem ps_2 = player.transform.GetChild(0).gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule spikes_ps = ps_2.main;
                    spikes_ps.simulationSpeed = 1;
                    ring_ps.simulationSpeed = 1;
                }
            }
        }
    }

    private bool TimeIsOut()
    {
        return false;
    }

    public void LastThrowerOut(GameObject ball)
    {
        throws[ball].GetComponent<Player>().isOut = true;
        string outType = "c";
        Remove(throws[ball], outType, ball);
        ball.GetComponent<Ball>().DeactivateThrow();
        levelManager.PlayCatches();
    }
    public void AddThrow(GameObject ball, GameObject player)
    {
        if (!throws.ContainsKey(ball))
        {
            throws.Add(ball, player);
        }
    }

    public void AddHit(GameObject ball, GameObject player)
    {
        // TODO Chain hits
        // i think a new world of logic presents itself when a ball hits multiplayers or multiple balls hit a player
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

    public void GetAnotherPlayer(int team)
    {
        bool somebodyOut = false;
        if (team == 1)
        {
            foreach (GameObject player in levelManager.tm1.players)
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
            foreach (GameObject player in levelManager.tm2.players)
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

    public void Remove(GameObject player, string outType, GameObject ball)
    {


        if (outType == "c")
        {
            int throwerNumber = player.GetComponent<Player>().number;
            int catcherNumber = catches[ball].GetComponent<Player>().number;
            int[] array = new int[2];
            array[0] = catcherNumber;
            array[1] = throwerNumber;
            Dictionary<int[], string> dict = new Dictionary<int[], string>();
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
            Dictionary<int[], string> dict = new Dictionary<int[], string>();
            dict.Add(array, outType);
            outLog.Add(dict);
            LogOuts();
        }

        if (player.GetComponentInChildren<Controller3D>().enabled)
        {
            if (player.GetComponentInChildren<Controller3D>().ballGrabbed)
            {
                player.GetComponentInChildren<Controller3D>().DropBall();
            }
           
            player.GetComponentInChildren<Controller3D>().isKnockedOut = false;
            player.GetComponentInChildren<Controller3D>().playerConfigObject.GetComponent<PlayerConfiguration>().ballContact = false;
            player.GetComponentInChildren<Controller3D>().enabled = false;
            ParticleSystem ps = player.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule ring_ps = ps.main;
            ParticleSystem ps_2 = player.transform.GetChild(0).gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule spikes_ps = ps_2.main;
            spikes_ps.simulationSpeed = 1;
            ring_ps.simulationSpeed = 1;
        }
        else
        {
            if (player.GetComponentInChildren<AI>().enabled)
            {
                if (player.GetComponentInChildren<AI>().ballGrabbed)
                {
                    player.GetComponentInChildren<AI>().DropBall();
                }
                player.GetComponentInChildren<AI>().isKnockedOut = false;
                player.GetComponentInChildren<AI>().playerConfigObject.GetComponent<PlayerConfiguration>().ballContact = false;
                player.GetComponentInChildren<AI>().enabled = false;
                player.GetComponentInChildren<UnityEngine.AI.NavMeshAgent>().enabled = false;

            }
        }
        player.GetComponent<Player>().PlayOutSound();
        player.GetComponent<Player>().DeRender();


    }


    private void LogOuts()
    {
        int outer = 0;
        int scorer = 0;
        string type = "";
        foreach (var group in outLog[outLog.Count - 1])
        {
            scorer = group.Key[0];
            outer = group.Key[1];
            type = (group.Value);
        }

        levelManager.GetLogFeed().GetComponent<GameLog>().AddToFeed(scorer, outer, type);
    }

    public void BringPlayerIn(int team)
    {
        int index = 0;

        List<GameObject> t1Players = levelManager.tm1.players;
        List<GameObject> t2Players = levelManager.tm2.players;

        if (team == 1)
        {
            foreach (GameObject player in t1Players)
            {
                if (player.GetComponent<Player>().isOut)
                {
                    index = t1Players.IndexOf(player);
                    break;
                }
            }
            if (t1Players[index].GetComponent<Player>().hasAI)
            {
                t1Players[index].GetComponentInChildren<AI>().enabled = true;
                t1Players[index].GetComponentInChildren<UnityEngine.AI.NavMeshAgent>().enabled = true;
                levelManager.tm1.players[index].GetComponentInChildren<Rigidbody>().isKinematic = true;
            }
            else
            {
                t1Players[index].GetComponentInChildren<Controller3D>().enabled = true;
                t1Players[index].GetComponentInChildren<Rigidbody>().isKinematic = false;
            }
            t1Players[index].transform.GetChild(1).gameObject.SetActive(true);
            t1Players[index].transform.GetChild(0).transform.position = t1Players[index].GetComponent<Player>().childPos0;
            t1Players[index].GetComponentInChildren<SpriteRenderer>().enabled = true;
            t1Players[index].GetComponentInChildren<CapsuleCollider>().enabled = true;
            t1Players[index].GetComponentInChildren<SphereCollider>().enabled = true;
            t1Players[index].GetComponentInChildren<Rigidbody>().useGravity = true;
            t1Players[index].GetComponent<Player>().isOut = false;
            Debug.Log(" bringing 1 in");
        }
        if (team == 2)
        {
            foreach (GameObject player in t2Players)
            {
                if (player.GetComponent<Player>().isOut)
                {
                    index = t2Players.IndexOf(player);
                    break;
                }
            }
            if (t2Players[index].GetComponent<Player>().hasAI)
            {
                t2Players[index].GetComponentInChildren<AI>().enabled = true;
                t2Players[index].GetComponentInChildren<UnityEngine.AI.NavMeshAgent>().enabled = true;
                t2Players[index].GetComponentInChildren<Rigidbody>().isKinematic = true;
            }
            else
            {
                t2Players[index].GetComponentInChildren<Controller3D>().enabled = true;
                t2Players[index].GetComponentInChildren<Rigidbody>().isKinematic = false;
            }
            t2Players[index].transform.GetChild(1).gameObject.SetActive(true);
            t2Players[index].transform.GetChild(0).transform.position = t2Players[index].GetComponent<Player>().childPos0;
            t2Players[index].GetComponentInChildren<SpriteRenderer>().enabled = true;
            t2Players[index].GetComponentInChildren<CapsuleCollider>().enabled = true;
            t2Players[index].GetComponentInChildren<SphereCollider>().enabled = true;
            t2Players[index].GetComponentInChildren<Rigidbody>().useGravity = true;
            t2Players[index].GetComponent<Player>().isOut = false;
            Debug.Log(" bringing 2 in");
        }
    }

    internal void PostFX(string type)
    {
        if (type == "Player1Hit")
        {

        }
    }


    private void CheckTeamHasPlayer()           // def  must revamp, especially when we dont consider 
    {                                                // most likely have to switch and update pi indexes
        foreach (GameObject player in levelManager.tm1.players)
        {
            if (player.GetComponent<Player>().isOut && player.GetComponent<Player>().hasJoystick)
            {
                int joystickNumber = player.GetComponent<Player>().joystick.number;
                foreach (GameObject other in levelManager.tm1.players)
                {
                    if (other.GetComponent<Player>().hasAI && other.GetComponent<Player>().isOut == false)
                    {
                        if (GlobalConfiguration.Instance.GetDeviceCount() >= 1)
                        {
                            other.GetComponent<Player>().enableController(joystickNumber, GlobalConfiguration.Instance.GetJoystickAt(joystickNumber - 1));
                        }
                        else
                        {
                            other.GetComponent<Player>().enableController(joystickNumber, "");
                        }

                        other.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                        player.GetComponent<Player>().hasAI = true;
                        player.GetComponent<Player>().hasJoystick = false;
                        break;
                    }
                }
            }
        }
        foreach (GameObject player in levelManager.tm2.players)
        {
            if (player.GetComponent<Player>().isOut && player.GetComponent<Player>().hasJoystick)
            {
                int joystickNumber = player.GetComponent<Player>().joystick.number;
                foreach (GameObject other in levelManager.tm2.players)
                {
                    if (other.GetComponent<Player>().hasAI && other.GetComponent<Player>().isOut == false)
                    {
                        other.GetComponent<Player>().enableController(joystickNumber, GlobalConfiguration.Instance.GetJoystickAt(joystickNumber - 1));
                        // Destroy(other.transform.GetChild(0).transform.GetChild(0).gameObject);
                        //   GameObject aura = Instantiate(player.transform.GetChild(0).transform.GetChild(0).gameObject, other.transform.GetChild(0));
                        // aura.SetActive(true);
                        other.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                        //   other.GetComponent<Player>().playerAura = aura;                                             // I think  we only need color
                        //  other.GetComponent<Player>().color = aura.GetComponent<ParticleSystem>().startColor;
                        player.GetComponent<Player>().hasAI = true;
                        player.GetComponent<Player>().hasJoystick = false;
                        break;
                    }
                }
            }
        }
    }

}
