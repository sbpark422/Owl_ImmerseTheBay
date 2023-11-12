using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;


public class Player : MonoBehaviour
{

    LevelManager levelManager;

    // transform or spawn point

    public int number;

    int playerIndex;   // < should be alligned w myjoystick

    bool isSet;

    public GameObject playerConfigObject;  // 0
    public GameObject playerAura;   // 0.1
    SpriteRenderer healthSR;
    SpriteRenderer staminaSR;
    SpriteRenderer powerSR;
    GameObject hitObject;
    GameObject catchObject;
    GameObject winObject;
    public GameObject shadow;   // 0.2
    public GameObject controller3DObject;  // 1
    public GameObject aiObject;  // 2

    Controller3D controller3D;
    AI aiScript;

    public Sprite playerIconImage;
    public GameObject staminaBarObject;

    public GameObject powerBarObject;

    public GameObject super;

    public MyJoystick joystick;
    public bool hasJoystick;

    public bool hasAI;

    public string type;

    public Color color;    // should always be joystick colorS
    public int team;
    public Vector3 childPos0;
    public bool isOut = false;

    // attributes  read for show ... Set in SetPlayerType in Global cCnfig
    [System.NonSerialized] public float maxSpeed;
    [System.NonSerialized] public float xspeed;
    [System.NonSerialized] public float zspeed;
    [System.NonSerialized] public float dodgeSpeed;
    [System.NonSerialized] public float throwPower0;
    [System.NonSerialized] public float standingThrowPower;
    [System.NonSerialized] public float maxThrowPower;
    [System.NonSerialized] public float catchLagTime;    //sec 
    // acceleration

    public int stamina;
    private float staminaCool;
    public int power;
    private float powerCool;

    public Material out_mat;
    public Material default_mat;



    private bool dodgeActivated;

    private float fxSpeed = 1f;

    public List<RuntimeAnimatorController> runtimeControlllers = new List<RuntimeAnimatorController>();
 //   public RuntimeAnimatorController main; 0
  //  public RuntimeAnimatorController alt1;
  //  public RuntimeAnimatorController alt2;
 //   public RuntimeAnimatorController alt3;


    public AudioSource playerAudioSource;

    public AudioClip catchSound;
    public AudioClip[] throwSounds;
    public AudioClip dodgeSound;

    public AudioClip outSound;
    public AudioClip footsteps;

    private bool isDeRendering;
    private float dR_Cool;
    private float drC_t0;
    private float drC_tF;

    public static event Action<InputUser, InputUserChange, InputDevice> onChange;
    


    private void Awake()
    {
        GameObject gameManagerObject = GlobalConfiguration.Instance.gameManager.gameObject;

        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        // print("Player Awake");
        //CheckChildStructure();
        //isSet = false;
       // InputUser.listenForUnpairedDeviceActivity = 1;
      //  onChange += DeviceChange();


    }


    void CheckChildStructure()
    {
        if (!playerConfigObject)
        {
            playerConfigObject = transform.GetChild(0).gameObject;
        }

        if (!playerAura)
        {
            playerAura = playerConfigObject.transform.GetChild(0).gameObject;
        }

        if (!shadow)
        {
            shadow = playerConfigObject.transform.GetChild(1).gameObject;
        }

        if (!controller3DObject)
        {
            controller3DObject = transform.GetChild(1).gameObject;
        }

        if (!aiObject)
        {
            aiObject = transform.GetChild(2).gameObject;
        }

        if (!healthSR)
        {
            healthSR = playerAura.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        }

        if (!staminaSR)
        {
            staminaSR = playerAura.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
        }

        if (!powerSR)
        {
            powerSR = playerAura.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>();
        }

        if (!hitObject)
        {
            hitObject = playerAura.transform.GetChild(3).gameObject;
        }

        if (!catchObject)
        {
            catchObject = playerAura.transform.GetChild(4).gameObject;
        }

        if (!winObject)
        {
            winObject = playerAura.transform.GetChild(5).gameObject;
        }


        controller3D = controller3DObject.GetComponent<Controller3D>();
        aiScript = aiObject.GetComponent<AI>();
    }

