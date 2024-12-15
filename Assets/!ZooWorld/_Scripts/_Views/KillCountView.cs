using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillCountView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _predatorKillCount;
    [SerializeField] private TextMeshProUGUI _preyKillCount;

    private void Start()
    {
        GameManager.instance.onKillPredator += OnKillPredator;
        GameManager.instance.onKillPrey += OnKillPrey;

        _predatorKillCount.text = "Predator: " + 0.ToString();
        _preyKillCount.text = "Prey: " + 0.ToString();
    }

    private void OnKillPrey(int killCount)
    {
        _preyKillCount.text = "Prey: " + killCount;
    }

    private void OnKillPredator(int killCount)
    {
        _predatorKillCount.text = "Predator: " + killCount;
    }
}
