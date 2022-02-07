using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Enemy : Entity
{
    public float Speed;

    private float _moveDirection = 1;

    public delegate void Death();
    public Death OnDeath;

    protected override void Awake()
    {
        base.Awake();

        OnDeath = Despawn;
    }

    private void OnEnable()
    {
        _moveDirection = transform.localScale.x == 1? 1 : -1;
    }

    private void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        var targetLateralSpeed = IsGrounded? _moveDirection * Speed : 0;
        var targetVerticalSpeed = _rb.velocity.y;

        _rb.velocity = new Vector2(targetLateralSpeed, targetVerticalSpeed);
        _anim.SetFloat(ANIM_MOVEMENT_SPEED_STATE, Mathf.Abs(targetLateralSpeed));

        if(targetLateralSpeed != 0)
        {
            _anim.SetBool(ANIM_IS_MOVING_STATE, true);

            transform.localScale = new Vector3(_moveDirection > 0? 1 : -1, 1, 1);
        }
        else
        {
            _anim.SetBool(ANIM_IS_MOVING_STATE, false);
        }
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(IsKillTrigger(other))
        {
            OnDeath?.Invoke();
        }
    }
}
