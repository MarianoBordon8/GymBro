using UnityEngine;
using System.Collections;
// ¡IMPORTANTE! Agregamos la librería de UI para poder controlar el Slider
using UnityEngine.UI;

public class Enemigo : MonoBehaviour
{
    [Header("Estadísticas de Vida")]
    public float vidaMax;
    public float vidaActual;

    [Header("Referencia de Interfaz")]
    // Aquí arrastraremos nuestro componente Slider
    public Slider barraVidaUI;

    [Header("Movimiento por Turnos Suave")]
    public float distanciaPaso = 0.5f;
    public float velocidadMovimiento = 3f;
    private bool seEstaMoviendo = false;

    void Start()
    {
        // Elegimos la vida aleatoria
        vidaMax = Random.Range(1000, 1251);
        vidaActual = vidaMax;

        // --- ¡NUEVO! ---
        // Configuramos los límites del Slider según la vida aleatoria que le tocó a este enemigo
        if (barraVidaUI != null)
        {
            barraVidaUI.maxValue = vidaMax;
            barraVidaUI.value = vidaActual;
        }

        Debug.Log($"¡Enemigo spawneado con {vidaActual} HP!");
    }

    public void RecibirDano(float cantidadDano)
    {
        vidaActual -= cantidadDano;

        // --- ¡NUEVO! ---
        // Actualizamos la barra verde visual en pantalla con la nueva vida restante
        if (barraVidaUI != null)
        {
            barraVidaUI.value = vidaActual;
        }

        Debug.Log($"¡Enemigo dañado! Recibió {cantidadDano}. Vida restante: {vidaActual}");

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    public void AvanzarUnTramo()
    {
        if (vidaActual <= 0 || (GameController.Instancia != null && GameController.Instancia.juegoTerminado)) return;

        if (!seEstaMoviendo)
        {
            Vector3 posicionDestino = transform.position;
            posicionDestino.x -= distanciaPaso;
            StartCoroutine(RutinaMovimientoSuave(posicionDestino));
        }
    }

    IEnumerator RutinaMovimientoSuave(Vector3 destino)
    {
        seEstaMoviendo = true;

        while (Vector3.Distance(transform.position, destino) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destino,
                velocidadMovimiento * Time.deltaTime
            );
            yield return null;
        }

        transform.position = destino;
        seEstaMoviendo = false;

        if (GameController.Instancia != null)
        {
            GameController.Instancia.FinalizarTurnoEnemigo();
        }
    }

    void OnTriggerEnter2D(Collider2D otroObjeto)
    {
        if (otroObjeto.CompareTag("Player"))
        {
            if (GameController.Instancia != null && !GameController.Instancia.juegoTerminado)
            {
                GameController.Instancia.DerrotaFisica();
            }
        }
    }

    private void Morir()
    {
        Debug.Log("¡El enemigo ha sido destruido!");
        Destroy(gameObject);
        if (GameController.Instancia != null) GameController.Instancia.VerificarCondiciones();
    }
}