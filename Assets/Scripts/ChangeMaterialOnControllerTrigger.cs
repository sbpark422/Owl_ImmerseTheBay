using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ChangeMaterialOnControllerTrigger : MonoBehaviour
{
    public Material newMaterial; // Assign this in the inspector
    public GameObject objectToChange; // Assign this in the inspector
    public XRController controller;
    

    private bool isTriggerPressed = false;

    void Update()
    {
        // List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();
        // InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, devices);

        // foreach (var device in devices)
        // {
        //     if (device.TryGetFeatureValue(CommonUsages.triggerButton, out isTriggerPressed) && isTriggerPressed)
        //     {
        //         ChangeMaterial();
        //         break; // Exit the loop once the material is changed
        //     }
        // }

                // Check if the trigger is pressed
        if (controller.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool isPressed) && isPressed)
        {
            // VRDebug.Instance.Log("Trigger Pressed");
            ChangeMaterial();
        }
    }

    void ChangeMaterial()
    {
        Renderer renderer = objectToChange.GetComponent<Renderer>();
        if (renderer != null && newMaterial != null)
        {
            renderer.material = newMaterial;
        }
    }
}
