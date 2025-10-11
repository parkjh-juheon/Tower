using UnityEngine;
using System;

public class PlayerFallTracker : MonoBehaviour
{
    public float fallHeightThreshold = 5f;   
    public event Action OnHighFallLanded;    

    private Rigidbody2D rb;
    private float startFallY;
    private bool isFalling;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float velocityY = rb.linearVelocity.y;

        if (velocityY < -0.1f && !isFalling)
        {
            isFalling = true;
            startFallY = transform.position.y;
        }

        if (isFalling && Mathf.Abs(velocityY) < 0.05f)
        {
            float fallDistance = startFallY - transform.position.y;

            if (fallDistance >= fallHeightThreshold)
            {
                Debug.Log($"높은 곳에서 착지! (낙하 거리: {fallDistance:F2})");
                OnHighFallLanded?.Invoke();  
            }

            isFalling = false;
        }
    }
}
