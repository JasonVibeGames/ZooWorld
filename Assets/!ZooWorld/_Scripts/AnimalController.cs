using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    [Header("Set Up")]
    [SerializeField] private AnimalMovementBehaviour _animalMovementBehaviour;

    private void Awake()
    {
        _animalMovementBehaviour = GetComponent<AnimalMovementBehaviour>();
    }

    private void OnEnable()
    {
        Initialize();
    }

    void Initialize()
    {
        _animalMovementBehaviour.CanMove = true;
    }
}
