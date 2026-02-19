using UnityEngine;

public class Plastic_BallDropper : MonoBehaviour
{
    public Rigidbody ball;

    public void DropBall()
    {
        ball.useGravity = true;     // let the ball fall
        // ball.constraints = RigidbodyConstraints.None; // remove freeze
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