    void Start()
    {

        // attributes set in GlobalConfig


        // grab from level Manager
        /*
                if (number == 1)       // this should be all in one ui object
                    {
                        staminaBarObject = GameObject.Find("1 Stamina Slider");
                        powerBarObject = GameObject.Find("1 Power Slider");
                    }
                    if (number == 2)
                    {
                        staminaBarObject = GameObject.Find("2 Stamina Slider");
                        powerBarObject = GameObject.Find("2 Power Slider");
                    }
                    if (number == 3)
                    {
                        staminaBarObject = GameObject.Find("3 Stamina Slider");
                        powerBarObject = GameObject.Find("3 Power Slider");
                    }
                    if (number == 4)
                    {
                        staminaBarObject = GameObject.Find("4 Stamina Slider");
                        powerBarObject = GameObject.Find("4 Power Slider");
                    }

                    staminaCool = (int)(stamina-controller3D.staminaCool);    // iffy 
                    staminaBarObject.GetComponent<Slider>().value = stamina;
                    powerCool = (int)(stamina- controller3D.superCoolDown);     // iffy
                    powerBarObject.GetComponent<Slider>().value = power;

                

        playerAudioSource = playerConfigObject.GetComponent<AudioSource>();

        */

    }

    // Update is called once per frame
    void Update()
    {
        /*
       if (levelManager.isPlaying)
       {

           if (hasJoystick)
           {
               staminaCool = (stamina - controller3D.staminaCool);
               staminaSR.material.SetFloat("_Rotation", staminaCool / 300f);
               staminaBarObject.GetComponent<Slider>().value = staminaCool;
               powerCool = (power - controller3D.superCoolDown);
               powerSR.material.SetFloat("_Rotation", powerCool / 60f);
               powerBarObject.GetComponent<Slider>().value = powerCool;
           }
           if (hasAI)
           {
               staminaCool = (int)(stamina - aiScript.staminaCool);
               staminaBarObject.GetComponent<Slider>().value = staminaCool;
               powerCool = (int)(power - aiScript.superCoolDown);
               powerBarObject.GetComponent<Slider>().value = powerCool;
           }

           if (dodgeActivated)
           {
               /*
               fxSpeed = 3f;
               spikeSize = 0.5f;
               stars.simulationSpeed = fxSpeed;
               spikes.simulationSpeed = fxSpeed;
               ring.simulationSpeed = fxSpeed * 4f;
               spikes.startSize = spikeSize;


           }
           else
           {
               /*
               if (fxSpeed > 1f)
               {
                   fxSpeed -= .25f;
                   spikeSize = .2f;
                   stars.simulationSpeed = fxSpeed;
                   spikes.simulationSpeed = fxSpeed;
                   ring.simulationSpeed = fxSpeed * 4f;
                   spikes.startSize = spikeSize;

               }

           }
       }

       if (isDeRendering)
       {
           drC_tF = Time.realtimeSinceStartup;
           float t = drC_tF - drC_t0;
           DeRender(t);
       }
       */

    }

    internal void DisablePlayerInput()
    {
        Controller3D controller3D = controller3DObject.GetComponent<Controller3D>();
        controller3D.DisablePlayerInputControls();
    }

    internal void SetisSet(bool v)
    {
        isSet = v;
    }

    private void DeRender(float v)              // DeRender + + +
    {
        SpriteRenderer sr = playerConfigObject.GetComponent<SpriteRenderer>();
        if ((dR_Cool - v) <= 0)
        {
            // print("done deRendering");
            isDeRendering = false;
            // playerConfigObject.GetComponent<PlayerConfiguration>().SetOutAnimation(false);
            // out_mat.SetFloat("_Start", 0);
            playerConfigObject.GetComponent<SpriteRenderer>().material = default_mat;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
            playerConfigObject.GetComponent<SpriteRenderer>().enabled = false;
            playerConfigObject.GetComponent<CapsuleCollider>().enabled = false;
            playerConfigObject.GetComponent<SphereCollider>().enabled = false;
            playerConfigObject.GetComponent<Rigidbody>().isKinematic = true;
            playerConfigObject.GetComponent<Rigidbody>().useGravity = false;

            playerConfigObject.transform.position = transform.position;   //?    +

            playerConfigObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);  // +

            playerAura.SetActive(false);
        }

        else
        {
            // playerConfigObject.transform.Translate(new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), -1 * v, 0f));
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (dR_Cool - v));

