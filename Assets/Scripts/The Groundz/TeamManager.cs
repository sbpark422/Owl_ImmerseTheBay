using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public int number;

    public List<GameObject> players = new List<GameObject>();
    List<GameObject> users = new List<GameObject>();
    List<GameObject> ais = new List<GameObject>();



     int initPlayerCount = 1;  // set in teamSelect

   public int playerCount;

    int aiCount;

    int userCount;

    int outCount;

    int mackCount;
    int kingCount;
    int ninaCount;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }


    public void SetNumber(int x)
    {
        number = x;
    }


    internal void AddObject(GameObject pObject, bool isUser)
    {
        pObject.transform.parent = this.transform;

        if (!players.Contains(pObject))
        {
            players.Add(pObject);
           
        }


        if (isUser)
        {

            if (!users.Contains(pObject))
            {
                userCount++;
                users.Add(pObject);
            }

        }
        else
        {
            if (!ais.Contains(pObject))
            {
                aiCount++;
                ais.Add(pObject);
            }
        }

        playerCount = players.Count;

        // print("team = " + number);
        // print("isUser = " + isUser);
        // print("userCount = " + userCount);
        // print("aiCount = " + aiCount);
    }

    internal void SetInitPlayerCount(int count)
    // parsed at TeamSelect scene only
    {
        initPlayerCount = count;
    }



    internal List<GameObject> PopulateAI(int team, string charName)
    {
        List<GameObject> returnMe = new List<GameObject>();

        int x = 1;            //  init for Arcade

        for (int i = 0; i < x; i++)
        {
            GameObject pObject = GlobalConfiguration.Instance.InstantiateAIPrefab();
            Player pScript = pObject.GetComponent<Player>();
            pScript.team = team;
            pScript.hasAI = true;

            // pScript.enableAI();    // cant unlessAtScene

            pScript.SetOnStandby(true);
           // GlobalConfiguration.Instance.SetPlayerType(pObject, charName, GetCharCount(charName));

            AddObject(pObject, false);      // gets unparented then reparented on global config level..

            returnMe.Add(pObject);
        }

        return returnMe;
    }


    internal List<GameObject> PopulateAI(int team)
    {
        List<GameObject> returnMe = new List<GameObject>();

        int x = initPlayerCount - (userCount + aiCount);            // what if we want to replace

        for (int i = 0; i < x; i++)
        {
            GameObject pObject = GlobalConfiguration.Instance.InstantiateAIPrefab();
            Player pScript = pObject.GetComponent<Player>();
            pScript.team = team;
            pScript.hasAI = true;

            // pScript.enableAI();    // cant unlessAtScene

            pScript.SetOnStandby(true);

          //  GlobalConfiguration.Instance.SetPlayerType(pObject, GetOppPlayerType(), GetCharCount(GetOppPlayerType()));
            AddObject(pObject, false);      // gets unparented then reparented on global config level..

            returnMe.Add(pObject);
        }

        return returnMe;
    }
    internal List<GameObject> PopulateAIRevamp(int team,int count)
    {
        List<GameObject> returnMe = new List<GameObject>();
        int x = count;
        int maxTeamCount = GlobalConfiguration.Instance.maxTeamCount;

        if (count + userCount > maxTeamCount)
        {
            x = Mathf.Clamp(maxTeamCount - userCount, 0, maxTeamCount);
        }
          // what if we want to replace

        for (int i = 0; i < x; i++)
        {
            GameObject pObject = GlobalConfiguration.Instance.InstantiateAIPrefab();
            Player pScript = pObject.GetComponent<Player>();
            pScript.team = team;
            pScript.hasAI = true;

            // pScript.enableAI();    // cant unlessAtScene

            pScript.SetOnStandby(true);

           // GlobalConfiguration.Instance.SetPlayerType(pObject, GetOppPlayerType(), GetCharCount(GetOppPlayerType()));
            AddObject(pObject, false);      // gets unparented then reparented on global config level..

            returnMe.Add(pObject);
        }

        return returnMe;
    }

    public List<GameObject> CreateAI(int count)
    {
        List<GameObject> returnMe = new List<GameObject>();

        for (int i = 0; i< count; i++)
        {
            GameObject pObject = GlobalConfiguration.Instance.InstantiateAIPrefab();
            Player pScript = pObject.GetComponent<Player>();
            pScript.team = number;
            pScript.hasAI = true;

           //  pScript.enableAI();   

            pScript.SetOnStandby(true);

          //  GlobalConfiguration.Instance.SetPlayerType(pObject, GetOppPlayerType(), GetCharCount(GetOppPlayerType()));
            AddObject(pObject, false);      // gets unparented then reparented on global config level..

            returnMe.Add(pObject);
        }

        return returnMe;
    }

    public List<GameObject> CreateAI(int count, string charName)
    {
        List<GameObject> returnMe = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            GameObject pObject = GlobalConfiguration.Instance.InstantiateAIPrefab();
            Player pScript = pObject.GetComponent<Player>();
            pScript.team = number;
            pScript.hasAI = true;

            //  pScript.enableAI();   

            pScript.SetOnStandby(true);

           // GlobalConfiguration.Instance.SetPlayerType(pObject, charName, GetCharCount(charName));

            AddObject(pObject, false);      // gets unparented then reparented on global config level..
          
            returnMe.Add(pObject);
        }

        return returnMe;
    }


    internal void SetInitAICount(int count)
    {
        aiCount = count;
    }

    internal void StandByPlayers(bool v)
    {
        foreach (GameObject player in players)
        {
            Player pScript = player.GetComponent<Player>();
            pScript.SetOnStandby(v);
        }
    }



    internal void MoveToSpawnPoints(List<Vector3> spawnPoints)
    {
        //print(" team = " + number);
      //  print("playerCount = " + playerCount);
       // print("spawnPoints.Count " + spawnPoints.Count);
        for (int i = 0; i < playerCount; i++)
        {
            players[i].transform.localPosition =Vector3.zero;
            players[i].transform.GetChild(0).position = spawnPoints[i];
            //NavMeshAgent.Warp(Vector3).
        }
    }

  

    internal void FaceOpp(int team)
    {
        if (team == 1)
        {
            foreach (GameObject pObject in players)
            {
                SpriteRenderer pSR = pObject.GetComponent<Player>().playerConfigObject.GetComponent<SpriteRenderer>();     // filthy lol
                pSR.flipX = false;
            }
        }

        if (team == 2)
        {
            foreach (GameObject pObject in players)
            {
                SpriteRenderer pSR = pObject.GetComponent<Player>().playerConfigObject.GetComponent<SpriteRenderer>();     // filthy lol
                pSR.flipX = true;

            }
        }
    }



    internal void SetRetreatPoints(List<Vector3> spawnPoints)
    {
        for (int i = 0; i < playerCount; i++)
        {
            if (players[i].GetComponent<Player>().hasAI)
            {

                // players[i].transform.GetChild(2).GetComponent<AI>().retreatPoint.position = 
            }

        }
    }

    internal void Clear()
    {
        userCount = 0;
        aiCount = 0;
        initPlayerCount = 1;
        playerCount = 1;
        players.Clear();
    }

    public int GetAICount()
    {
        print("team "+ number + " aiCount = " + aiCount);
        return aiCount;
    }

    public int GetUserCount()
    {
        print("team " + number + " userCount = " + userCount);
        return userCount;
    }

    internal int GetPlayerCount()
    {
        return playerCount;
    }

    internal void ClearAdded()
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (GameObject player in players)
        {
            if (player.GetComponent<Player>().aiObject.GetComponent<AI>().addedAtStage)
            {
                toRemove.Add(player);
            }
        }

        for (int i=0; i< toRemove.Count; i++)
        {
            ais.Remove(toRemove[i]);
            aiCount--;
            playerCount--;
            players.Remove(toRemove[i]);
        }
    }

}
