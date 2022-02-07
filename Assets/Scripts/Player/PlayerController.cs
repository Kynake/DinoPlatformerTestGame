using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class PlayerController : Entity
{
    private static readonly int ANIM_IS_MOVING_STATE      = Animator.StringToHash("Is_Moving");
    private static readonly int ANIM_MOVEMENT_SPEED_STATE = Animator.StringToHash("Movement_Speed");
    private static readonly int ANIM_JUMP_STATE           = Animator.StringToHash("Jump");

    public float Speed;
    public float JumpHeight;

    public LayerMask EnemyMask;

    public bool isMoving { get; private set; } = false;

    private float _lateralMoveForce = 0;
    private bool _shouldJump = false;

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

        if(_shouldJump && IsGrounded)
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
}
