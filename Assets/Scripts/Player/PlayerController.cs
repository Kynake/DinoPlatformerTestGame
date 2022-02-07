using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class PlayerController : Entity
{
    private static readonly int ANIM_JUMP_STATE = Animator.StringToHash("Jump");
    private static readonly int ANIM_DIE_STATE = Animator.StringToHash("Die");

    public float Speed;
    public float JumpHeight;

    [HideInInspector]
    public bool ControlsDisabled = false;

    public LayerMask EnemyMask;
    private int _enemyLayer = -1;

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
        var targetLateralSpeed = ControlsDisabled? 0 : _lateralMoveForce * Speed;
        var targetVerticalSpeed = _rb.velocity.y;

        if(_shouldJump && IsGrounded && !ControlsDisabled)
        {
            targetVerticalSpeed = Mathf.Sqrt(2 * Physics2D.gravity.magnitude * JumpHeight);
            _anim.SetTrigger(ANIM_JUMP_STATE);
        }
        _shouldJump = false;

        _rb.velocity = new Vector2(targetLateralSpeed, targetVerticalSpeed);
        _anim.SetFloat(ANIM_MOVEMENT_SPEED_STATE, Mathf.Abs(targetLateralSpeed));
        if(targetLateralSpeed != 0)
        {
            _anim.SetBool(ANIM_IS_MOVING_STATE, true);

            transform.localScale = new Vector3(_lateralMoveForce > 0? 1 : -1, 1, 1);
        }
        else
        {
            _anim.SetBool(ANIM_IS_MOVING_STATE, false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(isEnemyCollision(other))
        {
            // Ignore Collisions between player and enemies while the player is dead
            _enemyLayer = other.gameObject.layer;
            Physics2D.IgnoreLayerCollision(gameObject.layer, _enemyLayer, true);
            TakeHit();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(IsKillTrigger(other))
        {
            Die();
        }
    }

    private bool isEnemyCollision(Collision2D other)
    {
        return (EnemyMask & (1 << other.gameObject.layer)) != 0;
    }

    // When touching an enemy, first we take a hit
    // and play animations before dying
    public void TakeHit()
    {
        ControlsDisabled = true;
        _anim.SetTrigger(ANIM_DIE_STATE);

    }

    // When dying, we emit and event so that
    // we can reset the level, play an ad, etc
    public void Die()
    {
        print("Player DEAD!");

        if(_enemyLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, _enemyLayer, false);
        }

        ResetAnimation();
        ControlsDisabled = false;
    }
}
