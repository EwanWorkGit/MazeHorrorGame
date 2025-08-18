using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour, IDamageable
{
    public float BaseHealth = 100f;
    public float Health = 100f;

    public void TakeDamage(float damage)
    {
        Health -= damage;
    }
}
