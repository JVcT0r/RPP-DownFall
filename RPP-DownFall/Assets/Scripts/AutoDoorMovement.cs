using UnityEngine;

public class AutoDoorMoverNavmesh : MonoBehaviour
{
    public Transform door;
    public Vector3 openOffset = new Vector3(1.5f, 0, 0);
    public float openTime = 0.25f;
    public float closeDelay = 1.5f;
    public string[] allowedTags = { "Player", "Inimigo" };

    private Vector3 closedPos;
    private bool isOpen = false;

    private AudioSource audioSource;
    public AudioClip portaAbrindoSFX;
    public AudioClip portaFechandoSFX;

    void Start()
    {
        closedPos = door.localPosition;
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        foreach (var tag in allowedTags)
        {
            if (other.CompareTag(tag))
            {
                StopAllCoroutines();
                StartCoroutine(OpenDoor());
                break;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        foreach (var tag in allowedTags)
        {
            if (other.CompareTag(tag))
            {
                StopAllCoroutines();
                StartCoroutine(CloseDoor());
                break;
            }
        }
    }

    private System.Collections.IEnumerator OpenDoor()
    {
        if (isOpen) yield break;
        audioSource.PlayOneShot(portaAbrindoSFX);
        isOpen = true;

        Vector3 target = closedPos + openOffset;
        float t = 0;
        while (t < openTime)
        {
            door.localPosition = Vector3.Lerp(closedPos, target, t / openTime);
            t += Time.deltaTime;
            yield return null;
        }
        door.localPosition = target;
    }

    private System.Collections.IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(closeDelay);
        Vector3 target = closedPos;
        Vector3 start = door.localPosition;
        float t = 0;
        while (t < openTime)
        {
            door.localPosition = Vector3.Lerp(start, target, t / openTime);
            t += Time.deltaTime;
            yield return null;
        }
        door.localPosition = target;
        audioSource.PlayOneShot(portaFechandoSFX);
        isOpen = false;
    }
}
