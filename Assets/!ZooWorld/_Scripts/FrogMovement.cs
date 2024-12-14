using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FrogMovement : AnimalMovementBehaviour
{
    [SerializeField] private float _jumpIntervalTime;
    [SerializeField] private float _jumpDistance;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _jumpDuration;
    [SerializeField] private float _randomDirectionChangeAngle = 90f;
    [SerializeField] private float screenMargin = 0.1f; // Margin percentage (e.g., 0.1 = 10% of the screen)

    private float _timer;
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    protected override void Move()
    {
        _timer += Time.deltaTime;

        if (_timer >= _jumpIntervalTime)
        {
            ChangeDirectionIfNeeded();
            Jump(_jumpHeight);
            _timer = 0;
        }
    }

    public override void BounceAway()
    {
        Debug.Log("Bounce");
        transform.DOKill();
        TurnAround();
        Jump(0);
    }

    void Jump(float jumpHeight)
    {
        Vector3 jumpPos = transform.position + transform.forward * _jumpDistance;
        jumpPos.y = 0;
        transform.DOJump(jumpPos, jumpHeight, 1, _jumpDuration);
    }

    void TurnAround()
    {
        float currentYRotation = transform.eulerAngles.y;
        float oppositeYRotation = (currentYRotation + 180f) % 360f;
        transform.rotation = Quaternion.Euler(0, oppositeYRotation, 0);
    }

    void ChangeDirectionIfNeeded()
    {
        Vector3 predictedPosition = transform.position + transform.forward * _jumpDistance;

        if (IsOutOfScreenBounds(predictedPosition))
        {
            RotateTowardsCenter();
        }
        else
        {
            float randomAngle = Random.Range(-_randomDirectionChangeAngle, _randomDirectionChangeAngle);
            transform.Rotate(0, randomAngle, 0);
        }
    }

    bool IsOutOfScreenBounds(Vector3 position)
    {
        Vector3 viewportPosition = _mainCamera.WorldToViewportPoint(position);
        return viewportPosition.x < screenMargin || viewportPosition.x > 1 - screenMargin ||
               viewportPosition.y < screenMargin || viewportPosition.y > 1 - screenMargin;
    }

    void RotateTowardsCenter()
    {
        Vector3 screenCenter = _mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, _mainCamera.nearClipPlane + 10f));
        screenCenter.y = transform.position.y; // Keep the same height
        Vector3 directionToCenter = (screenCenter - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(directionToCenter);
        transform.rotation = targetRotation; // Instantly align towards the center
    }
    
}
