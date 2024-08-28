using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PhotonView photonView;
    public GameObject mark;
    //public GameObject cameraMain;

    public float moveSpeed = 5f;           // Speed of the character
    public float gravity = -9.81f;         // Gravity force
    public float jumpHeight = 1.5f;        // Height of the jump

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    public GameObject bullet;
    public Transform bulletSpawnPos;
    public float bulletSpeed;

    private Camera mainCamera;


    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            mark.SetActive(true);
            mainCamera = Camera.main;
            //cameraMain.SetActive(true);
        }
            

        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            HandleMovement();
            RotateTowardsMouse();
            //if (Input.GetKeyDown(KeyCode.F))
            if (Input.GetMouseButtonDown(1))
            {
                //photonView.RPC(nameof(HandleShoot), RpcTarget.All);
                //HandleShootTest();
                photonView.RPC(nameof(HandleShootTest), RpcTarget.All);
            }
            //HandleShoot();
        }

    }

    [PunRPC]
    private void HandleShoot()
    {
        GameObject bulletObject = GameObject.Instantiate(bullet, bulletSpawnPos.position, Quaternion.identity);
        bulletObject.GetComponent<Rigidbody>().velocity = Vector3.forward * bulletSpeed;

    }

    [PunRPC]
    private void HandleShootTest()
    {
        //GameObject bulletObject = GameObject.Instantiate(bullet, bulletSpawnPos.position, Quaternion.identity);
        //bulletObject.GetComponent<Rigidbody>().velocity = bulletSpawnPos.forward * bulletSpeed;
        GetComponent<ECprojectileActor>().Fire();

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

    private void RotateTowardsMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 targetPosition = hitInfo.point;
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f; // Keep the rotation in the horizontal plane
            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10f); // Smooth rotation
            }
        }
    }
}
