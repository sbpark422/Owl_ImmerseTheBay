using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBall : MonoBehaviour
{
    public SphereCollider sphereCollider;
    public SpriteRenderer spriteRenderer;
    public MeshRenderer meshRenderer;
    public AudioSource AudioSource;
    public AudioClip he10;
    public AudioClip he1100;
    public ParticleSystem ps;
    public Vector3 vel0;
    public float dur = 1f;
    public float mult = 1.5f;
    public Controller3D playerCont;

    [System.NonSerialized] public bool hit;

    void Start()
    {
        GetComponent<Ball>().type = "Speed";
    }

    // Update is called once per frame
    void Update()
    {
        if (hit)
        {
            Vector3 nuPos = new Vector3(playerCont.transform.position.x, playerCont.transform.position.y - 2, playerCont.transform.position.z);
            transform.position = nuPos;

                transform.eulerAngles = new Vector3(transform.eulerAngles.x + 1f, 0, 0);
        }
     
    }

    public void Hit(Controller3D controller3D)
    {
        hit = true;
        playerCont = controller3D;
        AudioSource.clip = he1100;
        AudioSource.Play();

        sphereCollider.enabled = false;
        spriteRenderer.enabled = false;
        meshRenderer.enabled = false;
        ParticleSystem.MainModule psMain = ps.main;
        psMain.simulationSpeed = .1f;
        ParticleSystem.VelocityOverLifetimeModule psVel = ps.velocityOverLifetime;
        psVel.yMultiplier = 10f;

    }

}
