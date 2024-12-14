using System;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class SnakeTail : MonoBehaviour
{
    [SerializeField] private Transform snakeHead;
    [SerializeField] private GameObject tailPrefab;
    [SerializeField] private int initialTailLength = 5;
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float distanceBetweenSegments = 0.5f;

    private List<Transform> tailSegments = new List<Transform>();

    void OnEnable()
    {
        InitializeTail();
    }

    void Update()
    {
        MoveTail();
    }

    private void OnDisable()
    {
        DespawnTail();
    }

    private void InitializeTail()
    {
        Vector3 previousPosition = snakeHead.position;

        for (int i = 0; i < initialTailLength; i++)
        {
            GameObject tailSegment = LeanPool.Spawn(tailPrefab, previousPosition, Quaternion.identity);
            tailSegments.Add(tailSegment.transform);

            previousPosition -= snakeHead.forward * distanceBetweenSegments;
        }
    }

    void DespawnTail()
    {
        foreach (var tailSegment in tailSegments)
        {
            LeanPool.Despawn(tailSegment);
        }
        
        tailSegments.Clear();
    }

    private void MoveTail()
    {
        Vector3 previousPosition = snakeHead.position;

        for (int i = 0; i < tailSegments.Count; i++)
        {
            Transform currentSegment = tailSegments[i];
            Vector3 newPosition = Vector3.Lerp(currentSegment.position, previousPosition, Time.deltaTime * followSpeed);

            currentSegment.position = newPosition;
            currentSegment.LookAt(previousPosition);

            previousPosition = currentSegment.position;
        }
    }
}