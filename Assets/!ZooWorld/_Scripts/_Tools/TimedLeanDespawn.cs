using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class TimedLeanDespawn : MonoBehaviour
{
    [SerializeField] private float _despawnTime;

    private void OnEnable()
    {
        StartCoroutine(DespawnAfterTime());
    }

    IEnumerator DespawnAfterTime()
    {
        yield return new WaitForSeconds(_despawnTime);
        LeanPool.Despawn(gameObject);
    }
}
