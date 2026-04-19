using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator pAni;
    private bool isGrounded;
    private float moveInput;

    //private bool isSpeed = false;
    private bool isKickboard = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();
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

        if (value.isPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            pAni.SetTrigger("Jump");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (collision.CompareTag("Enemy"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (collision.CompareTag("Finish"))
        {
            collision.GetComponent<LevelObject>().MoveToNextLevel();
        }
        

        if(collision.CompareTag("speedItem"))
        {
            moveSpeed += 3f;
            isKickboard = true;
            Destroy(collision.gameObject);
            Invoke(nameof(ResetSpeed), 5f);
        }

    }
        void ResetSpeed()
        {
            moveSpeed -= 3f;
            isKickboard = false;
        }

}
