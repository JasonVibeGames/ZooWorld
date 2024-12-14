using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public UnityAction<int> onKillPredator;
    public UnityAction<int> onKillPrey;
    private int _predatorKillCount;
    private int _preyKillCount;
    
    private void Awake()
    {
        instance = this;
    }

    public void OnKill(AnimalData.AnimalRole animalRole)
    {
        if (animalRole == AnimalData.AnimalRole.Predator)
        {
            _predatorKillCount += 1;
            onKillPredator?.Invoke(_predatorKillCount);
        }
        
        if (animalRole == AnimalData.AnimalRole.Prey)
        {
            _preyKillCount += 1;
            onKillPrey?.Invoke(_preyKillCount);
        }
    }
}
