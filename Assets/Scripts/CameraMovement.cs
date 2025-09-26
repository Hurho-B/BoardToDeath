using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    //Making the movements of the player public so the camera can reference them
    public Transform orientation;
    public Transform player;
    public Transform deathboard;
    public Rigidbody rb;

    public float rotationSpeed;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //rotate orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        //rotate player object
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //smoothes camera if there's an input
        if (inputDir != Vector3.zero)
            deathboard.forward = Vector3.Slerp(deathboard.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
    }

}
