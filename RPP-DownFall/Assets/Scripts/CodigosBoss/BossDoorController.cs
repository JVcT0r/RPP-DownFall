using UnityEngine;

public class BossDoorController : MonoBehaviour
{
    public float moveDistance = 30f;
    public float moveTime = 0.5f;
    public bool moveRight = true;

    private bool opening = false;

    public void OpenDoor()
    {
        if (opening) return;
        opening = true;

        Vector3 start = transform.position;
        Vector3 target = moveRight
            ? start + new Vector3(moveDistance, 0, 0)
            : start + new Vector3(-moveDistance, 0, 0);

        StartCoroutine(OpenDoorRoutine(start, target));
    }

    private System.Collections.IEnumerator OpenDoorRoutine(Vector3 start, Vector3 target)
    {
        float t = 0;

        while (t < moveTime)
        {
            transform.position = Vector3.Lerp(start, target, t / moveTime);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }
}