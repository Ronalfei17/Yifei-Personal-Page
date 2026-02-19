using UnityEngine;

public class MR_ResetAllBalls_Script: MonoBehaviour
{
    public Transform[] balls;
    private Vector3[] originalPositions;
    private Quaternion[] originalRotations;

    void Start()
    {
        originalPositions = new Vector3[balls.Length];
        originalRotations = new Quaternion[balls.Length];

        // Store original transforms at scene start
        for (int i = 0; i < balls.Length; i++)
        {
            originalPositions[i] = balls[i].position;
            originalRotations[i] = balls[i].rotation;
        }
    }

    public void ResetBalls()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].position = originalPositions[i];
            balls[i].rotation = originalRotations[i];

            Rigidbody rb = balls[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;

            }
        }
    }
}
