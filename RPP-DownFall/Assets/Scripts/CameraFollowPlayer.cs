using UnityEngine;
public class CameraFollowPlayer : MonoBehaviour
{
    GameObject player;
    private bool followPlayer = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (followPlayer)
        {
            CamFollowPlayer();
        }
    }

    public void SetFollowPlayer(bool follow)
    {
        followPlayer = follow;
    }

    void CamFollowPlayer()
    {
        Vector3 newPosition = new Vector3(player.transform.position.x, player.transform.position.y, this.player.transform.position.z);
        this.transform.position = newPosition;
    }
}
