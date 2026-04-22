using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class Configuraciones : MonoBehaviour
{
    [Header("--- UI ---")]
    public Toggle baja;
    public Toggle media;
    public Toggle alta;

    public Slider musica;
    public Slider efectos;

    [Header("--- Managers ---")]
    public UIManager uiManager;

    int calidad = 1;

    void Start()
    {
        CargarConfiguracion();
    }

    public void SetCalidad(int value)
    {
        calidad = value;
    }

    public void Guardar()
    {
        PlayerPrefs.SetInt("Calidad", calidad);
        PlayerPrefs.SetFloat("Musica", musica.value);
        PlayerPrefs.SetFloat("Efectos", efectos.value);
        PlayerPrefs.Save();

        AplicarConfiguracion();

        // Volver al menú según el modo activo
        if (XRSettings.isDeviceActive)
            uiManager.IrMenu_VR();
        else
            uiManager.IrMenu();
    }

    public void Restablecer()
    {
        calidad = 1;
        musica.value = 0.5f;
        efectos.value = 0.5f;

        ActualizarUI();
        AplicarConfiguracion();
    }

    void CargarConfiguracion()
    {
        calidad = PlayerPrefs.GetInt("Calidad", 1);
        musica.value = PlayerPrefs.GetFloat("Musica", 0.5f);
        efectos.value = PlayerPrefs.GetFloat("Efectos", 0.5f);

        ActualizarUI();
        AplicarConfiguracion();
    }

    void ActualizarUI()
    {
        baja.isOn = (calidad == 0);
        media.isOn = (calidad == 1);
        alta.isOn = (calidad == 2);
    }

    void AplicarConfiguracion()
    {
        QualitySettings.SetQualityLevel(calidad);
        AudioListener.volume = musica.value;
    }
}