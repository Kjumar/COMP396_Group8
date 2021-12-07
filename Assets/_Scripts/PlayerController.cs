using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Camera Controls")]
    [SerializeField] Transform firstPersonCamera;
    [SerializeField] Vector2 mouseSensitivity;

    [Header("Player Mobility")]
    [SerializeField] float jumpForce;
    [SerializeField] float speed;

    [Header("Ground check")]
    [SerializeField] Transform groundCheckPos;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundCheckMask;

    [Header("Misc")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject pauseMenu;

    private Vector2 rotation;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        SetLockMouse(true);
        rb = GetComponent<Rigidbody>();

        // see InGameHUDController
        FindObjectOfType<BuildHUDController>().SetMainPlayer(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            // when mouse is locked, read mouse movement and rotate the player and camera
            Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;
            rotation += mouseMovement;

            // limit rotation so the player doesn't flip on their head
            rotation.y = Mathf.Clamp(rotation.y, -80, 80);

            // apply rotation
            firstPersonCamera.localRotation = Quaternion.Euler(rotation.y, 0, 0);
            transform.localRotation = Quaternion.Euler(0, rotation.x, 0);

            // manual player movement
            Vector2 playerMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * speed * Time.deltaTime;
            transform.position = transform.position
                + (transform.right * playerMove.x)
                + (transform.forward * playerMove.y);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // jump
            if (Physics.CheckSphere(groundCheckPos.position, groundCheckRadius, groundCheckMask))
            {
                rb.AddForce(new Vector3(0, jumpForce, 0));
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu != null)
        {
            SetLockMouse(false);
            pauseMenu.SetActive(true);
        }
    }

    // locks or unlocks the cursor
    private void SetLockMouse(bool lockMouse)
    {
        if (lockMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("DeathPlane"))
        {
            // respawn the player
            transform.position = spawnPoint.position;
        }
    }

    public Vector3 GetCameraLookAt()
    {
        return firstPersonCamera.transform.forward;
    }
}
