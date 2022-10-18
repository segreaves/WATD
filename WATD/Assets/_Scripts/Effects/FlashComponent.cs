using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashComponent : MonoBehaviour
{
    private SkinnedMeshRenderer meshRenderer;
    Color originalColor;
    Color flashColor = Color.red;
    float flashTime = .15f;

    private void Start()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (meshRenderer == null) { return; }
        originalColor = meshRenderer.material.color;
        if (gameObject.tag == "Player")
        {
            flashColor = Color.white;
        }
    }

    public void HitFlash()
    {
        StartCoroutine(EFlash());
    }

    IEnumerator EFlash()
    {
        meshRenderer.material.color = flashColor;
        yield return new WaitForSeconds(flashTime);
        meshRenderer.material.color = originalColor;
    }
}
