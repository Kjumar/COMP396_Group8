using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerController : NetworkBehaviour, IDamageable
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

    [Header("Shooting")]
    [SerializeField] Transform handCannon;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed;
    private float attackSpeed = 0.5f;
    private float attackCooldown = 0;

    [Header("Misc")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject[] towerPrefabs; // we need a direct reference to each prefab to spawn them over the network
    [SerializeField] NetworkBankController bank;
    [SerializeField] Material ghostMat;

    private Vector2 rotation;
    private Rigidbody rb;

    private NetworkBuildHUD buildHud;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        firstPersonCamera.gameObject.SetActive(true);
        GameObject.Find("MainCamera").SetActive(false);

        SetLockMouse(true);
        rb = GetComponent<Rigidbody>();

        GameObject playerHB = GameObject.Find("PlayerHealthBar");
        if (playerHB != null)
        {
            healthbar = playerHB.GetComponent<UIHealthBar>();
            healthbar.SetMaxHealth(maxHealth);
            healthbar.SetHealth(currentHealth);
        }
        else Debug.Log("Could not find 'PlayerHealthBar' in scene");

        // see InGameHUDController
        buildHud = FindObjectOfType<NetworkBuildHUD>();
        if (buildHud != null)
        {
            buildHud.SetMainPlayer(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // firstPersonCamera.gameObject.GetComponent<Camera>().enabled = false;
        if (spawnPoint != null)
        {
            spawnPoint = transform;
        }

        bank = FindObjectOfType<NetworkBankController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.isLocalPlayer)
        {
            return;
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (currentHealth <= 0)
            {
                HandleGhostMovement();
                return;
            }
            else
            {
                // when mouse is locked, read mouse movement and rotate the player and camera
                Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;
                rotation += mouseMovement;

                // limit rotation so the player doesn't flip on their head
                rotation.y = Mathf.Clamp(rotation.y, -80, 80);

                // apply rotation
                firstPersonCamera.localRotation = Quaternion.Euler(rotation.y, 0, 0);
                transform.localRotation = Quaternion.Euler(0, rotation.x, 0);
                handCannon.localRotation = Quaternion.Euler(rotation.y, 0, 0);

                // manual player movement
                // input manager axis were glitching out, so I'm manually checking each button
                float horizontal = 0;
                float vertical = 0;
                if (Input.GetKey(KeyCode.W)) vertical = 1;
                else if (Input.GetKey(KeyCode.S)) vertical = -1;
                if (Input.GetKey(KeyCode.A)) horizontal = -1;
                else if (Input.GetKey(KeyCode.D)) horizontal = 1;

                Vector2 playerMove = new Vector2(horizontal, vertical).normalized * speed * Time.deltaTime;
                transform.position = transform.position
                    + (transform.right * playerMove.x)
                    + (transform.forward * playerMove.y);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // jump
                    if (Physics.CheckSphere(groundCheckPos.position, groundCheckRadius, groundCheckMask))
                    {
                        rb.AddForce(new Vector3(0, jumpForce, 0));
                    }
                }

                if (attackCooldown > 0)
                {
                    attackCooldown -= Time.deltaTime;
                }
                else if (!buildHud.buildMode && Input.GetMouseButtonDown(0))
                {
                    CmdFire();
                    attackCooldown = attackSpeed;
                }
            }
        }
    }

    private void HandleGhostMovement()
    {
        // when mouse is locked, read mouse movement and rotate the player and camera
        Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;
        rotation += mouseMovement;

        // limit rotation so the player doesn't flip on their head
        rotation.y = Mathf.Clamp(rotation.y, -80, 80);

        // apply rotation
        firstPersonCamera.localRotation = Quaternion.Euler(rotation.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, rotation.x, 0);

        float vertical = 0;
        if (Input.GetKey(KeyCode.Space))
        {
            vertical = 4 * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            vertical = 4 * -Time.deltaTime;
        }

        // manual player movement
        Vector2 playerMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * speed * Time.deltaTime;
        transform.position = transform.position
            + (transform.right * playerMove.x)
            + (transform.forward * playerMove.y)
            + (transform.up * vertical);

        if (transform.position.y < 5f)
        {
            transform.position = new Vector3(transform.position.x, 5f, transform.position.z);
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
        // let the client handle deathplane collision
        if (!this.isLocalPlayer)
        {
            return;
        }

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
        if (currentHealth < 0)
        {

            GetComponent<CapsuleCollider>().enabled = false;
            rb.isKinematic = true;
            rb.useGravity = false;
            if (ghostMat != null) GetComponent<MeshRenderer>().material = ghostMat;

            // trigger game over
            currentHealth = 0;
            //gameOverScreen.SetActive(true);
            //Time.timeScale = 0f;

            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;

            CmdSetHealth(currentHealth);
        }
        if (healthbar != null)
        {
            healthbar.SetHealth(currentHealth);
        }
    }

    [Command]
    public void CmdSetHealth(int health)
    {
        // sync health with the server side
        currentHealth = health;
        GetComponent<CapsuleCollider>().enabled = false;
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    [Command]
    public void CmdChangeMat()
    {
        if (ghostMat != null) GetComponent<MeshRenderer>().material = ghostMat;
    }

    [Command]
    public void CmdSpawnTower(int selectedTower, GameObject buildNode, int cost)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        GameObject newTower = Instantiate(towerPrefabs[selectedTower], buildNode.transform.position, Quaternion.identity);
        bank.Pay(cost);

        NetworkServer.Spawn(newTower);
        NetworkServer.Destroy(buildNode);
    }

    [Command]
    public void CmdFire()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;

        NetworkServer.Spawn(bullet);
    }
}
