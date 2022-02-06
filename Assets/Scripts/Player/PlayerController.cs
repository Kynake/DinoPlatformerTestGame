using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase, RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float JumpHeight;
    public LayerMask groundMask;

    public bool isGrounded { get; private set; } = false;

    private Rigidbody2D _rb;
    private float _lateralMoveForce = 0;
    private bool _shouldJump = false;

    private WaitForFixedUpdate _fixedUpdateWait = new WaitForFixedUpdate();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        InputController.OnMove += Move;
        InputController.OnJump += Jump;
    }

    private void OnDisable()
    {
        InputController.OnMove -= Move;
        InputController.OnJump -= Jump;
    }

    private void Update()
    {
        UpdateMovement();
    }

    private void LateUpdate()
    {
        if(isGrounded)
        {
            // Start the clearGrounded coroutine,
            // which attempts to reset the grounded state for the next frame
            StartCoroutine(nameof(ClearGrounded));
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        DetectGround(other);
    }

    public void Move(Vector2 direction)
    {
        _lateralMoveForce = Vector2.Dot(direction, Vector2.right);
    }

    public void Jump()
    {
        _shouldJump = true;
    }

    private void UpdateMovement()
    {
        var targetLateralSpeed = _lateralMoveForce * Speed;
        var targetVerticalSpeed = _rb.velocity.y;

        if(_shouldJump && isGrounded)
        {
            targetVerticalSpeed = Mathf.Sqrt(2 * Physics.gravity.magnitude * JumpHeight);
        }
        _shouldJump = false;

        _rb.velocity = new Vector2(targetLateralSpeed, targetVerticalSpeed);
        if(_lateralMoveForce != 0)
        {
            transform.localScale = new Vector3(_lateralMoveForce > 0? 1 : -1, 1, 1);
        }
    }

    private void DetectGround(Collision2D other)
    {
        if((groundMask & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        for(int i = 0; i < other.contactCount; i++)
        {
            var contact = other.GetContact(i);

            if(!IsFloorCollision(contact))
            {
                continue;
            }

            // Stop coroutine from previous frame from taking effect
            StopCoroutine(nameof(ClearGrounded));
            isGrounded = true;
            return;
        }
    }

    // Since this game doesn't have any slopes
    // testing if a collision is ground is very simple
    private bool IsFloorCollision(ContactPoint2D contact)
    {
        return contact.normal == Vector2.up;
    }

    private IEnumerator ClearGrounded()
    {
        yield return _fixedUpdateWait;

        isGrounded = false;
    }
}
