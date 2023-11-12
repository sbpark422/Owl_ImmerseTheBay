using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLog : MonoBehaviour {

    public GameObject Log;
    List<GameObject> logList = new List<GameObject>();
    private float y0;
    private int logSize =5;
	void Start () {
		if (!Log)
        {
            Log = GameObject.Find("Log");
        }
         y0 = transform.position.y + 5;
	}
	
	// Update is called once per frame
	void Update () {

       // Vector3 nuLogPos = new Vector3(Log.transform.position.x, Log.transform.position.y - 10 * logList.Count, Log.transform.position.z);
    }

    public void AddToFeed(int scorer, int outer, string type)
    {
        print("Player " + scorer + " " + type + " " + "Player " + outer);
        GameObject nuLog = GameObject.Instantiate(Log, Vector3.zero, Log.transform.rotation, gameObject.transform);
        nuLog.GetComponent<RectTransform>().position = Vector3.zero;
        nuLog.GetComponent<RectTransform>().localPosition = Vector3.zero;

        GameObject Scorer = nuLog.transform.GetChild(0).gameObject;
        GameObject Verb = nuLog.transform.GetChild(1).gameObject;
        GameObject Outer = nuLog.transform.GetChild(2).gameObject;

        string verb;
        if (type == "h")
        {
            verb = "hit";
        }
        else
        {
            verb = "caught";
        }

        if (scorer == 0)
        {
            Scorer.GetComponent<TextMesh>().text = "Player 0";
            Scorer.GetComponent<TextMesh>().color = new Color(0.57f, 0.72f, 0.77f, 1f);
            Verb.GetComponent<TextMesh>().text = verb;
            Verb.GetComponent<TextMesh>().color = new Color(0.57f, 0.72f, 0.77f, 1f);
        }

        if (scorer == 1)
        {
            Scorer.GetComponent<TextMesh>().text = "Player 1";
            Scorer.GetComponent<TextMesh>().color = new Color(0.57f, 0.72f, 0.77f, 1f);
            Verb.GetComponent<TextMesh>().text = verb;
            Verb.GetComponent<TextMesh>().color = new Color(0.57f, 0.72f, 0.77f, 1f);
        }
        if (scorer == 2)
        {
            Scorer.GetComponent<TextMesh>().text = "Player 2";
            Scorer.GetComponent<TextMesh>().color = new Color(0.88f, 0.50f, 0.14f, 1f);
            Verb.GetComponent<TextMesh>().text = verb;
            Verb.GetComponent<TextMesh>().color = new Color(0.88f, 0.50f, 0.14f, 1f);
        }
        if (scorer == 3)
        {
            Scorer.GetComponent<TextMesh>().text = "Player 3";
            Scorer.GetComponent<TextMesh>().color = new Color(0.52f, 0.65f, 0.10f, 1f);
           Verb.GetComponent<TextMesh>().text = verb;
            Verb.GetComponent<TextMesh>().color = new Color(0.52f, 0.65f, 0.10f, 1f);
        }
        if (scorer == 4)
        {
           Scorer.GetComponent<TextMesh>().text = "Player 4";
           Scorer.GetComponent<TextMesh>().color = new Color(0.75f, .23f, 0.81f, 1f);
            Verb.GetComponent<TextMesh>().text = verb;
            Verb.GetComponent<TextMesh>().color = new Color(0.75f, .23f, 0.81f, 1f);
        }
        if (outer == 1)
        {
            Outer.GetComponent<TextMesh>().text = "Player 1";
           Outer.GetComponent<TextMesh>().color = new Color(0.57f, 0.72f, 0.77f, 1f);
        }
        if (outer == 2)
        {
           Outer.GetComponent<TextMesh>().text = "Player 2";
           Outer.GetComponent<TextMesh>().color = new Color(0.88f, 0.50f, 0.14f, 1f);
        }
        if (outer == 3)
        {
            Outer.GetComponent<TextMesh>().text = "Player 3";
            Outer.GetComponent<TextMesh>().color = new Color(0.52f, 0.65f, 0.10f, 1f);
        }
        if (outer == 4)
        {
            Outer.GetComponent<TextMesh>().text = "Player 4";
            Outer.GetComponent<TextMesh>().color = new Color(0.75f, .23f, 0.81f, 1f);
        }

        logList.Add(nuLog);
        if (logList.Count > 1) {

                for (int i = 1; i <= logList.Count; i++)
                {
                    if (i < logSize)
                    {
                        logList[logList.Count - i].transform.position = new Vector3(transform.position.x, y0 - i, transform.position.z);
                        TextMesh textMesh = logList[logList.Count - i].transform.GetChild(0).gameObject.GetComponent<TextMesh>();
                        Color textColor = new Color(0f, 0f, 0f);
      
                    }
                    else
                    {
                        Destroy(logList[logList.Count - i]);
                    }
                }
        }
    }
}
