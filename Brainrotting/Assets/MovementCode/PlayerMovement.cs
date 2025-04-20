using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed       = 10f;
    public float laneDistance       =  3f;
    public float laneSwitchSpeed    = 10f;

    [Header("Jump & Fall Control")]
    public float jumpForce          =  7f;
    public float fallMultiplier     =  4f;
    public float lowJumpMultiplier  =  2f;

    private int     currentLane = 1;
    private Vector2 touchStart;
    private bool    isGrounded  = true;

    Rigidbody rb;
    Animator  anim;

    void Awake()
    {
        rb   = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        rb.useGravity             = true;
        rb.drag                   = 0f;
        rb.interpolation          = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        HandleSwipeInput();
    }

    void FixedUpdate()
    {
        MoveForward();
        SwitchLanePhysics();
        ApplyEnhancedGravity();
    }

    void MoveForward()
    {
        Vector3 step = transform.forward * forwardSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + step);
    }

    void HandleSwipeInput()
    {
    #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            touchStart = (Vector2)Input.mousePosition;

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 mouseEnd = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 swipe    = mouseEnd - touchStart;
            if (swipe.magnitude > 50f)
                ProcessSwipe(swipe);
        }
    #else
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
                touchStart = t.position;
            if (t.phase == TouchPhase.Ended)
            {
                Vector2 swipe = t.position - touchStart;
                if (swipe.magnitude > 50f)
                    ProcessSwipe(swipe);
            }
        }
    #endif
    }

    void ProcessSwipe(Vector2 swipe)
    {
        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        {
            if (swipe.x > 0 && currentLane < 2) currentLane++;
            else if (swipe.x < 0 && currentLane > 0) currentLane--;
        }
        else
        {
            if (swipe.y > 0 && isGrounded) Jump();
            else if (swipe.y < 0)            Roll();
        }
    }

    void SwitchLanePhysics()
    {
        float targetX = (currentLane - 1) * laneDistance;
        Vector3 pos   = rb.position;
        pos.x         = Mathf.Lerp(pos.x, targetX, laneSwitchSpeed * Time.fixedDeltaTime);
        rb.MovePosition(pos);
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        anim.SetTrigger("Jump");
        isGrounded = false;
    }

    void Roll()
    {
        anim.SetTrigger("Roll");
        if (!isGrounded)
            rb.AddForce(Vector3.down * 50f, ForceMode.VelocityChange);
    }

    void ApplyEnhancedGravity()
    {
        Vector3 v = rb.velocity;
        if (v.y < 0)
            rb.AddForce(Vector3.up * Physics.gravity.y * (fallMultiplier - 1), ForceMode.Acceleration);
        else if (v.y > 0 && !Input.GetMouseButton(0))
            rb.AddForce(Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1), ForceMode.Acceleration);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }
}
