using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    [SerializeField] GameObject Bullet;
    [SerializeField] Transform Muzzle;

    [SerializeField] float BulletForce = 20f, FiringDelay = 0.1f, ReloadTime = 1.5f;
    [SerializeField] int MaxAmmo = 30, CurrentAmmo = 30;

    bool Shooting = false, Reloading = false;

    private void Start()
    {
        CurrentAmmo = MaxAmmo;
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            if(CurrentAmmo <= 0)
            {
                if(!Reloading) //attempted fire but no ammo
                {
                    StartCoroutine(Reload());
                }
            }
            else
            {
                if (!Shooting && !Reloading)
                {
                    StartCoroutine(Fire());
                    CurrentAmmo--;
                }
            }
        }

        //optional "whenever" reload
        if(Input.GetKeyDown(KeyCode.R))
        {
            if (!Reloading && CurrentAmmo < MaxAmmo)
            {
                StartCoroutine(Reload());
            }
        }
    }

    IEnumerator Fire()
    {
        Shooting = true;

        GameObject newBullet = Instantiate(Bullet, Muzzle.position, Quaternion.identity);
        Quaternion bulletRotation = Quaternion.LookRotation(transform.forward, transform.up);
        newBullet.transform.rotation = bulletRotation;
        Rigidbody rb = newBullet.GetComponent<Rigidbody>();
        rb.AddForce(Camera.main.transform.forward * BulletForce, ForceMode.Impulse);

        yield return new WaitForSeconds(FiringDelay);

        Shooting = false;
    }

    IEnumerator Reload()
    {
        Reloading = true;

        yield return new WaitForSeconds(ReloadTime);
        CurrentAmmo = MaxAmmo;

        Reloading = false;
    }
}
