using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileBaseThomos : MonoBehaviour
{
    public GameObject Owner { get; private set; }
    public Vector3 InitialPosition { get; private set; }
    public Vector3 InitialDirection { get; private set; }
    public Vector3 InheritedMuzzleVelocity { get; private set; }

    public UnityAction OnShoot;

    public void Shoot(WeaponController weaponController)
    {
        Owner = weaponController.Owner;
        InitialPosition = transform.position;
        InitialDirection = transform.forward;
        InheritedMuzzleVelocity = weaponController.MuzzleWorldVelocity;
        OnShoot?.Invoke();
    }
}
