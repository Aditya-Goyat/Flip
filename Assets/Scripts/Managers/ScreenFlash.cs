using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{
    public static ScreenFlash Instance;

    [SerializeField] Image image;
    [SerializeField] float flashDuration = 0.15f;
    [SerializeField] float alpha = 0.15f;

    void Awake()
    {
        Instance = this;
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        image.color = new Color(1f, 1f, 1f, alpha);
        yield return new WaitForSeconds(flashDuration);
        image.color = Color.clear;
    }
}
