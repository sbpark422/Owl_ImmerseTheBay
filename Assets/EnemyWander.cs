using UnityEngine;

public class EnemyWander : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float turnSpeed = 60f;
    public float pauseDuration = 2f;

    private Rigidbody rb;
    private float moveTimer;
    private float pauseTimer;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveDirection = Vector3.forward;
        moveTimer = pauseDuration;
        pauseTimer = 0f;
    }

    void Update()
    {
        // Update the timers
        moveTimer -= Time.deltaTime;
        pauseTimer -= Time.deltaTime;

        if (pauseTimer <= 0)
        {
            if (moveTimer <= 0)
            {
                // Time to pause and pick a new direction
                moveTimer = pauseDuration;
                pauseTimer = Random.Range(1f, 3f); // Random pause duration between 1 and 3 seconds
                PickRandomDirection();
            }
            else
            {
                // Move in the current direction
                rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
            }
        }
    }

    void PickRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
        moveDirection = rotation * Vector3.forward;
    }
}
