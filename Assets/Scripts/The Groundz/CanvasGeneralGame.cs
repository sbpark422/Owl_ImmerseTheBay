using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGeneralGame : MonoBehaviour
{
    List<GameObject> t1UIs = new List<GameObject>();
    List<GameObject> t2UIs = new List<GameObject>();

    public GameObject t1Score;
    public GameObject t2Score;

    public GameObject logFeed;

    public GameObject timer;

    public GameObject t1p1Icon;
     GameObject t1p1Power;
     GameObject t1p1Stamina;

    public GameObject t1p2Icon;
     GameObject t1p2Power;
     GameObject t1p2Stamina;

    public GameObject t1p3Icon;
     GameObject t1p3Power;
     GameObject t1p3Stamina;

    public GameObject t1p4Icon;
     GameObject t1p4Power;
     GameObject t1p4Stamina;

    public GameObject t2p1Icon;
     GameObject t2p1Power;
     GameObject t2p1Stamina;

    public GameObject t2p2Icon;
     GameObject t2p2Power;
     GameObject t2p2Stamina;

    public GameObject t2p3Icon;
     GameObject t2p3Power;
     GameObject t2p3Stamina;

    public GameObject t2p4Icon;
     GameObject t24Power;
     GameObject t2p4Stamina;


    GameManager gm;
    LevelManager lm;


    private void Awake()
    {

         if (!gm)
        {
            gm = GlobalConfiguration.Instance.gameManager.GetComponent<GameManager>();
        }

        if (gm)
        {
            lm = gm.levelManager;
        }

        t1UIs.Add(t1p1Icon);
        t1UIs.Add(t1p2Icon);
        t1UIs.Add(t1p3Icon);
        t1UIs.Add(t1p4Icon);
        t2UIs.Add(t2p1Icon);
        t2UIs.Add(t2p2Icon);
        t2UIs.Add(t2p3Icon);
        t2UIs.Add(t2p4Icon);


        
    }


    private void OnEnable()
    {
        if (gm)
        {
            lm = gm.levelManager;
        }

        lm.SetUICanvas(this);
    }


    void Start()
    {
        // check Icons


}


    void Update()
    {
        
    }

    public List<GameObject> GetPlayerUIs(int team)
    {
        if (team == 1)
        {
            return t1UIs;
        }

        if (team == 2)
        {
            return t2UIs;
        }

        return null;
    }

    public GameObject GetLogFeedObj()
    {
        // check if null
        return logFeed;
    }
    public GameObject GetTimerObj()
    {

        // check if null
        return timer;
    }

    public GameObject GetTeamScoreObj(int team)
    {

        // check if null
        if (team == 1)
        {
            return t1Score;
        }

        if (team == 2)
        {
            return t2Score;
        }

        return null;
    }
}
