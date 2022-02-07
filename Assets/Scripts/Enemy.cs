using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Enemy : Entity
{

    public float Speed;

    public delegate void Death();
    public Death OnDeath;

    protected override void Awake()
    {
        base.Awake();

        OnDeath = Despawn;
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(IsKillTrigger(other))
        {
            OnDeath?.Invoke();
        }
    }
}
