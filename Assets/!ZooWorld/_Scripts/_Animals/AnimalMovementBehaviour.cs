using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
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

    public virtual void BounceAway()
    {
        
    }
}
