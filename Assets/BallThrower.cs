using UnityEngine;
using System.Collections;

public class BallThrower : MonoBehaviour
{
    public Transform xrOrigin; // Reference to the XR Origin
    public GameObject ballPrefab; // The prefab of the ball to be thrown
    public float throwInterval = 2.0f; // Time interval between throws
    public Transform spawnLocation; // Single spawn location

    private void Start()
    {
        StartCoroutine(ThrowBallAtInterval());
    }

    private IEnumerator ThrowBallAtInterval()
    {
        while (true)
        {
            yield return new WaitForSeconds(throwInterval);
            ThrowBall();
        }
    }

    private void ThrowBall()
    {
        // Instantiate the ball at the spawn location
        GameObject ball = Instantiate(ballPrefab, spawnLocation.position, Quaternion.identity);

        // Calculate parabolic trajectory
        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
        if (ballRigidbody != null)
        {
            ballRigidbody.velocity = CalculateParabolicVelocity(spawnLocation.position, xrOrigin.position, 1f); // Adjust the time parameter as needed
        }
    }

    private Vector3 CalculateParabolicVelocity(Vector3 start, Vector3 end, float time)
    {
        Vector3 distance = end - start;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;

        float sY = distance.y;
        float sXZ = distanceXZ.magnitude;

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * sY);
        Vector3 velocityXZ = distanceXZ / time;

        return velocityXZ + velocityY * -Mathf.Sign(Physics.gravity.y);
    }
}
