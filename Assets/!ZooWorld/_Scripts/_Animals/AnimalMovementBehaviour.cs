using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalMovementBehaviour : MonoBehaviour
{
    public bool CanMove { get; set; }

    private void Update()
    {
        if (CanMove == false)
        {
            return;
        }
        
        Move();
    }

    protected virtual void Move()
    {
    }
}
