using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    [Header("Camera")]
    public Camera playerCamera;

    [Header ("Speed Controll")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 20f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    [Header ("Animator")]
    public Animator animator;
    private bool canMove = true;

    [Header ("Life Bars")]
    public Image[] lifeBars;
    public int life = 3;
    private int maxLife = 3;

    [Header ("Coin")]
    public int totalCoins = 10;
    private int collectedCoins = 0;
    public TextMeshProUGUI coinDisplay;

    [Header ("Effects")]
    //public GameObject EnemyDeathEffect;
    public GameObject PlayerHealthEffect;
    public GameObject PlayerHealerCircl;
    public GameObject PlayerHealerEffect;
    public GameObject BigCoin;
    public GameObject CoinTExt;

    [Header ("Audio")]
    [SerializeField] AudioSource WRSound;
    [SerializeField] AudioSource AllSound;
    [SerializeField] AudioSource EnemyAttackSource;
    public AudioClip Walking;
    public AudioClip Running;
    public AudioClip Healing;
    public AudioClip Coin;
    public AudioClip EnemyAttact;
    public AudioClip Door;

    [Header ("Next Room")]
    public string nextSceneName = "NextScene";

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        animator.SetBool("Idle", true);
        animator.SetBool("Sprint", false);
        animator.SetBool("Walking", false);

        UpdateLifeBarUI();
        UpdateCoinDisplay();

        //EnemyDeathEffect.SetActive(false);
        PlayerHealthEffect.SetActive(false);

        PlayerHealerCircl.SetActive(false);
        PlayerHealerEffect.SetActive(false);
        CoinTExt.SetActive(false);

        BigCoin.SetActive(false);
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = 0;
        if (canMove)
        {
            if (isRunning)
            {
                curSpeedX = runSpeed * Input.GetAxis("Vertical");
            }
            else
            {
                curSpeedX = walkSpeed * Input.GetAxis("Vertical");
            }
        }
        if (life <= 0)
        {
            Debug.Log("Player is dead!");
        }
        float curSpeedY = 0;
        if (canMove)
        {
            if (isRunning)
            {
                curSpeedY = runSpeed * Input.GetAxis("Horizontal");
            }
            else
            {
                curSpeedY = walkSpeed * Input.GetAxis("Horizontal");
            }
        }

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        bool isMoving = curSpeedX != 0 || curSpeedY != 0;
        if (isMoving)
        {
            animator.SetBool("Idle", false);
            if (isRunning)
            {
                animator.SetBool("Sprint", true);
                animator.SetBool("Walking", false);
                if (WRSound.clip != Running || !WRSound.isPlaying)
                {
                    WRSound.clip = Running;
                    WRSound.Play();
                }
            }
            else
            {
                animator.SetBool("Sprint", false);
                animator.SetBool("Walking", true);
                if (WRSound.clip != Walking || !WRSound.isPlaying)
                {
                    WRSound.clip = Walking;
                    WRSound.Play();
                }
            }
        }
        else
        {
            animator.SetBool("Idle", true);
            animator.SetBool("Walking", false);
            animator.SetBool("Sprint", false);
            if (WRSound.isPlaying)
            {
                WRSound.Stop();
            }
        }

        float movementDirectionY = moveDirection.y;

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
            animator.SetBool("Jump", true);
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
            animator.SetBool("Jump", false);
        }

        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            life--;

            UpdateLifeBarUI();

            Debug.Log("Player hit by an enemy! Life remaining: " + life);
            StartCoroutine(EnemyDeath(other));

            if (life <= 0)
            {
                StartCoroutine(QuitGame());
            }
        }
        if (other.CompareTag("Healer"))
        {
            HealPlayer();
            Debug.Log("Player healed! Life restored to maximum.");

            other.gameObject.SetActive(false);
            
        }
        if (other.CompareTag("Coin"))
        {
            collectedCoins++;
            UpdateCoinDisplay();

            other.gameObject.SetActive(false);

            AllSound.clip = Coin;
            AllSound.Play();

            Debug.Log("Collected a coin! Total coins: " + collectedCoins);
            if (collectedCoins >= totalCoins)
            {
                Debug.Log("All coins collected! You win!");
            }
        }
        if (other.CompareTag("BigCoin"))
        {
            collectedCoins+=10;
            UpdateCoinDisplay();
            other.gameObject.SetActive(false);
            BigCoin.SetActive(true);
            Debug.Log("Collected a coin! Total coins: " + collectedCoins);
        }
        if (other.CompareTag("Door"))
        {
            if (collectedCoins >= totalCoins)
            {
                Debug.Log("All coins collected! Moving to the next scene.");
                MoveToNextScene();
            }
            else
            {
                Debug.Log("You need to collect all the coins to unlock the door.");
                StartCoroutine(DisplayCoinText());
            }
        }
        if (other.CompareTag("Door First"))
        {
            if (collectedCoins >= totalCoins)
            {
                AllSound.clip = Door;
                AllSound.Play();
            }
        }
    }
    void UpdateLifeBarUI()
    {
        for (int i = 0; i < lifeBars.Length; i++)
        {
            if (i < life)
            {
                lifeBars[i].enabled = true;

            }
            else
            {
                lifeBars[i].enabled = false;
            }
        }
    }
    void HealPlayer()
    {
        StartCoroutine(PlayerHealth());
        
    }
    IEnumerator EnemyDeath(Collider enemy)
    {
        yield return new WaitForSeconds(0.5f);
        PlayerHealthEffect.SetActive(true);
        EnemyAttackSource.clip = EnemyAttact;
        EnemyAttackSource.Play();
        yield return new WaitForSeconds(1.0f);
        enemy.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        PlayerHealthEffect.SetActive(false);
        EnemyAttackSource.clip = EnemyAttact;
        EnemyAttackSource.Stop();
    }
    IEnumerator PlayerHealth()
    {
        yield return new WaitForSeconds(0.7f);
        PlayerHealerCircl.SetActive(true);
        PlayerHealerEffect.SetActive(true);
        life = maxLife;
        AllSound.clip = Healing;
        AllSound.Play();
        UpdateLifeBarUI();
        yield return new WaitForSeconds(1.5f);
        PlayerHealerCircl.SetActive(false);
        PlayerHealerEffect.SetActive(false);
        AllSound.clip = Healing;
        AllSound.Stop();
    }

    IEnumerator DisplayCoinText()
    {
        CoinTExt.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        CoinTExt.SetActive(false);
    }
    void UpdateCoinDisplay()
    {
        coinDisplay.text = "" + collectedCoins + " / " + totalCoins;
    }
    void MoveToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
    IEnumerator QuitGame()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Dungeon");
    }
}
