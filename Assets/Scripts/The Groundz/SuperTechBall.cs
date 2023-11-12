using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperTechBall : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite image;
    public float seekMagnetism = 100f;
    public GameObject ps;

    void Start()
    {
        transform.parent.localScale = new Vector3(2, 2, 2);
    }

    // Update is called once per frame
    void Update()
    {
      //  transform.parent.eulerAngles = new Vector3(0f, 0f, 0f);
      //  transform.eulerAngles = Vector3.zero;
    }
}
