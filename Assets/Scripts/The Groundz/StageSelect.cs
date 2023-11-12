using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour {

public void selectScene()
    {
      
        switch (this.gameObject.name)
        {
            case "TheGroundzEastButton":
                GlobalConfiguration.Instance.SetStage("theGroundzEast");
                GlobalConfiguration.Instance.TurnThemeMusic(false);
                SceneManager.LoadScene("TheGroundzEast");
                break;
            case "TheGymButton":
                GlobalConfiguration.Instance.SetStage("theGym");
                GlobalConfiguration.Instance.TurnThemeMusic(false);
                SceneManager.LoadScene("TheGym");
                break;
            case "TheBlockButton":
                GlobalConfiguration.Instance.SetStage("theBlock");
                GlobalConfiguration.Instance.TurnThemeMusic(false);
                SceneManager.LoadScene("TheBlock");
                break;

            case "TheLibraryButton":
                GlobalConfiguration.Instance.SetStage("theBlock");
                GlobalConfiguration.Instance.TurnThemeMusic(false);
                SceneManager.LoadScene("TheBlock");
                break;

            case "TheGroundzWestButton":
                GlobalConfiguration.Instance.SetStage("theBlock");
                GlobalConfiguration.Instance.TurnThemeMusic(false);
                SceneManager.LoadScene("TheBlock");
                break;
            case "TheBackyardButton":
                GlobalConfiguration.Instance.SetStage("theBackyard");
                GlobalConfiguration.Instance.TurnThemeMusic(false);
                SceneManager.LoadScene("TheBackyard");
                break;

        }
    }
}
