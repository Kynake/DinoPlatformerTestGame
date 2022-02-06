using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase, RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private static readonly int ANIM_IS_MOVING_STATE      = Animator.StringToHash("Is_Moving");
    private static readonly int ANIM_MOVEMENT_SPEED_STATE = Animator.StringToHash("Movement_Speed");
    private static readonly int ANIM_JUMP_STATE           = Animator.StringToHash("Jump");
    private static readonly int ANIM_IS_FALLING_STATE     = Animator.StringToHash("Is_Falling");

    public float Speed;
    public float JumpHeight;
    public LayerMask groundMask;

    public bool isMoving { get; private set; } = false;
    public bool isGrounded { get; private set; } = true;

    private Rigidbody2D _rb;
    private Animator _anim;
    private float _lateralMoveForce = 0;
    private bool _shouldJump = false;

    private WaitForFixedUpdate _fixedUpdateWait = new WaitForFixedUpdate();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
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

    public void SetFallingAnimationState() => _anim.SetBool(ANIM_IS_FALLING_STATE, true);
    public void ClearFallingAnimationState() => _anim.SetBool(ANIM_IS_FALLING_STATE, false);

    private void UpdateMovement()
    {
        var targetLateralSpeed = _lateralMoveForce * Speed;
        var targetVerticalSpeed = _rb.velocity.y;

        if(_shouldJump && isGrounded)
        {
            targetVerticalSpeed = Mathf.Sqrt(2 * Physics2D.gravity.magnitude * JumpHeight);
            _anim.SetTrigger(ANIM_JUMP_STATE);
        }
        _shouldJump = false;

        _rb.velocity = new Vector2(targetLateralSpeed, targetVerticalSpeed);
        _anim.SetFloat(ANIM_MOVEMENT_SPEED_STATE, Mathf.Abs(targetLateralSpeed));
        if(_lateralMoveForce != 0)
        {
            if(!isMoving)
            {
                isMoving = true;
                _anim.SetBool(ANIM_IS_MOVING_STATE, true);
            }

            transform.localScale = new Vector3(_lateralMoveForce > 0? 1 : -1, 1, 1);
        }
        else
        {
            isMoving = false;
            _anim.SetBool(ANIM_IS_MOVING_STATE, false);
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
            _anim.SetBool(ANIM_IS_FALLING_STATE, false);
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
        _anim.SetBool(ANIM_IS_FALLING_STATE, true);
    }
}
