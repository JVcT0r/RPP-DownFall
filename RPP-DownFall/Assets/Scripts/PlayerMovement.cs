using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    public bool moving = false;
    float speed = 5.0f;

    // Use this for initialization
    void Start () 
    {
        
    }

    // Update is called once per frame
    void Update () 
    {
        movement ();
    }

    void movement() 
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) 
        {
            direction += Vector3.up;
        }
        if (Input.GetKey(KeyCode.S)) 
        {
            direction += Vector3.down;
        }
        if (Input.GetKey(KeyCode.A)) 
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D)) 
        {
            direction += Vector3.right;
        }

        if (direction != Vector3.zero) 
        {
            direction.Normalize(); // Normaliza para evitar velocidade duplicada na diagonal
            transform.Translate(direction * (speed * Time.deltaTime), Space.World);
            moving = true;
        } 
        
        else 
        {
            moving = false;
        }
    }
}
