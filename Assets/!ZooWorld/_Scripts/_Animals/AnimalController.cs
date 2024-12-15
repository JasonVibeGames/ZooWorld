using System;
using System.Collections;
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
    [SerializeField] private GameObject _bloodVFX;
    private Vector3 _eatVFXOffset = new Vector3(0, .5f, 0);

    private UnityAction<AnimalController> _onDespawn;
    [SerializeField] private BoxDetector _boxDetector;

    public int priorityID;

    public int PriorityID => priorityID;

    private bool _eatHandled;

    private void Awake()
    {
        _animalMovementBehaviour = GetComponent<AnimalMovementBehaviour>();
        _boxDetector.Initialize(OnDetectTrigger);
    }

    private void OnEnable()
    {
        _eatHandled = false;
        GenerateNewPriorityId();
    }

    private void OnDisable()
    {
        GameManager.instance.OnKill(_animalData.Role);
        _onDespawn?.Invoke(this);
    }

    public void Initialize(UnityAction<AnimalController> onDespawn)
    {
        _animalMovementBehaviour.CanMove = true;
        _onDespawn = onDespawn;
    }

    private void GenerateNewPriorityId()
    {
        priorityID = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }

    private void OnDetectTrigger(Collider other)
    {

        if ((_animalLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            AnimalController otherAnimal = other.attachedRigidbody.GetComponent<AnimalController>();

            if (otherAnimal == null)
            {
                return;
            }

            // Collision handling logic
            if (_animalData.Role == AnimalData.AnimalRole.Prey && otherAnimal._animalData.Role == AnimalData.AnimalRole.Prey)
            {
                _animalMovementBehaviour.BounceAway();
                otherAnimal._animalMovementBehaviour.BounceAway();
            }
            else if (_animalData.Role == AnimalData.AnimalRole.Predator && otherAnimal._animalData.Role == AnimalData.AnimalRole.Prey)
            {
                otherAnimal.GetEaten();
            }
            else if (_animalData.Role == AnimalData.AnimalRole.Prey && otherAnimal._animalData.Role == AnimalData.AnimalRole.Predator)
            {
                GetEaten();
            }
            else if (_animalData.Role == AnimalData.AnimalRole.Predator && otherAnimal._animalData.Role == AnimalData.AnimalRole.Predator)
            {
                // Compare instance IDs to determine which one survives
                if (this.PriorityID < otherAnimal.PriorityID)
                {
                    GetEaten();
                }
                else
                {
                    otherAnimal.GetEaten();
                }
            }
        }
    }

    private void GetEaten()
    {
        if (_eatHandled)
        {
            return;
        }
        
        _eatHandled = true;
        LeanPool.Spawn(_bloodVFX, transform.position, _bloodVFX.transform.rotation);
        LeanPool.Spawn(_eatVFX, transform.position + _eatVFXOffset, _eatVFX.transform.rotation);
        LeanPool.Despawn(gameObject);
    }
}
