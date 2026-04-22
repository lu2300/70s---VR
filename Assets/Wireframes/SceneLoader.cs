using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public FadeManager fadeManager;

    public Sprite imagenPatio;
    public Sprite imagenPueblo;

    public void IrAPatioDeJuegos()
    {
        StartCoroutine(CargarEscena("Patio de juegos", imagenPatio));
    }
    
    public void IrAlPueblo()
    {
        StartCoroutine(CargarEscena("Principal VR - PC", imagenPueblo));
    }

    IEnumerator CargarEscena(string nombreEscena, Sprite imagen)
    {
        // Fade con imagen
        yield return StartCoroutine(fadeManager.FadeOut(imagen));

        // Espera opcional para que se vea la imagen
        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(nombreEscena);
    }
}