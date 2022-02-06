using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField, Tooltip("The object that this camera follow")]
    private Transform _followTarget = null;
    public Transform FollowTarget {
        get => _followTarget;
        set {
            _followTarget = value;
            _shouldFollow = value != null;
        }
    }
    private bool _shouldFollow = false;

    [Tooltip("The maximum height (in y coordinates) this camera is allowed to go while following an object")]
    public float MaximumHeight;

    [Tooltip("The minimum height (in y coordinates) this camera is allowed to go while following an object")]
    public float MinimumHeight;

    public float EaseSpeed;

    private void Start()
    {
        _shouldFollow = _followTarget != null;
    }

    private void LateUpdate()
    {
        if(!_shouldFollow)
        {
            return;
        }

        // Ignore z delta when calculating distance to target
        var targetPosition = _followTarget.position;
        targetPosition.z = transform.position.z;

        var calculatedPosition = Vector3.Lerp(transform.position, targetPosition, EaseSpeed * Time.deltaTime);
        calculatedPosition.y = Mathf.Clamp(calculatedPosition.y, MinimumHeight, MaximumHeight);

        transform.position = calculatedPosition;
    }
}
