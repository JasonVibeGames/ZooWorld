using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    private List<AnimalController> _animals = new List<AnimalController>();
    [SerializeField] private int _maxAnimals;
    [SerializeField] private AnimalController[] animalPrefabs;
    [SerializeField] private Vector2 _spawnIntervalRange = new Vector2(1,2);
    [SerializeField] private float spawnDistance = 10f;
    private float _spawnInterval;

    private float timer;
    private Camera mainCamera;


    void Start()
    {
        mainCamera = Camera.main;
        ResetSpawnInterval();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= _spawnInterval)
        {
            SpawnAnimal();
            ResetSpawnInterval();
        }
    }

    void ResetSpawnInterval()
    {
        timer = 0;
        _spawnInterval = Random.Range(_spawnIntervalRange.x, _spawnIntervalRange.y);
    }

    void SpawnAnimal()
    {
        if (animalPrefabs.Length == 0)
        {
            return;
        }

        if (_animals.Count >= _maxAnimals)
        {
            return;
        }

        Vector3 spawnPosition = GetRandomBorderPosition();

        // Set Y position to 0
        spawnPosition.y = 0;

        GameObject randomAnimal = animalPrefabs[Random.Range(0, animalPrefabs.Length)].gameObject;
        GameObject spawnedGameObject = LeanPool.Spawn(randomAnimal, spawnPosition, Quaternion.identity);
        AnimalController spawnedAnimal = spawnedGameObject.GetComponent<AnimalController>();
        _animals.Add(spawnedAnimal);
        
        // Calculate direction to the center
        Vector3 directionToCenter = new Vector3(0, 0, 0) - spawnPosition;
        directionToCenter.y = 0; // Ensure no vertical rotation

        // Rotate animal to face the center on the Y-axis only
        spawnedAnimal.transform.rotation = Quaternion.LookRotation(directionToCenter);
    }

    Vector3 GetRandomBorderPosition()
    {
        Vector3 viewportPoint = Vector3.zero;
        int border = Random.Range(0, 4); // 0: Top, 1: Bottom, 2: Left, 3: Right
        float offset = 0.05f; // Slight offset to ensure it's just off-screen

        switch (border)
        {
            case 0: // Top
                viewportPoint = new Vector3(Random.Range(0f, 1f), 1f + offset, mainCamera.nearClipPlane + 10f);
                break;
            case 1: // Bottom
                viewportPoint = new Vector3(Random.Range(0f, 1f), 0f - offset, mainCamera.nearClipPlane + 10f);
                break;
            case 2: // Left
                viewportPoint = new Vector3(0f - offset, Random.Range(0f, 1f), mainCamera.nearClipPlane + 10f);
                break;
            case 3: // Right
                viewportPoint = new Vector3(1f + offset, Random.Range(0f, 1f), mainCamera.nearClipPlane + 10f);
                break;
        }

        Vector3 worldPosition = mainCamera.ViewportToWorldPoint(viewportPoint);
        worldPosition.y = 0; // Ensure Y is 0 for spawning on the X-Z plane
        return worldPosition;
    }

}