            //  float cutThresh = playerConfigObject.GetComponent<SpriteRenderer>().material.GetFloat("_CutThresh");   // ?
            //   playerConfigObject.GetComponent<SpriteRenderer>().material.SetFloat("_CutThresh", cutThresh + .05f);    //?

        }
    }

    public float GetThrowPower0()
    {
        return throwPower0;
    }
    public float GetMaxThrowPower()
    {
        return maxThrowPower;

    }

    public void enableAI()
    {


        hasAI = true;

        aiObject.SetActive(true);
        aiScript.enabled = true;
        aiScript.Init();

        hasJoystick = false;
        controller3DObject.SetActive(false);
        controller3D.enabled = false;
        PlayerInput playerInput = controller3DObject.GetComponent<PlayerInput>();
        playerInput.enabled = false;


        NavMeshAgent navMeshAgent = playerConfigObject.GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;

        playerAura.SetActive(false);

        playerConfigObject.GetComponent<Rigidbody>().isKinematic = true;

    }


        

    public void ControlSwap(GameObject pSwap)
    {
        /// Gotta check keyboard 
        
      

            Player pScript = pSwap.GetComponent<Player>();
            Color pColor = pScript.color;
            int pIndex = pScript.joystick.number;

        if (pIndex != -1)
            {
            print("pjoyNum " + pIndex);

            PlayerInput pInput = PlayerInput.all[pIndex];
            /*
            print("playerInput.playerIndex " + pInput.playerIndex);
            print(" InputUser.all " + InputUser.all.Count);
            print("PlayerInput.all size " + PlayerInput.all.Count);
            print("playerInput.user.id " + pInput.user.id);
            print("playerInput.user.index " + pInput.user.index);
            print("playerInput.user.pairedDevices" + pInput.user.pairedDevices);
            print(" InputSystem.devices.count " + pInput.devices.Count);
            */

            disableAI();

            hasJoystick = true;

            joystick = pScript.joystick;
            pScript.joystick = new MyJoystick();

            pScript.color = color;
            color = pColor;
            SetAuraColor(color);

            enableController();

            PlayerInput myPlayerInput0 = controller3DObject.GetComponent<PlayerInput>();
            PlayerInput myPlayerInput = PlayerInput.all[myPlayerInput0.playerIndex];
            InputUser myPlayerInputUser = myPlayerInput.user;


            print("myPlayerInput0.playerIndex " + myPlayerInput0.playerIndex);

            print("joystick.number " + joystick.number);

            print("Gamepad.all.Count " + Gamepad.all.Count);

            CheckDevices();

            /*
            print(" InputUser.all " + InputUser.all.Count);
            print(" myPlayerInputUser.valid " + myPlayerInputUser.valid);
            print(" myPlayerInputUser.id " + myPlayerInputUser.id);
            print("myPlayerInputUser.pairedDevices.Count " + myPlayerInputUser.pairedDevices.Count);
            print("myPlayerInput.devices.count " + myPlayerInput.devices.Count);
            print("myPlayerInputUser.index " + myPlayerInputUser.index);
            */
                // PlayerInput.all[
        }

        else
        {
            print("Key Control Swap");
            disableAI();

            hasJoystick = true;

            joystick = pScript.joystick;
            pScript.joystick = new MyJoystick();

            pScript.color = color;
            color = pColor;
            SetAuraColor(color);

            enableController();
        }

    }


    private Action<InputUser, InputUserChange, InputDevice> DeviceChange()
    {
     //   print("Device changed!");
        /*
        if (Gamepad.all.Count >= 1)
        {
            foreach (GameObject p in levelManager.GetPlayers())
            {
                if (p.GetComponent<Player>().hasJoystick)
                {
                    Player pScript = p.GetComponent<Player>();
                    PlayerInput pInput = pScript.controller3DObject.GetComponent<PlayerInput>();
                    InputUser pUser = pInput.user;
                   Gamepad g = Gamepad.all[pScript.joystick.number];
                   InputUser.PerformPairingWithDevice(g, pUser, InputUserPairingOptions.UnpairCurrentDevicesFromUser);
                }
              
            }
  
        }
        */
        return null;
    }

    public void CheckDevices()
    {
        if (Gamepad.all.Count >= 1)
        {
            foreach (GameObject p in levelManager.GetPlayers())
            {
                if (p.GetComponent<Player>().hasJoystick)
                {
                    Player pScript = p.GetComponent<Player>();
                    PlayerInput pInput = pScript.controller3DObject.GetComponent<PlayerInput>();
                    InputUser pUser = pInput.user;
                    Gamepad g = Gamepad.all[pScript.joystick.number];
                    InputUser.PerformPairingWithDevice(g, pUser, InputUserPairingOptions.UnpairCurrentDevicesFromUser);
                }
                 }
            }
        }

    private void enableController()
    {
        controller3DObject.SetActive(true);
        controller3D.enabled = true;
        PlayerInput playerInput = controller3DObject.GetComponent<PlayerInput>();
        if (playerInput)
        {
            playerInput.enabled = true;   // might be called twice .. (Controller3D.OnEnable())
        }

        playerConfigObject.GetComponent<Rigidbody>().isKinematic = false;
        
    }

    internal void SetOnStandby(bool v)
    {
        Rigidbody rigidbody = playerConfigObject.GetComponent<Rigidbody>();    // should prolly do this in playerConfig script

        rigidbody.isKinematic = v;


        if (hasJoystick)
        {
            if (v)
            {
                controller3D.DisablePlayerInputControls();
            }
            else
            {            
                controller3D.EnablePlayerInputControls();
            }
            
        }


        //print("hasAI")
        if (hasAI)


        {
            if (!v)
            {
                NavMeshAgent navMeshAgent = playerConfigObject.GetComponent<NavMeshAgent>();
                navMeshAgent.enabled = true;
            }
            else
            {
                NavMeshAgent navMeshAgent = playerConfigObject.GetComponent<NavMeshAgent>();
                navMeshAgent.enabled = false;
            }
        }
    }

    private void disableAI()
    {
        hasAI = false;

        aiObject.SetActive(false);
        aiScript.enabled = false;

        NavMeshAgent navMeshAgent = playerConfigObject.GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = false;

        playerAura.SetActive(false);
    }

    private void SetAuraColor(Color color)
    
    {
        playerAura.SetActive(true);

        GameObject psRoot = playerAura;
        ParticleSystem.MainModule rootPS = psRoot.GetComponent<ParticleSystem>().main;
        rootPS.startColor = new ParticleSystem.MinMaxGradient(color);

        /*
        int childCount = psRoot.transform.childCount;

        for (int i = 0; i < childCount; ++i)
        {
            GameObject childObject = psRoot.transform.GetChild(i).gameObject;
            ParticleSystem.MainModule childPS = childObject.GetComponent<ParticleSystem>().main;

            if (i == 1 || i == 2)
            {
                childPS.startColor = new ParticleSystem.MinMaxGradient(new Color(color.r, color.g, color.b, .1f));
            }
            else
            {
                childPS.startColor = new ParticleSystem.MinMaxGradient(color);
            }

        }
           */

    }

    public void enableController(int pIndex)    // init ..
    {
        hasJoystick = true;

        pIndex = playerIndex;

        if (joystick == null)                                  // might be obsolete since playerIndex is in PlayerInput
        {
            joystick = new MyJoystick(pIndex);
            print("joystick is null");

        }

        //  joystick.SetJoystick(playerIndex);

        controller3DObject.SetActive(true);
        controller3D.GetComponent<Controller3D>().enabled = true;

        PlayerInput playerInput = controller3DObject.GetComponent<PlayerInput>();    // might be called twice .. (Controller3D.OnEnable())
       
        if (playerInput)
        {
            playerInput.enabled = true;
        }


        hasAI = false;
        aiScript.enabled = false;
        aiObject.SetActive(false);


        NavMeshAgent navMeshAgent = playerConfigObject.GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = false;
        playerConfigObject.GetComponent<Rigidbody>().isKinematic = false;      // must handle w SetIsReadys ...


    }

    public void enableController(int joystickNumber, string type)    // ++  obsolete
    {

        hasAI = false;
        aiScript.enabled = false;


        if (aiObject.GetComponent<NavMeshAgent>())                                   // not sure why we don't do all these checks at start!  +
        {
            NavMeshAgent navMeshAgent = aiObject.GetComponent<NavMeshAgent>();
            navMeshAgent.enabled = false;
            playerConfigObject.GetComponent<Rigidbody>().isKinematic = false;
        }

        hasJoystick = true;
        joystick = new MyJoystick();
        joystick.SetJoystick(joystickNumber, type);


        controller3D.GetComponent<Controller3D>().enabled = true;
    }


    public void ToggleActivateDodge()
    {
        dodgeActivated = !dodgeActivated;

    }

    internal void PlayWintion()
    {
        playerConfigObject.GetComponent<PlayerConfiguration>().SwitchToWinAnimation();


    }


    public void DisablePlayer()
    {
        isOut = true;
       
        if (hasJoystick)
        {
            if (controller3D.ballGrabbed)
            {
                controller3D.DropBall();
            }

            controller3D.isKnockedOut = false;
            controller3D.playerConfigObject.GetComponent<PlayerConfiguration>().ballContact = false;
            hitObject.SetActive(false);
            playerAura.gameObject.SetActive(false);

        }

        else
        {
            if (aiScript.enabled)
            {
                if (aiScript.ballGrabbed)
                {
                    aiScript.DropBall();
                }
                aiScript.isKnockedOut = false;
                aiScript.playerConfigObject.GetComponent<PlayerConfiguration>().ballContact = false;
                aiScript.enabled = false;
                hitObject.SetActive(false);

                NavMeshAgent navMeshAgent = playerConfigObject.GetComponent<NavMeshAgent>();
                navMeshAgent.enabled = false;

            }
        }

        DeRender();

    }

    public void DeRender()
    {
        //playerConfigObject.GetComponent<PlayerConfiguration>().SetOutAnimation(true);
        isDeRendering = true;
        dR_Cool = 1f;                                       // careful here, things mess up
        drC_t0 = Time.realtimeSinceStartup;
        playerConfigObject.GetComponent<CapsuleCollider>().enabled = false;
        playerConfigObject.GetComponent<SphereCollider>().enabled = false;
        playerConfigObject.GetComponent<Rigidbody>().isKinematic = true;
        shadow.SetActive(false);
    }

    internal void playFootsteps()
    {
        if (!playerAudioSource.isPlaying)
        {
            playerAudioSource.volume = .85f;
            playerAudioSource.clip = footsteps;
            playerAudioSource.Play();
        }

    }

    private AudioClip GetThrowSound()
    {

        if (throwSounds.Length > 0)
        {
            return throwSounds[0];
        }
        else
        {
            return null;
        }
    }


    internal void PlayOutSound()
    {

        playerAudioSource.clip = outSound;
        playerAudioSource.pitch += UnityEngine.Random.Range(-3f, 3f);
        playerAudioSource.volume = .25f;
        playerAudioSource.Play();

        NormalAudioSource();

    }

    internal void PlayDodgeSound()
    {

        playerAudioSource.clip = dodgeSound;
        playerAudioSource.pitch += UnityEngine.Random.Range(1f, 1.5f);
        playerAudioSource.volume = .85f;
        playerAudioSource.Play();

        NormalAudioSource();

    }
    internal void playThrowSound()
    {

        playerAudioSource.volume = 1f;
        playerAudioSource.pitch += UnityEngine.Random.Range(1f, 1.5f);
        playerAudioSource.clip = GetThrowSound();
        playerAudioSource.Play();

        NormalAudioSource();
    }

    private void NormalAudioSource()
    {
        playerAudioSource.pitch = 1;
        playerAudioSource.volume = .5f;
    }

    public void SetPlayerIndex(int pi)
    {
        playerIndex = pi;
    }
    public int GetPlayerIndex()
    {
        return playerIndex;
    }

    internal void CreateJoystick(int pIndex, string type)
    {
        hasJoystick = true;
        joystick = new MyJoystick(pIndex, type);                      // change to start @ 0.   allign w pi.p
    }

    internal void CreateJoystick(int pIndex)
    {
        hasJoystick = true;
        joystick = new MyJoystick(pIndex);                      // change to start @ 0.   allign w pi.p
    }

    public MyJoystick GetJoystick()
    {
        if (joystick != null)
        {
            return joystick;
        }
        else
        {
            Debug.Log("No Joystick");
            return null;
        }
    }

    public void SetColor(Color c)
    
    {
        color = c;

        if (hasJoystick)
        {
            SetAuraColor(color);
        }

    }

    internal void SetHitFX(bool x)
    {
        hitObject.SetActive(x);
    }

    internal void TriggerCatchFX()
    {
        catchObject.SetActive(true);
        Invoke("DisableCatchFX", 2f);
    }

    internal void DisableCatchFX()
    {
        catchObject.SetActive(false);
  
    }

    internal void TriggerWinFX()
    {
        winObject.SetActive(true);
        Invoke("DisableWinFX", 2f);
    }
    internal void DisableWinFX()
    {
        winObject.SetActive(false);

    }



}
