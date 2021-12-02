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

    private Vector2 rotation;
    private bool isMouseLocked = false;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        SetLockMouse(true);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMouseLocked)
        {
            // when mouse is locked, read mouse movement and rotate the player and camera
            Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;
            rotation += mouseMovement;

            // limit rotation so the player doesn't flip on their head
            rotation.y = Mathf.Clamp(rotation.y, -80, 80);

            // apply rotation
            firstPersonCamera.localRotation = Quaternion.Euler(rotation.y, 0, 0);
            transform.localRotation = Quaternion.Euler(0, rotation.x, 0);

            // below is player movement
            Vector2 playerMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed * Time.deltaTime;
            transform.position = transform.position
                + (transform.right * playerMove.x)
                + (transform.forward * playerMove.y);
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetLockMouse(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // jump
            if (Physics.CheckSphere(groundCheckPos.position, groundCheckRadius, groundCheckMask))
            {
                rb.AddForce(new Vector3(0, jumpForce, 0));
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetLockMouse(false);
        }
    }

    // locks or unlocks the cursor
    private void SetLockMouse(bool lockMouse)
    {
        if (lockMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isMouseLocked = true;
            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isMouseLocked = false;
        }
    }
}
