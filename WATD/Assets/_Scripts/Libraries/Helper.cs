using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
 {
    public static Vector3 Vector3Mean(Vector3[] vectors)
    {
        if (vectors.Length == 0) { return Vector3.zero; }
        Vector3 total = Vector3.zero; 
        for (int i = 0; i < vectors.Length; i++)
        {
            total += vectors[i];
        }
        return total/vectors.Length;
    }
 }
