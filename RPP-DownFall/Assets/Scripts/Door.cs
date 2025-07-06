using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField]
    private float openAngle = 90f;

    [SerializeField]
    private float openSpeed = 90f;

    private bool isMoving = false;
    private bool isOpen = false;
    private float targetAngle = 0f;

    private void Update()
    {
        if (isMoving)
        {
            float currentZ = transform.eulerAngles.z;
            float angleStep = openSpeed * Time.deltaTime;

            
            if (currentZ > 180) currentZ -= 360;

            float angleDiff = targetAngle - currentZ;
            float angleMove = Mathf.Clamp(angleDiff, -angleStep, angleStep);

            transform.Rotate(0f, 0f, angleMove);

            if (Mathf.Abs(angleDiff) < 0.1f)
            {
                transform.eulerAngles = new Vector3(0f, 0f, targetAngle);
                isMoving = false;
            }
        }
    }

    public void Interact()
    {
        if (isMoving) return;

        isOpen = !isOpen;
        targetAngle = isOpen ? openAngle : 0f;
        isMoving = true;
    }
}



