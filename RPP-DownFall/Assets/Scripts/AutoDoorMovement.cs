using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class AutoDoorMoverNavmesh : MonoBehaviour
{
     [Header("Quem pode abrir")]
    public string[] allowedTags = { "Player", "Inimigo" };

    [Header("Referências")]
    public Transform doorTransform;           
    public Collider2D blockingCollider;       
    public NavMeshObstacle navObstacle;      
    public Vector3 openOffset = new Vector3(1.5f, 0, 0);

    [Header("Tempos")]
    public float openTime = 0.25f;
    public float closeDelay = 1.5f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen = false;
    private Coroutine closeCoroutine;

    private void Start()
    {
        if (doorTransform == null) doorTransform = transform;
        closedPos = doorTransform.localPosition;
        openPos = closedPos + openOffset;

        if (blockingCollider == null)
            blockingCollider = GetComponent<Collider2D>();

        if (navObstacle == null)
            navObstacle = GetComponent<NavMeshObstacle>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach (var tag in allowedTags)
        {
            if (other.CompareTag(tag))
            {
                if (!isOpen)
                    StartCoroutine(OpenDoor());
                return;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        foreach (var tag in allowedTags)
        {
            if (other.CompareTag(tag))
            {
                if (closeCoroutine != null)
                    StopCoroutine(closeCoroutine);
                closeCoroutine = StartCoroutine(CloseDoorAfterDelay());
                return;
            }
        }
    }

    private IEnumerator OpenDoor()
    {
        isOpen = true;

        
        if (blockingCollider != null) blockingCollider.enabled = false;
        if (navObstacle != null) navObstacle.carving = false;

        Vector3 startPos = doorTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < openTime)
        {
            doorTransform.localPosition = Vector3.Lerp(startPos, openPos, elapsed / openTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        doorTransform.localPosition = openPos;

        
        NavMeshUpdater.UpdateNavMeshGlobal();
    }

    private IEnumerator CloseDoorAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);

        Vector3 startPos = doorTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < openTime)
        {
            doorTransform.localPosition = Vector3.Lerp(startPos, closedPos, elapsed / openTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        doorTransform.localPosition = closedPos;

        if (blockingCollider != null) blockingCollider.enabled = true;
        if (navObstacle != null) navObstacle.carving = true;

        
        NavMeshUpdater.UpdateNavMeshGlobal();

        isOpen = false;
    }
}
