using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : AnimalMovementBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float positionLerpSpeed = 10f;
    [SerializeField] private float rotationLerpSpeed = 10f;
    [SerializeField] private float wanderStrength = 0.5f;
    [SerializeField] private float wanderInterval = 2f;

    private float time;
    private Vector3 startPosition;
    private Vector3 movementDirection;
    private Vector3 perpendicularDirection;
    private Camera mainCamera;
    private float wanderTimer;

    void OnEnable()
    {
        startPosition = transform.position;
        movementDirection = transform.forward.normalized;
        perpendicularDirection = new Vector3(-movementDirection.z, 0, movementDirection.x).normalized;

        mainCamera = Camera.main;
    }

    protected override void Move()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderInterval)
        {
            RandomizeDirection();
            wanderTimer = 0f;
        }

        if (IsOutOfCameraBounds())
        {
            RotateTowardsScreenCenter();
        }

        time += Time.deltaTime * speed;
        float waveOffset = Mathf.Sin(time * frequency) * amplitude;
        Vector3 targetPosition = startPosition + movementDirection * time + perpendicularDirection * waveOffset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * positionLerpSpeed);

        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);
        }
    }

    private bool IsOutOfCameraBounds()
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
        return viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1;
    }

    private void RotateTowardsScreenCenter()
    {
        Vector3 screenCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane + 10f));
        screenCenter.y = 0;

        Vector3 directionToCenter = (screenCenter - transform.position).normalized;

        if (directionToCenter.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToCenter);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);
        }

        movementDirection = directionToCenter;
        perpendicularDirection = new Vector3(-movementDirection.z, 0, movementDirection.x).normalized;

        startPosition = transform.position;
        time = 0;
    }

    private void RandomizeDirection()
    {
        float randomAngle = Random.Range(-wanderStrength, wanderStrength);
        Quaternion randomRotation = Quaternion.Euler(0, randomAngle, 0);

        movementDirection = randomRotation * movementDirection;
        movementDirection.Normalize();

        perpendicularDirection = new Vector3(-movementDirection.z, 0, movementDirection.x).normalized;

        startPosition = transform.position;
        time = 0;
    }
}
