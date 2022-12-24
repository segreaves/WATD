using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunController : MonoBehaviour, IShootable
{
    [field: SerializeField] public UnityEvent OnShoot { get; set; }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void StartShootting()
    {}

    public void StopShootting()
    {}
}
