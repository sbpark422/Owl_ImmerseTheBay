using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReboundLinkFX : MonoBehaviour
{
    Vector3 reboundPlayerFXPos;
    Vector3 reboundBallFXPos;

     float trackSpeed = .5f;

    bool isTracking;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTracking)
        {
            if ( Vector3.Distance(this.transform.position, reboundPlayerFXPos) > 3.0f)
            {
                Vector3 trackVec = (reboundPlayerFXPos - this.transform.position).normalized;
                this.transform.position += trackVec * trackSpeed;
            }
            else
            {
                isTracking = false;
                print("Destroy track linkFX");
                Destroy(this.gameObject);
            }
        }
    }

    public void Track(Vector3 reboundPlayerPos, Vector3 reboundBallPos)
    {
        isTracking = true;
        reboundPlayerFXPos = reboundPlayerPos;
        reboundBallFXPos = reboundBallPos;
    }

}
