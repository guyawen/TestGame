using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f; // 移动速度

    [Header("跳跃设置")]
    public float jumpForce = 10f; // 跳跃力度
    public LayerMask groundLayer; // 地面层

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 获取水平输入（A/D 或 左/右箭头）
        moveInput = Input.GetAxisRaw("Horizontal");

        // 检测是否在地面上
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);

        // 角色跳跃
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
        // 角色移动
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // 角色朝向
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
        }
    }
}

