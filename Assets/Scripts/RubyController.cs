using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;

    public GameObject projectilePrefab;

    public AudioClip throwSound;
    public AudioClip hitSound;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    public GameObject healthIncreaseParticlePrefab;
    public GameObject healthDecreaseParticlePrefab;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;
    public int score = 0;
    public TMP_Text scoreUI;
    public GameObject winUI;
    public GameObject loseUI;
    public bool gameOver = false;
    public GameObject[] enemyObjects;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
        enemyObjects = GameObject.FindGameObjectsWithTag("EnemyController");
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }
        if (score >= enemyObjects.Length)
        {
            WinGame();
        }
        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }
        if (gameOver)
        {
            rigidbody2d.constraints = RigidbodyConstraints2D.FreezePosition;
            if (Input.GetKey(KeyCode.R))

            {

                if (gameOver == true)

                {

                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene

                }

            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            Instantiate(healthDecreaseParticlePrefab, transform.position, Quaternion.identity);

            PlaySound(hitSound);
        }
        else if (amount > 0)
        {
            Instantiate(healthIncreaseParticlePrefab, transform.position, Quaternion.identity);
        }
        

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        if (currentHealth <= 0)
        {
            LoseGame();
        }
    }
     public void ChangeSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public void LoseGame()
    {
        loseUI.SetActive(true);
        gameOver = true;
    }
    public void WinGame()
    {
        winUI.SetActive(true);
        gameOver = true;
    }
    public void ChangeScore(int amount)
    {
        score += amount;

        scoreUI.text = score.ToString();
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}