using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public bool doubleJump = false;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator pAni;
    private bool isGrounded;
    private float moveInput;

    //private bool isSpeed = false;
    private bool isKickboard = false;

    [SerializeField] private UnityEngine.UI.Image[] hearts;
    private int hp = 3;
    public static int currentHp = 3;

    private bool isInvincible = false;
    private bool isShield = false;

    private SpriteRenderer sr;

    [SerializeField] private GameObject shieldObject;

    AudioManager audioManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        hp = currentHp;
        UpdateHearts();
    }
    private void Update()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput < 0)
            transform.localScale = new Vector3(-0.7f, 0.7f, 0.7f);
        else if (moveInput > 0)
            transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        pAni.SetBool("isGrounded", isGrounded);
        pAni.SetBool("isKickboard", isKickboard);
        pAni.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));

    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
        //pAni.SetTrigger("isRun");
    }

    public void OnJump(InputValue value)
    {
        if (moveSpeed >= 7f) return;

        if (value.isPressed)
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                pAni.SetTrigger("Jump");
            }
            else if (!isGrounded && doubleJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                pAni.SetTrigger("Jump");
                
                doubleJump = false;
            }
        }
    }

    void ResetInvincible()
    {
        isInvincible = false;
        isShield = false;
        shieldObject.SetActive(false);
    }
    void EndShield()
    {
        isInvincible = false;
        isShield = false;
        shieldObject.SetActive(false);
    }
    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Die()
    {
        PlayerController.currentHp = 3;

        if (audioManager != null && audioManager.death != null)
        {
            audioManager.PlaySFX(audioManager.death);
        }

        pAni.SetTrigger("Die");

        rb.linearVelocity = Vector2.zero;
        this.enabled = false;

        Invoke(nameof(RestartScene), 1.5f);
    }

    public void TakeDamage()
    {
        if (isInvincible) return;

        isInvincible = true;

        hp--;
        currentHp = hp;

        UpdateHearts();

        if (hp <= 0)
        {
            Die();
            return;
        }

        Invoke(nameof(ResetInvincible), 2f);
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < hp;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            audioManager.PlaySFX(audioManager.death);
            Die();
        }

        if (collision.CompareTag("Enemy"))
        {
            if (isInvincible) return;

            audioManager.PlaySFX(audioManager.enemy);

            if (isShield)
            {
                isInvincible = true;
                Invoke(nameof(EndShield), 3f);
                return;
            }

            TakeDamage();
        }
    

        if (collision.CompareTag("Finish"))
        {
 
            collision.GetComponent<LevelObject>().MoveToNextLevel();
        }


        if (collision.CompareTag("hpItem"))
        {
            audioManager.PlaySFX(audioManager.item);
            if (hp < 3)
            {
                hp++;
                currentHp = hp;
                UpdateHearts(); 
            }

            Destroy(collision.gameObject);
        }
        
        if (collision.CompareTag("speedItem"))
        {
            audioManager.PlaySFX(audioManager.item);
            moveSpeed += 3f;
            isKickboard = true;
            Destroy(collision.gameObject);
            Invoke(nameof(ResetSpeed), 5f);
        }

        if (collision.CompareTag("shieldItem"))
        {
            audioManager.PlaySFX(audioManager.item);
            isShield = true;
            shieldObject.SetActive(true);
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("jumpItem"))
        {
            audioManager.PlaySFX(audioManager.item);
            doubleJump = true;
            Destroy(collision.gameObject);
        }

    }
        void ResetSpeed()
        {
            moveSpeed -= 3f;
            isKickboard = false;
        }

        void RestShield()
        {
            isShield = false;
        }

}
