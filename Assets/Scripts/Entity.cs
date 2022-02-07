using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator))]
public class Entity : MonoBehaviour
{
    protected static readonly int ANIM_IS_FALLING_STATE = Animator.StringToHash("Is_Falling");

    public LayerMask GroundMask;
    public LayerMask KillTriggerMask;

    public bool IsGrounded { get; private set; } = true;

    protected Rigidbody2D _rb;
    protected Animator _anim;

    private bool _hasGoneToSleep = false;
    protected WaitForFixedUpdate _fixedUpdateWait = new WaitForFixedUpdate();

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        if(_rb.IsAwake() && IsGrounded)
        {
            // Start the clearGrounded coroutine,
            // which attempts to reset the grounded state for the next frame
            StartCoroutine(nameof(ClearGrounded));
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D other)
    {
        DetectFloor(other);
    }

    protected virtual void DetectFloor(Collision2D other)
    {
        if((GroundMask & (1 << other.gameObject.layer)) == 0)
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
            IsGrounded = true;
            _anim.SetBool(ANIM_IS_FALLING_STATE, false);
            return;
        }
    }

    // Since this game doesn't have any slopes
    // testing if a collision is ground is very simple
    protected virtual bool IsFloorCollision(ContactPoint2D contact)
    {
        return Mathf.Approximately(contact.normal.y, 1);
    }

    private IEnumerator ClearGrounded()
    {
        yield return _fixedUpdateWait;

        IsGrounded = false;
        _anim.SetBool(ANIM_IS_FALLING_STATE, true);
    }

    protected bool IsKillTrigger(Collider2D other)
    {
        return (KillTriggerMask & (1 << other.gameObject.layer)) != 0;
    }
}
