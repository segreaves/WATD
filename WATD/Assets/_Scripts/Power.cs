using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour
{
    public float maxPower = 3;
    [SerializeField] public float power { get; set; }

    private void Start()
    {
        power = maxPower;
    }

    public void UsePower(float powerCost)
    {
        if (power == 0) { return; }
        // Remove power from maxPower
        power = Mathf.Max(power - powerCost, 0);
        Debug.Log("Power.cs = " + power);
    }
}
