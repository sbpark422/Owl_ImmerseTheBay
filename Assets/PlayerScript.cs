using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerScript : MonoBehaviour
{
    public float throwForce = 2.0f;
    public Material thrownMaterial;
    private XRDirectInteractor interactor;

    private void Awake()
    {
        interactor = GetComponent<XRDirectInteractor>();
    }

    private void OnEnable()
    {
        interactor.selectEntered.AddListener(GrabBall);
        interactor.selectExited.AddListener(ThrowBall);
    }

    private void OnDisable()
    {
        interactor.selectEntered.RemoveListener(GrabBall);
        interactor.selectExited.RemoveListener(ThrowBall);
    }

    private void GrabBall(SelectEnterEventArgs arg)
    {
        // Check if the grabbed object is tagged as 'ball'
        if (arg.interactableObject.transform.CompareTag("ball"))
        {
            // Handle ball grabbing logic if needed
        }
    }

    private void ThrowBall(SelectExitEventArgs arg)
    {
        // Check if the released object is tagged as 'ball'
        if (arg.interactableObject.transform.CompareTag("ball"))
        {
            Rigidbody ballRigidbody = arg.interactableObject.transform.GetComponent<Rigidbody>();
            if (ballRigidbody != null)
            {
                // Apply force to throw the ball

                Debug.Log("Throw");
                Vector3 throwDirection = arg.interactorObject.transform.forward;
                ballRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);

                ChangeBallMaterial(arg.interactableObject.transform, thrownMaterial);

            }
        }
    }

    private void ChangeBallMaterial(Transform objectTransform, Material newMaterial)
    {
        Renderer objectRenderer = objectTransform.GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            objectRenderer.material = newMaterial;
        }
    }
}