using UnityEngine;

public class Table_ResetAllBalls_Script: MonoBehaviour
{
    public Transform[] balls;
    private Vector3[] originalPositions;
    private Quaternion[] originalRotations;

    void Start()
    {
        originalPositions = new Vector3[balls.Length];
        originalRotations = new Quaternion[balls.Length];

        for (int i = 0; i < balls.Length; i++)                                     // To store original transforms at scene start
        {
            originalPositions[i] = balls[i].position;                              // To store original position at scene start in a list
            originalRotations[i] = balls[i].rotation;                              // To store original transforms at scene start in a list
        }
    }

    public void TableResetBalls()
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
            }
        }
    }
}
