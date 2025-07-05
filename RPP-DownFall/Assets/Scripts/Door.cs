using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField]
    private float openAngle = 90f; 

    [SerializeField]
    private float openSpeed = 90f; 

    private bool isOpening = false;
    private float currentAngle = 0f;

    public void Interact()
    {
        if (!isOpening && currentAngle < openAngle)
        {
            isOpening = true;
        }
    }

    private void Update()
    {
        if (isOpening && currentAngle < openAngle)
        {
            float angleStep = openSpeed * Time.deltaTime;
            float angleToRotate = Mathf.Min(angleStep, openAngle - currentAngle);

            transform.Rotate(0f, 0f, angleToRotate);
            currentAngle += angleToRotate;

            if (currentAngle >= openAngle)
            {
                isOpening = false;
            }
        }
    }
}


