using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator))]
public class Entity : MonoBehaviour
{
    protected static readonly int ANIM_IS_MOVING_STATE      = Animator.StringToHash("Is_Moving");
    protected static readonly int ANIM_MOVEMENT_SPEED_STATE = Animator.StringToHash("Movement_Speed");
    protected static readonly int ANIM_IS_FALLING_STATE     = Animator.StringToHash("Is_Falling");
    protected static readonly int ANIM_RESET_STATE          = Animator.StringToHash("Reset_Animation");

    public LayerMask GroundMask;
    public LayerMask KillTriggerMask;

    public bool IsGrounded { get; private set; } = true;

    protected Rigidbody2D _rb;
    protected Animator _anim;

    protected WaitForFixedUpdate _fixedUpdateWait = new WaitForFixedUpdate();

    private const int BUFFER_SIZE = 5;
    private const float UP_NORMAL_ANGLE = 90f;
    private const float GROUND_CAST_DISTANCE = 0.05f;
    private RaycastHit2D[] _groundDetectionBuffer = new RaycastHit2D[BUFFER_SIZE];
    private ContactFilter2D _groundFilter;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();

        _groundFilter = new ContactFilter2D();
        _groundFilter.SetLayerMask(GroundMask);
        _groundFilter.SetNormalAngle(UP_NORMAL_ANGLE - 1, UP_NORMAL_ANGLE + 1);
    }

    private void FixedUpdate()
    {
        DetectGround();
    }

    protected virtual void DetectGround()
    {
        var groundDetected = _rb.Cast(Vector2.down, _groundFilter, _groundDetectionBuffer, GROUND_CAST_DISTANCE) > 0;

        if(IsGrounded)
        {
            IsGrounded = groundDetected;
        }

        // Might now be grounded
        else if(groundDetected && _rb.velocity.y < 0 || Utils.CloseEnough(_rb.velocity.y, 0))
        {
            for(int i = 0; i < _groundDetectionBuffer.Length; i++)
            {
                if(IsFloorCollision(_groundDetectionBuffer[i]))
                {
                    IsGrounded = true;
                    break;
                }
            }
        }

        _anim.SetBool(ANIM_IS_FALLING_STATE, !IsGrounded);
    }

    // Since this game doesn't have any slopes
    // testing if a collision is ground is very simple
    protected virtual bool IsFloorCollision(RaycastHit2D hit)
    {
        return Mathf.Approximately(hit.normal.y, 1);
    }

    public void ResetAnimation()
    {
        _anim.SetTrigger(ANIM_RESET_STATE);
    }

    protected bool IsKillTrigger(Collider2D other) => CompareToMask(KillTriggerMask, other);

    protected bool CompareToMask(LayerMask layerMask, Collider2D other)
    {
        return (layerMask & (1 << other.gameObject.layer)) != 0;
    }
}
