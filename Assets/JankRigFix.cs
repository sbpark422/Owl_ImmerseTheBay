using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class JankRigFix : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(QueueFix());
    }

    IEnumerator QueueFix()
    {
        var origin = GetComponent<XROrigin>();

        yield return new WaitForSeconds(2);
        origin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Device;
        yield return null;
        origin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Floor;
    }
}
