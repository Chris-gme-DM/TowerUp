using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Range(0f, 5f)] public float lookSensitivityX;
    [Range(0f, 5f)] public float lookSensitivityY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
