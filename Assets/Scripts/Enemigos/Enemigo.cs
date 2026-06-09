using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Enemigo : MonoBehaviour
{
    [Header("Estadísticas de Vida")]
    public float vidaMax;
    public float vidaActual;

    [Header("Referencia de Interfaz")]
    public Slider barraVidaUI;

    [Header("Movimiento por Turnos Suave")]
    public float distanciaPaso = 0.5f;
    public float velocidadMovimiento = 3f;
    private bool seEstaMoviendo = false;
    private bool yaMurio = false; // Cerrojo interno del enemigo

    void Start()
    {
        vidaMax = Random.Range(1000, 1251);
        vidaActual = vidaMax;

        if (barraVidaUI != null)
        {
            barraVidaUI.maxValue = vidaMax;
            barraVidaUI.value = vidaActual;
        }

        Debug.Log($"¡Enemigo spawneado con {vidaActual} HP!");
    }

    public void RecibirDano(float cantidadDano)
    {
        if (yaMurio) return; // Si ya está muerto, no recibe más daño de balas fantasma

        vidaActual -= cantidadDano;

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
        if (vidaActual <= 0 || yaMurio || (GameController.Instancia != null && GameController.Instancia.juegoTerminado)) return;

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
        yaMurio = true; // Activamos el cerrojo para que no se repita esta función
        Debug.Log("¡El enemigo ha sido destruido!");

        // --- ENVIAR REPORTE OFICIAL AL GAMECONTROLLER ---
        if (GameController.Instancia != null)
        {
            GameController.Instancia.ReportarMuerteEnemigo();
        }

        Destroy(gameObject); // Ahora sí, Unity lo limpia de la escena de forma segura
    }
}