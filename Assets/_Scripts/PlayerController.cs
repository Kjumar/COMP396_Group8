using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
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

    [Header("Health")]
    [SerializeField] int maxHealth = 50;
    [SerializeField] int currentHealth = 50;
    [SerializeField] UIHealthBar healthbar;
    [SerializeField] GameObject gameOverScreen;

    [Header("Sounds")]
    [SerializeField]
    Sounds[] jumpSounds;
    [SerializeField]
    Sounds[] takeDamageSounds;
    [SerializeField]
    AudioSource walk;

    private Sounds soundScript = new Sounds();

    [Header("Misc")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject pauseMenu;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Camera playerCam;

    private Vector2 rotation;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        SetLockMouse(true);
        rb = GetComponent<Rigidbody>();

        healthbar.SetMaxHealth(maxHealth);
        healthbar.SetHealth(currentHealth);

        // see InGameHUDController
        FindObjectOfType<BuildHUDController>().SetMainPlayer(this);

        // Load sounds:
        soundScript.LoadSounds(jumpSounds);
        soundScript.LoadSounds(takeDamageSounds);
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
                soundScript.PlayRandomSound(jumpSounds);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) && pauseMenu != null)
        {
            SetLockMouse(false);
            pauseMenu.SetActive(true);
        }
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bullet = Instantiate(bulletPrefab, playerCam.transform.position, Quaternion.identity);
            bullet.transform.position = playerCam.transform.position + playerCam.transform.forward;
            bullet.transform.forward = bullet.transform.forward;
        }
    }

    public void RunningSound()
    {
        if (!walk.isPlaying)
        {
            walk.Play();
        }
    }

    // locks or unlocks the cursor
    private void SetLockMouse(bool lockMouse)
    {
        if (lockMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        soundScript.PlayRandomSound(takeDamageSounds);
        if (currentHealth < 0)
        {
            // trigger game over
            currentHealth = 0;
            gameOverScreen.SetActive(true);
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        healthbar.SetHealth(currentHealth);

    }
}
