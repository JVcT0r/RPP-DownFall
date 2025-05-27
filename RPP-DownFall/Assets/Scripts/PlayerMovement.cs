using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public bool moving = false;
    public float speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }
    
    void Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.up * (speed * Time.deltaTime), Space.World);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * (speed * Time.deltaTime), Space.World);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * (speed * Time.deltaTime), Space.World);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.down * (speed * Time.deltaTime), Space.World);
        }

        if (Input.GetKey(KeyCode.W) != true && Input.GetKey(KeyCode.A) != true && Input.GetKey(KeyCode.D) != true &&
            Input.GetKey(KeyCode.S) != true)
        {
            moving = false;
        }
        
    }
}
