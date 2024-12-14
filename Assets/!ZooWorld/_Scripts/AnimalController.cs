using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Events;

public class AnimalController : MonoBehaviour
{
    [Header("Set Up")]
    [SerializeField] private AnimalMovementBehaviour _animalMovementBehaviour;
    [SerializeField] private AnimalData _animalData;
    [SerializeField] private LayerMask _animalLayer;

    [Header("VFX")] 
    [SerializeField] private GameObject _eatVFX;
    Vector3 _eatVFXOffset = new Vector3(0,.5f,0);

    private bool hasHandledCollision = false;
    private UnityAction<AnimalController> _onDespawn;
    [SerializeField] private BoxDetector _boxDetector;
    public int priorityID;

    public int PriorityID => priorityID;

    void GenerateNewPriorityId()
    {
        // Generate a new unique priority ID (use a random or timestamp-based approach)
        priorityID = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }

    private void Awake()
    {
        _animalMovementBehaviour = GetComponent<AnimalMovementBehaviour>();
        _boxDetector.Initialize(OnDetectTrigger);
    }

    private void OnEnable()
    {
        GenerateNewPriorityId();
    }

    private void OnDisable()
    {
        _onDespawn?.Invoke(this);
    }

    public void Initialize(UnityAction<AnimalController> onDespawn)
    {
        hasHandledCollision = false;
        _animalMovementBehaviour.CanMove = true;
        _onDespawn = onDespawn;
    }
    
    private void OnDetectTrigger(Collider other)
    {
        if (hasHandledCollision) return;

        if ((_animalLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            AnimalController otherAnimal = other.attachedRigidbody.GetComponent<AnimalController>();

            if (otherAnimal == null)
            {
                return;
            }
            
            if (_animalData.Role == AnimalData.AnimalRole.Prey && otherAnimal._animalData.Role == AnimalData.AnimalRole.Prey)
            {
                _animalMovementBehaviour.BounceAway();
                otherAnimal._animalMovementBehaviour.BounceAway();
            }
            
            if (_animalData.Role == AnimalData.AnimalRole.Predator && otherAnimal._animalData.Role == AnimalData.AnimalRole.Prey)
            { 
                Eat(otherAnimal);
            }
            
            if (_animalData.Role == AnimalData.AnimalRole.Prey && otherAnimal._animalData.Role == AnimalData.AnimalRole.Predator)
            { 
                GetEaten();
            }
            
            if (_animalData.Role == AnimalData.AnimalRole.Predator && otherAnimal._animalData.Role == AnimalData.AnimalRole.Predator)
            { 
                // Compare instance IDs to determine which one survives
                if (this.PriorityID < otherAnimal.PriorityID)
                {
                    GetEaten();
                }
                else
                {
                    Eat(otherAnimal);
                }

                this.hasHandledCollision = true;
                otherAnimal.hasHandledCollision = true;
            }
        }    
    }
    

    private void Eat(AnimalController animalToEat)
    {
        LeanPool.Spawn(_eatVFX, animalToEat.transform.position + _eatVFXOffset, _eatVFX.transform.rotation);
        LeanPool.Despawn(animalToEat.gameObject);
        
        GenerateNewPriorityId();
    }

    private void GetEaten()
    {
        LeanPool.Spawn(_eatVFX, transform.position + _eatVFXOffset, _eatVFX.transform.rotation);
        LeanPool.Despawn(gameObject);
        
        GenerateNewPriorityId();
    }
    
}
