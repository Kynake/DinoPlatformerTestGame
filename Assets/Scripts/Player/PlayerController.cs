using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase, RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float Speed;

    private Rigidbody2D _rb;
    private float _lateralMoveForce = 0;

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

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    public void Move(Vector2 direction)
    {
        _lateralMoveForce = Vector2.Dot(direction, Vector2.right);
    }

    public void Jump()
    {

    }

    private void UpdateMovement()
    {
        _rb.velocity = new Vector2(_lateralMoveForce * Speed, _rb.velocity.y);
        if(_lateralMoveForce != 0)
        {
            transform.localScale = new Vector3(_lateralMoveForce > 0? 1 : -1, 1, 1);
        }
    }
}
