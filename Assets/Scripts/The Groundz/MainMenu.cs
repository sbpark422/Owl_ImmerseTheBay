using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public bool SettingsIsOpen = false;

    public GameObject settingsMenuUI;

    public GameObject image;

    private void Start()
    {
        GlobalConfiguration.Instance.GetJoysticks();
    }


    public void PlayLocal()
    {
        GlobalConfiguration.Instance.SetGameMode("local");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayMultiplayer()

    {
        GlobalConfiguration.Instance.SetGameMode("multiplayer");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void PlayStory()
    {
        GlobalConfiguration.Instance.SetGameMode("story");
        SceneManager.LoadScene("StoryMode");
    }

    public void Menu()
    {
        SceneManager.LoadScene("GamemodeMenu");
    }


    public void PlayCredits()
    {
        SceneManager.LoadScene("CreditsRolling");
    }
        
	public void PlayControls()
	{
		SceneManager.LoadScene("Controls");
	}

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchrously(sceneIndex));
    }

    IEnumerator LoadAsynchrously(int sceneIndex)
    {
        image.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            Debug.Log(operation.progress);
            yield return null;
        }
    }

    void Update()
    {
         /*
        if (Input.GetJoystickNames().Length >= 1 || (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor))
        {
            GameManager.view = "Keyboard";
             Controller3D.mode = "Keyboard";

        }
        else
        {
            if (GameManager.view != "Virtual Joystick")
            {
                GameManager.view = "Virtual Joystick";
                Controller3D.mode = "Virtual Joystick";
            }
        }
         */
    }

}
