using UnityEngine;
using System.Collections;

public class BallLauncher : MonoBehaviour
{
    public GameObject ballPrefab; // The prefab of the ball to be thrown
    public Transform launchPoint; // The point from which the ball is launched
    public float launchInterval = 3.0f; // Time interval between launches
    public float launchSpeed = 10f; // Speed at which the ball is launched
    public float launchAngle = 45f; // Angle of launch

    private void Start()
    {
        StartCoroutine(LaunchBallAtInterval());
    }

    private IEnumerator LaunchBallAtInterval()
    {
        while (true)
        {
            yield return new WaitForSeconds(launchInterval);
            LaunchBall();
        }
    }

    private void LaunchBall()
    {
        // Instantiate the ball at the launch point
        GameObject ball = Instantiate(ballPrefab, launchPoint.position, Quaternion.identity);

        // Calculate launch velocity
        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
        if (ballRigidbody != null)
        {
            Vector3 launchDirection = Quaternion.AngleAxis(-launchAngle, launchPoint.right) * launchPoint.forward;
            ballRigidbody.velocity = launchDirection * launchSpeed;
        }
    }
}
