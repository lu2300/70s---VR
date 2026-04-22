using UnityEngine;
using UnityEngine.XR;

public class PlayerModeManager : MonoBehaviour
{
    public GameObject xrRig;
    public GameObject desktopPlayer;

    void Start()
    {
        Invoke(nameof(CheckXR), 0.5f);
    }

    void CheckXR()
    {
        if (XRSettings.isDeviceActive)
        {
            Debug.Log("Modo VR Activado");
            xrRig.SetActive(true);
            desktopPlayer.SetActive(false);
        }
        else
        {
            Debug.Log("Modo Desktop Activado");
            xrRig.SetActive(false);
            desktopPlayer.SetActive(true);
        }
    }

    // Agrega esto en PlayerModeManager.cs
    public bool IsVRMode => xrRig != null && xrRig.gameObject.activeSelf;
}

