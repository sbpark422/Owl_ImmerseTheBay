using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class GameManager : MonoBehaviour
{


    GameObject LogFeed;

    public LevelManager levelManager;

    GlobalConfiguration globalConfiguration;

    public GameObject GMFX;

    public static int ageThresh;

    public PlayerInputManager playerInputManager;

    public GameObject audioManager;


}