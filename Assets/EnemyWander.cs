using UnityEngine;

public class EnemyWander : Foundry.Networking.NetworkComponent
{
    public float moveSpeed = 2f;
    public float turnSpeed = 60f;
    public float pauseDuration = 2f;

    private Rigidbody rb;
    private float moveTimer;
    private float pauseTimer;
    private Vector3 moveDirection;

    public float weightTowardsOrigin = 70f;  // 70% chance to move towards origin
    public float weightRandomDirection = 30f; // 30% chance to move randomly


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveDirection = Vector3.forward;
        moveTimer = pauseDuration;
        pauseTimer = 0f;

        float angle = Random.Range(0f, 360f);
        Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
        moveDirection = rotation * Vector3.forward;
    }

    void Update()
    {
        if(!IsOwner) return;

        // Update the timers
        moveTimer -= Time.deltaTime;
        pauseTimer -= Time.deltaTime;

        if (pauseTimer <= 0)
        {
            if (moveTimer <= 0)
            {
                // Time to pause and pick a new direction
                pauseTimer = Random.Range(1f, 3f); // Random pause duration between 1 and 3 seconds
                moveTimer = pauseTimer + pauseDuration*Random.Range(0f,1f);
                moveDirection = ChooseDirection();
            }
            else
            {
                // Move in the current direction
                rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
            }
        }
    }


    private Vector3 ChooseDirection()
    {
        float totalWeight = weightTowardsOrigin + weightRandomDirection;
        float randomValue = Random.Range(0, totalWeight);

        if (randomValue <= weightTowardsOrigin)
        {
            // Move towards the origin
            Vector3 directionToOrigin = (Vector3.zero - transform.position).normalized;
            return directionToOrigin;
        }
        else
        {
            // Move in a random direction
            return new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        }
    }
}
