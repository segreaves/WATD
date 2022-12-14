using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour
{
    public float maxPower = 3;
    public float power;

    private void Start()
    {
        power = maxPower;
    }

    public void UsePower(float powerCost)
    {
        if (power == 0) { return; }
        // Remove power from maxPower
        power = Mathf.Max(power - powerCost, 0);
    }
}
