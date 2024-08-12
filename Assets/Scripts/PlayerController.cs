using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PhotonView photonView;
    public GameObject mark;

    public float moveSpeed = 5f;           // Speed of the character
    public float gravity = -9.81f;         // Gravity force
    public float jumpHeight = 1.5f;        // Height of the jump

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
            mark.SetActive(true);

        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
            HandleMovement();
    }

    private void HandleMovement()
    {
        // Check if the character is on the ground
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Ensures the character stays grounded
        }

        // Get input from keyboard
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right arrow keys
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down arrow keys

        // Calculate movement direction
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Move the character
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Jump logic
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
