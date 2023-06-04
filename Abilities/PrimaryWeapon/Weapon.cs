using Gamekit3D;
using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public GameObject Owner { get; protected set; }
    public WeaponType WeaponType { get; protected set; }

    protected Action hitCallback;

    public virtual void Init(GameObject owner, Action hitCallback)
    {
        Owner = owner;
        this.hitCallback = hitCallback;
    }
}
