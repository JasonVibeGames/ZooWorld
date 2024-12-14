using System;
using System.Collections;
using UnityEngine;

public class EagleMovement : AnimalMovementBehaviour
{
    [SerializeField] private float speed = 5f; // Flying speed
    [SerializeField] private float screenMargin = 0.1f; // Margin before reaching the edge
    [SerializeField] private float rotationSpeed = 5f; // Speed of rotation while turning
    [SerializeField] private bool adjustOnSpawn = true; // Force inward movement on spawn
    [SerializeField] private float flyHeight;
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
        if (IsNearScreenEdge(out Vector3 directionToCenter))
        {
            StartCoroutine(PauseAndRotate(directionToCenter));
        }
    }

    private void EnsureInitialDirectionInward()
    {
        // Get screen center and adjust direction toward it
        Vector3 screenCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane + 10f));
        screenCenter.y = transform.position.y; // Keep horizontal movement
        Vector3 initialDirection = (screenCenter - transform.position).normalized;

        // Set movement direction and rotation
        transform.rotation = Quaternion.LookRotation(initialDirection);
    }

    private bool IsNearScreenEdge(out Vector3 directionToCenter)
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
        directionToCenter = Vector3.zero;

        // Check if the eagle is near the edges of the screen
        if (viewportPosition.x < 0 + screenMargin || viewportPosition.x > 1 - screenMargin ||
            viewportPosition.y < 0 + screenMargin || viewportPosition.y > 1 - screenMargin)
        {
            Vector3 screenCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane + 10f));
            directionToCenter = (screenCenter - transform.position).normalized;
            directionToCenter.y = 0; // Keep movement on the horizontal plane
            return true;
        }
        return false;
    }

    private IEnumerator PauseAndRotate(Vector3 directionToCenter)
    {
        isPaused = true;
        
        // Smoothly rotate towards the new direction
        Quaternion targetRotation = Quaternion.LookRotation(directionToCenter);
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        // Resume movement
        isPaused = false;
    }
}
