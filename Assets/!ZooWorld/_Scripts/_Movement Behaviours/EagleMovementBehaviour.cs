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
    [SerializeField] private float swoopDuration;
    [SerializeField] private float _swoopSpeed;
    [SerializeField] private BoxDetector _swoopDetector;
    [SerializeField] private Transform art; // Reference to the child transform
    [SerializeField] private float _swoopRotationSpeed;
    private Coroutine swoopingCR;
    private float swoopHeight = 0;
    private Camera mainCamera;
    private bool _isRotating;
    private bool _isSwooping;
    private bool _justRotated;

    public bool IsMoving => !_isRotating;
    
    void Start()
    {
        mainCamera = Camera.main;

        if (adjustOnSpawn)
        {
            EnsureInitialDirectionInward();
        }

        _swoopDetector.Initialize(OnDetectPrey);
    }

    private void OnDetectPrey(Collider other)
    {
        SwoopDown();
    }

    private void OnDisable()
    {
        _isSwooping = false;
        _isRotating = false;
    }

    protected override void Move()
    {
        if (_isRotating) return;

        // Move forward in the current direction
        Vector3 targetPos = transform.position + transform.forward * speed * Time.deltaTime;
        float targetHeight = flyHeight;

        if (_isSwooping)
        {
            targetHeight = 0; // Swoop down to ground level
        }

        // Adjust height towards the target
        targetPos.y = Mathf.MoveTowards(transform.position.y, targetHeight, _swoopSpeed * Time.deltaTime);

        // Calculate the movement vector
        Vector3 movementVector = targetPos - transform.position;

        if (art != null && movementVector != Vector3.zero) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementVector);
            art.rotation = Quaternion.RotateTowards(art.rotation, targetRotation, _swoopRotationSpeed * Time.deltaTime);
        }

        // Update the position of the root GameObject
        transform.position = targetPos;

        // Check if near the edge of the screen
        if (IsNearScreenEdge() && !_isRotating && !_justRotated)
        {
            StartCoroutine(PauseAndRotate());
        }
    }

    private void EnsureInitialDirectionInward()
    {
        Vector3 screenCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane + 10f));
        screenCenter.y = transform.position.y; // Keep horizontal movement
        Vector3 initialDirection = (screenCenter - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(initialDirection); // Root rotation
    }

    private bool IsNearScreenEdge()
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        return viewportPosition.x < 0 + screenMargin || viewportPosition.x > 1 - screenMargin ||
               viewportPosition.y < 0 + screenMargin || viewportPosition.y > 1 - screenMargin;
    }

    private IEnumerator PauseAndRotate()
    {
        if (swoopingCR != null)
        {
            StopCoroutine(swoopingCR);
            _isSwooping = false;
        }
        
        _isRotating = true;

        // Generate a random direction biased toward the center
        Vector3 randomDirection = GetRandomDirectionTowardsCenter();
        Quaternion targetRotation = Quaternion.LookRotation(randomDirection);

        // Smoothly rotate the root GameObject
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        _isRotating = false;
        _justRotated = true;

        yield return new WaitForSeconds(1);
        _justRotated = false;
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

    public void SwoopDown()
    {
        if (_isSwooping || _isRotating)
        {
            return;
        }

        swoopingCR = StartCoroutine(PerformSwoop());
    }

    IEnumerator PerformSwoop()
    {
        _isSwooping = true;
        yield return new WaitForSeconds(swoopDuration);
        _isSwooping = false;
        swoopingCR = null;
    }
}
