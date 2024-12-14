using System;
using System.Collections;
using UnityEngine;

public class EagleMovement : AnimalMovementBehaviour
{
    [SerializeField] private float speed = 5f; // Flying speed
    [SerializeField] private float screenMargin = 0.1f; // Margin before reaching the edge
    [SerializeField] private float rotationSpeed = 5f; // Speed of rotation while turning
    [SerializeField] private bool adjustOnSpawn = true; // Force inward movement on spawn
    [SerializeField] private float flyHeight = 10f; // Default flying height
    private Camera mainCamera;
    private bool isPaused;

    void Start()
    {
        mainCamera = Camera.main;

        if (adjustOnSpawn)
        {
            EnsureInitialDirectionInward();
        }
    }

    protected override void Move()
    {
        if (isPaused) return;

        // Move forward in the current direction
        Vector3 targetPos = transform.position + transform.forward * speed * Time.deltaTime;
        targetPos.y = flyHeight;
        transform.position = targetPos;

        // Check if near the edge of the screen
        if (IsNearScreenEdge())
        {
            StartCoroutine(PauseAndRotate());
        }
    }

    private void EnsureInitialDirectionInward()
    {
        Vector3 screenCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane + 10f));
        screenCenter.y = transform.position.y; // Keep horizontal movement
        Vector3 initialDirection = (screenCenter - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(initialDirection);
    }

    private bool IsNearScreenEdge()
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        return viewportPosition.x < 0 + screenMargin || viewportPosition.x > 1 - screenMargin ||
               viewportPosition.y < 0 + screenMargin || viewportPosition.y > 1 - screenMargin;
    }

    private IEnumerator PauseAndRotate()
    {
        isPaused = true;

        // Generate a random direction biased toward the center
        Vector3 randomDirection = GetRandomDirectionTowardsCenter();
        Quaternion targetRotation = Quaternion.LookRotation(randomDirection);

        // Smoothly rotate towards the new direction
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        isPaused = false;
    }

    private Vector3 GetRandomDirectionTowardsCenter()
    {
        // Get the screen center position in world space
        Vector3 screenCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane + 10f));
        screenCenter.y = transform.position.y; // Keep flying at the same height

        // Calculate the direction toward the center
        Vector3 directionToCenter = (screenCenter - transform.position).normalized;

        // Add randomness to the direction
        float randomAngle = UnityEngine.Random.Range(-45f, 45f); // Adjust the randomness range as needed
        Quaternion randomRotation = Quaternion.Euler(0, randomAngle, 0);

        return (randomRotation * directionToCenter).normalized;
    }
}
