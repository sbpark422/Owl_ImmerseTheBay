using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperBall : MonoBehaviour {

    // Use this for initialization
    // NUHandSize
    // Ball Size
    // Ball Thrown
    // Button Pressed
   
    public AudioClip initAudioClip;
    public AudioClip hitAudioClip;
    public AudioClip releaseAudioClip;
    public PhysicMaterial superPhysicMaterial;
    public float mass;
    public int type;
    public float superTime;
    public float superMagnetism = 0f;
    public float damage = 60f;
    public float throwPowerMult = 2f;
    public GameObject smokeFX;

    void Start () {
        if (type == 2)
        {
            superMagnetism = 100f;
        }
	}
	
	// Update is called once per frame
	void Update () {
           
	}

    public void InstantiateSmokeFX( /* speed */)
    {
        float ground = GlobalConfiguration.Instance.gameManager.levelManager.stage.floor;
        
        Vector3 position = new Vector3(this.transform.parent.position.x, ground, this.transform.parent.position.z);
        if (smokeFX)
        {
            Instantiate(smokeFX, position, Quaternion.identity);
        }
    }
}
