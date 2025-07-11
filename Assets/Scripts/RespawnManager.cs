using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private string instantDeathTag;
    [SerializeField] private Transform respawnPoint;

    private Transform player;
    private Rigidbody rb;

    private void Awake()
    {
        // Get all referenced Objects i haven't assigned in inspector already
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        { 
            player = playerObject.transform;
            rb = playerObject.GetComponent<Rigidbody>();
        }
        if(playerObject == null)
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.CompareTag(instantDeathTag))
        {
            RespawnPlayer();
        }
    }
    private void RespawnPlayer()
    {
        // Return player to respawn point
        player.position = respawnPoint.position;
        // Stop player
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

    }
}
