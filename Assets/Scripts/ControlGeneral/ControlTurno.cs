using UnityEngine;
// ¡IMPORTANTE! Agregamos esta librería para poder controlar el Canvas (el botón)
using UnityEngine.UI;

public class ControlTurno : MonoBehaviour
{
    [Header("Referencias del Combate")]
    public GameObject balaPrefab; // Aquí arrastraremos la bala amarilla
    public Transform puntoDisparo;  // Donde está la mano del personaje
    public Button botonIniciarUI; // El botón del Canvas
    public ArrastrarArma scriptArrastreArma; // El script que mueve el arma verde

    void Start()
    {
        // Al arrancar, nos aseguramos de que el botón de Iniciar esté activado y visible
        if (botonIniciarUI != null) botonIniciarUI.gameObject.SetActive(true);
    }

    // Esta función es la que se activará cuando hagamos clic en el botón INICIAR
    public void PresionarIniciar()
    {
        // REGLA 1: Solo dispara si el arma está en la cuadrícula (para saberlo, verificamos si está visible en la mano)
        // Buscamos el objeto "Arma_Equipada_Visual" dentro del puntoDisparo
        GameObject armaEnMano = puntoDisparo.GetChild(0).gameObject; // Asumimos que es el primer hijo

        if (armaEnMano == null || !armaEnMano.activeSelf)
        {
            Debug.LogWarning("¡No se puede iniciar el turno si el arma no está en el inventario!");
            return;
        }

        // --- SI LLEGÓ ACÁ, EL DISPARO ES VÁLIDO ---
        Debug.Log("¡Turno iniciado! Disparando manceuerna.");

        

        // REGLA 3: Aparece la bala Prefab en la mano del personaje.
        // Instanciamos (creamos una copia) del prefab amarillo en la posición y rotación de la mano.
        if (balaPrefab != null && puntoDisparo != null)
        {
            Instantiate(balaPrefab, puntoDisparo.position, puntoDisparo.rotation);
        }

        // REGLA 4: Desactivar el botón para que no se pueda volver a presionar.
        if (botonIniciarUI != null)
        {
            botonIniciarUI.gameObject.SetActive(false);
        }
    }
}