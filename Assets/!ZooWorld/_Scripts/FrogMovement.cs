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
    [SerializeField] private float _randomDirectionChangeAngle = 90f; // Max angle for random direction change
    private float _timer;

    protected override void Move()
    {
        _timer += Time.deltaTime;

        if (_timer >= _jumpIntervalTime)
        {
            ChangeDirection();
            Jump();
            _timer = 0;
        }
    }

    void Jump()
    {
        Vector3 jumpPos = transform.position + transform.forward * _jumpDistance;
        transform.DOJump(jumpPos, _jumpHeight, 1, _jumpDuration);
    }

    void ChangeDirection()
    {
        float randomAngle = Random.Range(-_randomDirectionChangeAngle, _randomDirectionChangeAngle);
        transform.Rotate(0, randomAngle, 0);
    }
}