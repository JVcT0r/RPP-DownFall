using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lanterna : MonoBehaviour
{
    [Header("Lanterna")]
    public Transform flashlight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Ativar/desativar lanterna com F
        if (Input.GetKeyDown(KeyCode.F) && flashlight != null)
        {
            var light2D = flashlight.GetComponent<Light2D>();
            if (light2D != null)
                light2D.enabled = !light2D.enabled;
        }
    }
}
