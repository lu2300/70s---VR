using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    [Header("--- ESCRITORIO ---")]
    public Image fadeImage;

    [Header("--- VR ---")]
    public Image fadeImage_VR;

    [Header("--- Config ---")]
    public float fadeDuration = 0.5f;

    private bool esVR;

    void Start()
    {
        Invoke(nameof(CheckModo), 0.6f); // un poco después que el UIManager
    }

    void CheckModo()
    {
        esVR = UnityEngine.XR.XRSettings.isDeviceActive;
        StartCoroutine(FadeIn());
    }

    Image ImagenActual() => esVR ? fadeImage_VR : fadeImage;

    public IEnumerator FadeOut(Sprite nuevaImagen = null)
    {
        Image img = ImagenActual();

        if (nuevaImagen != null)
        {
            img.sprite = nuevaImagen;
            img.color = new Color(1, 1, 1, 0);
        }

        float t = 0;
        Color color = img.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            img.color = color;
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        Image img = ImagenActual();

        float t = 0;
        Color color = img.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            img.color = color;
            yield return null;
        }
    }
}