using UnityEngine;

public class AllBalls_BallDrpper : MonoBehaviour
{
    public Rigidbody[] balls;

    public void DropAll()
    {
        foreach (var rb in balls)
        {
            rb.useGravity = true;
            // rb.constraints = RigidbodyConstraints.None;
        }
    }
}