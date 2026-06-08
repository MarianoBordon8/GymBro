using UnityEngine;
using UnityEngine.UI;

public class ControlTurno : MonoBehaviour
{
    [Header("Referencias del Combate")]
    public GameObject balaPrefab;
    public Transform puntoDisparo;
    public Button botonIniciarUI;
    public ArrastrarArma scriptArrastreArma;

    // Este método lo sigue llamando el BOTÓN una sola vez al principio
    public void PresionarIniciar()
    {
        // Verificamos si el arma está equipada
        GameObject armaEnMano = puntoDisparo.GetChild(0).gameObject;

        if (armaEnMano == null || !armaEnMano.activeSelf)
        {
            Debug.LogWarning("¡Coloca el arma en la cuadrícula antes de iniciar!");
            return;
        }

        // Bloqueamos el arma permanentemente por toda la batalla
        if (scriptArrastreArma != null)
        {
            scriptArrastreArma.enabled = false;
        }

        // Hacemos desaparecer el botón para siempre en esta partida
        if (botonIniciarUI != null)
        {
            botonIniciarUI.gameObject.SetActive(false);
        }

        // Lanzamos el primer disparo para arrancar la reacción en cadena
        DispararBalaAutomatica();
    }

    // NUEVA FUNCIÓN: Se encarga puramente de clonar la bala
    public void DispararBalaAutomatica()
    {
        // Si el juego ya terminó por victoria o derrota, no dispara más
        if (GameController.Instancia != null && GameController.Instancia.juegoTerminado) return;

        if (balaPrefab != null && puntoDisparo != null)
        {
            Instantiate(balaPrefab, puntoDisparo.position, puntoDisparo.rotation);
            Debug.Log("Mancuerna disparada automáticamente.");
        }
    }
}