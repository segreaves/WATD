using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionComponent : MonoBehaviour
{
    public void HitSlowMotion()
    {
        StartCoroutine(ESlow());
    }

    IEnumerator ESlow()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.005f);
        Time.timeScale = 1f;
    }
}
