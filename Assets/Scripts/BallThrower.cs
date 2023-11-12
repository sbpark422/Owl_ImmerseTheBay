using UnityEngine;
using System.Collections;
using Foundry.Networking;
using Photon.Voice;
using System.Linq;
using Foundry;

public class BallLauncher : NetworkComponent
{
    public GameObject ballPrefab; // The prefab of the ball to be thrown
    public Transform launchPoint; // The point from which the ball is launched
    private Transform target; // Where the enemy launches balls towards
    public float launchInterval = 3.0f; // Time interval between launches
    // public float launchSpeed = 10f; // Speed at which the ball is launched
    public float firingAngle = 45.0f;
    public float gravity = 9.8f;
    public bool startCalled = false;
    private Vector3 targetpos;

    /*    public override void OnConnected()
        {
            startCalled = true;

            StartCoroutine(LaunchBallAtInterval());
        }*/

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
        if (!IsOwner) return;

        // Instantiate the ball at the launch point
        var spawnedPlayers = FindObjectsByType<Foundry.Player>(FindObjectsSortMode.None);
        Debug.Log("Spawned players: " + spawnedPlayers.Length);
        if (spawnedPlayers.Length > 0)
        {
            target = spawnedPlayers[Random.RandomRange(0, spawnedPlayers.Length - 1)].transform;
            targetpos = target.position;
        }
        else
            targetpos = new Vector3(0f, 5f, 0f);
        // if (!target)
        //    return;


        GameObject ball = NetworkManager.instance.Instantiate(ballPrefab, launchPoint.position, Quaternion.identity);
        ball.GetComponent<NetworkObject>().RequestOwnership();
        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();

        Vector3 AimPosition = targetpos + new Vector3(Random.RandomRange(-1f, 1f), Random.RandomRange(-1f, 1f), Random.RandomRange(-1f, 1f));

        // Calculate distance to target
        float targetDistance = Vector3.Distance(launchPoint.position, AimPosition);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectileVelocity = targetDistance / (Mathf.Sin(2 * (firingAngle+Random.RandomRange(-10f,10f)) * Mathf.Deg2Rad) / gravity);

        // Extract the X & Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectileVelocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectileVelocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = targetDistance / Vx;

        // Rotate projectile to face the target.
        ball.transform.rotation = Quaternion.LookRotation(AimPosition - launchPoint.position);

        // Set the velocity
        ballRigidbody.velocity = ball.transform.forward * Vx + ball.transform.up * Vy;



    }
}
