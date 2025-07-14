using UnityEngine;

public class RespawnManager : MonoBehaviour
{
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
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") )
        {
            Debug.Log(other.CompareTag("Player"), other.gameObject);
            RespawnPlayer();
        }
    }
    private void RespawnPlayer()
    {
        Debug.Log("I should respawn the player now");
        // Return player to respawn point
        rb.transform.position = respawnPoint.transform.position;
        // Stop player
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

    }
}
