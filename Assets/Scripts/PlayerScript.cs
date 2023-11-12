using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerScript : MonoBehaviour
{
    private enum PlayerState { Idle, Grab, Throw, Hit }
    private PlayerState currentState = PlayerState.Idle;
    private XRDirectInteractor interactor;
    public GameObject parentObject;


    private void Awake()
    {   Debug.Log("TRYYYYYY111111");
        interactor = GetComponent<XRDirectInteractor>();
        // parentObject = transform.parent.gameObject;

    }

    private void OnEnable()
    {
        interactor.selectEntered.AddListener(OnGrab);
        interactor.selectExited.AddListener(OnThrow);
    }

    private void OnDisable()
    {
        interactor.selectEntered.RemoveListener(OnGrab);
        interactor.selectExited.RemoveListener(OnThrow);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Entered");
        // Check if the collision object is the parent object
        if (collision.gameObject == parentObject)
        {
            if (collision.collider.CompareTag("ball"))
            {
                //VRDebug.Instance.Log("Collision with " + collision.gameObject.name);
                // Code for when the parent collider collides with a ball
                // Debug.Log("Parent Hit by Ball");
                // Additional logic for when the parent is hit
            }
        }
    }

    private void OnGrab(SelectEnterEventArgs arg)
    {
        if (arg.interactableObject.transform.CompareTag("ball"))
        {
            currentState = PlayerState.Grab;
            Debug.Log("Ball Grabbed");
            // Additional logic for when the ball is grabbed
        }
    }

    private void OnThrow(SelectExitEventArgs arg)
    {
        if (arg.interactableObject.transform.CompareTag("ball"))
        {
            currentState = PlayerState.Throw;
            Debug.Log("Ball Thrown");
            // Additional logic for when the ball is thrown
        }
    }

    // Optional: Method to reset state to Idle
    public void ResetState()
    {
        currentState = PlayerState.Idle;
    }
}
