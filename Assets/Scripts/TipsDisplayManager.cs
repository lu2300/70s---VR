using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TipsDisplayManager : MonoBehaviour
{
    [Header("Referencia al Game Manager")]
    [SerializeField] private PlayerModeManager playerModeManager;

    [Header("Consejos")]
    [SerializeField] private TipEntry[] tips;

    [System.Serializable]
    public class TipEntry
    {
        [Tooltip("Nombre descriptivo (solo para identificar en el Inspector)")]
        public string label;

        [Tooltip("Componente Image del Canvas de consejo")]
        public Image targetImage;

        [Tooltip("Sprite que se muestra en modo Escritorio")]
        public Sprite desktopSprite;

        [Tooltip("Sprite que se muestra en modo VR")]
        public Sprite vrSprite;
    }

    private IEnumerator Start()
    {
        // Espera 0.6s para que el Invoke de PlayerModeManager (0.5s) termine primero
        yield return new WaitForSeconds(0.6f);

        if (playerModeManager == null)
        {
            Debug.LogError("[TipsDisplayManager] Falta asignar el PlayerModeManager.", this);
            yield break;
        }

        ApplyTips();
    }

    public void ApplyTips()
    {
        bool vr = playerModeManager.IsVRMode;

        foreach (TipEntry tip in tips)
        {
            if (tip.targetImage == null)
            {
                Debug.LogWarning($"[TipsDisplayManager] '{tip.label}' no tiene Image asignada.", this);
                continue;
            }

            tip.targetImage.sprite = vr ? tip.vrSprite : tip.desktopSprite;
            tip.targetImage.enabled = tip.targetImage.sprite != null;
        }

        Debug.Log($"[TipsDisplayManager] Modo aplicado: {(vr ? "VR" : "Escritorio")}");
    }
}
