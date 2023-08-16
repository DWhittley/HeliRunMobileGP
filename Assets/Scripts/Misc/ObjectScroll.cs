using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScroll : MonoBehaviour
{
    public float minZPos = -2.0f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.forward * GameManager.scrollSpeed;
    }

    void Update()
    {
        rb.velocity = Vector3.forward * GameManager.scrollSpeed; // Asjust velocity to new speed

        if (transform.position.z < minZPos)
        {
            Destroy(gameObject);
        }
    }
}