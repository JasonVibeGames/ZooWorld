using System.Collections.Generic;
using UnityEngine;

public class SnakeTail : MonoBehaviour
{
    [SerializeField] private Transform snakeHead;
    [SerializeField] private GameObject tailPrefab;
    [SerializeField] private int initialTailLength = 5;
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float distanceBetweenSegments = 0.5f;

    private List<Transform> tailSegments = new List<Transform>();

    void Start()
    {
        InitializeTail();
    }

    void Update()
    {
        MoveTail();
    }

    private void InitializeTail()
    {
        Vector3 previousPosition = snakeHead.position;

        for (int i = 0; i < initialTailLength; i++)
        {
            GameObject tailSegment = Instantiate(tailPrefab, previousPosition, Quaternion.identity);
            tailSegments.Add(tailSegment.transform);

            previousPosition -= snakeHead.forward * distanceBetweenSegments;
        }
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

    public void AddTailSegment()
    {
        Transform lastSegment = tailSegments[tailSegments.Count - 1];
        Vector3 newSegmentPosition = lastSegment.position - snakeHead.forward * distanceBetweenSegments;

        GameObject newSegment = Instantiate(tailPrefab, newSegmentPosition, Quaternion.identity);
        tailSegments.Add(newSegment.transform);
    }
}