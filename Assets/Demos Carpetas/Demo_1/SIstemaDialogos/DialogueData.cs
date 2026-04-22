using UnityEngine;

/// ─────────────────────────────────────────────────────────────────────────────
/// CÓMO USAR ESTE SCRIPTABLEOBJECT (para diseñadores sin código)
/// ─────────────────────────────────────────────────────────────────────────────
///
///  1. NPC NAME       →  El nombre que aparece en la cabecera del panel.
///
///  2. OPENING LINES  →  Las líneas que dice el NPC al principio.
///                        El jugador avanza con el botón "Continuar".
///
///  3. CHOICES        →  Al terminar las Opening Lines aparecen estos botones.
///                        Pon siempre 2. Cada uno tiene:
///
///      • Choice Text       → texto del botón (lo que "dice" el jugador)
///      • Is Exit Choice    → si ON, cierra el diálogo sin más respuesta
///      • Response Lines    → lo que responde el NPC si eligen esta opción
///      • Animation Trigger → 🔥 nombre del trigger que se activará en el Animator
///
///  EJEMPLO RÁPIDO:
///      Opening Lines:  "Hola viajero." / "¿En qué puedo ayudarte?"
///      Choice 0 → Text: "¿Qué sabes del bosque?"   IsExit: OFF
///                  Trigger: "Talk"
///                  Response: "Dicen que está maldito..." / "Ten cuidado."
///      Choice 1 → Text: "Nada, adiós."              IsExit: ON
///                  Trigger: "Bye"
///
///  Si no añades choices, el diálogo cierra al terminar las Opening Lines.
/// ─────────────────────────────────────────────────────────────────────────────

[System.Serializable]
public class DialogueChoice
{
    [Tooltip("Texto del botón — lo que el jugador dice / elige.")]
    public string choiceText = "Nueva opción";

    [Tooltip("ON  → esta opción cierra el diálogo directamente (sin respuesta del NPC).\n" +
             "OFF → el NPC responde con las Response Lines de abajo.")]
    public bool isExitChoice = false;

    [Tooltip(" Nombre del Trigger en el Animator que se activará al elegir esta opción.")]
    public string animationTrigger;

    [Tooltip("Líneas que dice el NPC después de elegir esta opción.\n" +
             "Se ignoran si Is Exit Choice está ON.")]
    [TextArea(2, 4)]
    public string[] responseLines;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("──── Información del NPC ────")]
    public string npcName = "NPC";

    [Header("──── Líneas de apertura ────")]
    [Tooltip("Secuencia de líneas que el NPC dice al inicio. El jugador avanza con Continuar.")]
    [TextArea(2, 5)]
    public string[] openingLines;

    [Header("──── Opciones del jugador (al final de las líneas) ────")]
    [Tooltip("Botones de elección que aparecen al terminar las Opening Lines.\n" +
             "Deja vacío si no hay elección y el diálogo simplemente termina.")]
    public DialogueChoice[] choices;
}