using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenFlash : MonoBehaviour
{
    [SerializeField] private float flashAlpha;
    [SerializeField] private float duration;
    [SerializeField] private Material _material;
    private Coroutine flashCR;
  
    void Start()
    {
        GameManager.instance.onKill += ScreenFlash;
    }

    void ScreenFlash()
    {
        if (flashCR != null)
        {
            StopCoroutine(flashCR);
            flashCR = null;
        }
        
        flashCR = StartCoroutine(AnimateMaterialProperties());
    }

    private IEnumerator AnimateMaterialProperties()
    {
        float currentAlpha = _material.GetFloat("_Intensity");
        float elapsed = 0f;

        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2);
            _material.SetFloat("_Intensity", Mathf.Lerp(currentAlpha, flashAlpha, t));
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2);
            _material.SetFloat("_Intensity", Mathf.Lerp(flashAlpha, 0, t));
            yield return null;
        }

        flashCR = null;
    }


}
