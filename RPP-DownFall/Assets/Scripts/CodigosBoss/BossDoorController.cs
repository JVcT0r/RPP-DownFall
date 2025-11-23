using UnityEngine;

public class BossDoorController : MonoBehaviour
{
    [Header("Configurações da Porta")]
    public float moveDistance = 5f;   
    public float moveTime = 0.5f;     
    public bool moveRight = true;     

    private bool opened = false;

    public void OpenDoor()
    {
        if (opened) return;
        opened = true;

        Vector3 start = transform.position;
        Vector3 target;

        if (moveRight)
            target = start + new Vector3(moveDistance, 0, 0);   
        else
            target = start + new Vector3(-moveDistance, 0, 0);  

        StartCoroutine(MoveDoor(start, target));
    }

    private System.Collections.IEnumerator MoveDoor(Vector3 start, Vector3 target)
    {
        float t = 0f;

        while (t < moveTime)
        {
            transform.position = Vector3.Lerp(start, target, t / moveTime);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }
}