using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Details")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.18f * 3;

    public HitBox hitBox;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isMoving;

    [Header("Other")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("-----Animation-----")]
    [HideInInspector] public Weapon wp;
    private Animator anim;
    private bool isReloading;
    private bool flag;

    [Header("Joystick Controls")]
    [SerializeField] private FixedJoystick joystick; // Joystick để di chuyển

    private bool check;
    private float footstepCooldown = 1f; // Thời gian giữa các lần phát âm thanh bước chân
    private float nextFootstepTime = 0f;  // Thời gian cho lần phát tiếp theo

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        wp = GetComponentInChildren<Weapon>();
        anim = GetComponentInChildren<Animator>();

        if (joystick == null)
        {
            joystick = UIManager.Ins.mainCanvas.GetJoyStick();
        }

        UIManager.Ins.mainCanvas.GetReloadButton().onClick.AddListener(() => StartReload());

    }

    void Update()
    {
        if (wp == null || controller == null || anim == null) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        HandleMovement();
        HandleGravity();
        HandleMultiTouch();
        CheckAnim(isMoving);
        AutoReload();
        CheckDie();
    }

    private void HandleMultiTouch()
    {
        if (Input.touchCount > 1) // Kiểm tra nếu có nhiều ngón tay trên màn hình
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("Touch ID: " + touch.fingerId + " tại " + touch.position);
                }
            }
        }
    }

    private void CheckDie()
    {
        if (transform.position.y <= -60f)
        {
            hitBox.TakeDamage(100);
        }
    }

    private void HandleMovement()
    {
        float moveX = joystick.Horizontal;
        float moveZ = joystick.Vertical;

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ);
        moveDirection = transform.TransformDirection(moveDirection);

        isMoving = moveDirection.magnitude > 0.1f;
        controller.Move(moveDirection * speed * Time.deltaTime);

        if (isMoving && isGrounded && Time.time >= nextFootstepTime)
        {
            // Giả sử AudioManager.Ins.playerWalk là mảng AudioClip
            int num = Random.Range(0, AudioManager.Ins.playerWalk.Length);
            AudioManager.Ins.PlayPlayerClip(AudioManager.Ins.playerWalk[num]);

            nextFootstepTime = Time.time + footstepCooldown;
        }
    }

    private void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void AutoReload()
    {
        if (wp.NeedReload() && !isReloading)
        {
            isReloading = true;

            // Hủy mọi sự kiện Invoke trước đó để tránh bị lặp âm thanh
            CancelInvoke(nameof(ReloadSound));

            // Bắt đầu animation reload
            anim.Play(CacheString.TAG_RELOAD);
            Invoke(nameof(ReloadSound), 1.1f);  // Gọi âm thanh reload lần 1
            Invoke(nameof(ReloadSound), 2.2f); // Gọi âm thanh reload lần 2

            wp.Reload();
        }
    }

    private bool StartReload()
    {
        UIManager.Ins.mainCanvas.IsReloading(true);
        wp.isReloading = true;
        return check = true;
    }

    private void CheckAnim(bool isMoving)
    {
        if (wp.CheckIsShooting())
        {
            isReloading = false;
            return;
        }

        // Kiểm tra nếu đang reload
        if (isReloading)
        {
            // Nếu animation reload kết thúc, reset isReloading
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName(CacheString.TAG_RELOAD))
            {
                check = false;
                isReloading = false;
                Debug.Log("C");
                UIManager.Ins.mainCanvas.IsReloading(false);
                wp.ReloadComplete();
            }
            else
            {
                return;
            }
        }

        if (check && !isReloading)
        {
            isReloading = true;

            // Hủy mọi sự kiện Invoke trước đó để tránh bị lặp âm thanh
            CancelInvoke(nameof(ReloadSound));

            // Bắt đầu animation reload
            anim.Play(CacheString.TAG_RELOAD);
            Invoke(nameof(ReloadSound), 1.2f);  // Gọi âm thanh reload lần 1
            Invoke(nameof(ReloadSound), 2.2f); // Gọi âm thanh reload lần 2

            wp.Reload();
            return;
        }


        // Kiểm tra di chuyển
        if (isMoving)
        {
            anim.Play(CacheString.TAG_MOVE);
            speed = 5f;
            flag = true;
        }
        else
        {
            if (!flag) return;
            anim.Play(CacheString.TAG_IDLE);
        }
    }

    // Animation Event - Gọi âm thanh reload
    public void ReloadSound()
    {
        AudioManager.Ins.PlaySFX(AudioManager.Ins.reload);
    }
}
