using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.gameObject;
        if(hitObject.TryGetComponent(out HealthHandler healthHandler))
        {
            healthHandler.TakeDamage(Damage);
        }

    }
}
