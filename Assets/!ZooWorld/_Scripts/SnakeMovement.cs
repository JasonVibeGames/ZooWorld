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
    [SerializeField] private float screenMargin = 0.1f;
    [SerializeField] private float steerSpeed = 5f; // Speed of steering

    private float time;
    private Vector3 movementDirection;
    private Vector3 perpendicularDirection;
    private Camera mainCamera;
    private float wanderTimer;
    private bool isSteering;

    void OnEnable()
    {
        movementDirection = transform.forward.normalized;
        perpendicularDirection = new Vector3(-movementDirection.z, 0, movementDirection.x).normalized;
        mainCamera = Camera.main;
    }

    protected override void Move()
    {
        wanderTimer += Time.deltaTime;

        if (!isSteering && wanderTimer >= wanderInterval)
        {
            RandomizeDirection();
            wanderTimer = 0f;
        }

        if (IsNearScreenEdge(out Vector3 directionToCenter))
        {
            SteerTowardsCenter(directionToCenter);
        }
        else
        {
            isSteering = false;
        }

        time += Time.deltaTime;

        float waveOffset = Mathf.Sin(time * frequency) * amplitude;
        Vector3 forwardMovement = movementDirection * speed * Time.deltaTime;
        Vector3 oscillation = perpendicularDirection * waveOffset;

        transform.position += forwardMovement;

        if (!isSteering)
        {
            transform.position += oscillation * Time.deltaTime * positionLerpSpeed;

            Vector3 oscillatingDirection = movementDirection + oscillation.normalized * 0.1f;
            Quaternion targetRotation = Quaternion.LookRotation(oscillatingDirection.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);
        }
        else
        {
            Vector3 direction = movementDirection.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);
        }
    }

    private bool IsNearScreenEdge(out Vector3 directionToCenter)
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
        directionToCenter = Vector3.zero;

        if (viewportPosition.x < 0 + screenMargin || viewportPosition.x > 1 - screenMargin ||
            viewportPosition.y < 0 + screenMargin || viewportPosition.y > 1 - screenMargin)
        {
            Vector3 screenCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane + 10f));
            directionToCenter = (screenCenter - transform.position).normalized;
            directionToCenter.y = 0;
            return true;
        }
        return false;
    }

    private void SteerTowardsCenter(Vector3 directionToCenter)
    {
        isSteering = true;

        // Use a higher steerSpeed multiplier to make steering faster
        movementDirection = Vector3.Lerp(movementDirection, directionToCenter, Time.deltaTime * steerSpeed).normalized;

        perpendicularDirection = new Vector3(-movementDirection.z, 0, movementDirection.x).normalized;
    }

    private void RandomizeDirection()
    {
        float randomAngle = Random.Range(-wanderStrength, wanderStrength);
        Quaternion randomRotation = Quaternion.Euler(0, randomAngle, 0);

        movementDirection = randomRotation * movementDirection;
        movementDirection.Normalize();

        perpendicularDirection = new Vector3(-movementDirection.z, 0, movementDirection.x).normalized;
    }
}
