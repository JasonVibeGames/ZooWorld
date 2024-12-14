using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Animal Data", menuName = "Data/Create Animal Data", order = 1)]
public class AnimalData : ScriptableObject
{
    public enum AnimalRole
    {
        Prey,
        Predator
    }
    
    [SerializeField] private AnimalRole animalRole;
    public AnimalRole Role => animalRole;
}
