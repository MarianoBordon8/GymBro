using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController Instancia { get; private set; }

    [Header("Estados del Juego")]
    public bool esTurnoDelJugador = true;
    public bool juegoTerminado = false;

    [Header("Referencias de la Escena")]
    public ControlTurno controlTurno;
    public GridManager gridManager;
    public ArrastrarArma scriptArrastreArma;
    public GameObject fisicoculturista;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    public void FinalizarDisparoJugador()
    {
        if (juegoTerminado) return;
        esTurnoDelJugador = false;
        StartCoroutine(RutinaTurnoEnemigo());
    }

    IEnumerator RutinaTurnoEnemigo()
    {
        yield return new WaitForSeconds(0.2f); // Pausa corta tras el impacto

        Enemigo enemigo = FindAnyObjectByType<Enemigo>();
        if (enemigo != null)
        {
            enemigo.AvanzarUnTramo();
        }
        else
        {
            VerificarCondiciones();
        }
    }

    public void FinalizarTurnoEnemigo()
    {
        if (juegoTerminado) return;

        // Si el enemigo no tocó al jugador al moverse, el juego sigue
        StartCoroutine(RutinaSiguienteDisparoAutomatico());
    }

    IEnumerator RutinaSiguienteDisparoAutomatico()
    {
        yield return new WaitForSeconds(0.6f); // Pausa de respiro entre turnos

        esTurnoDelJugador = true;

        if (controlTurno != null && !juegoTerminado)
        {
            controlTurno.DispararBalaAutomatica();
        }
    }

    public void VerificarCondiciones()
    {
        Enemigo enemigo = FindAnyObjectByType<Enemigo>();
        // Solo chequeamos victoria acá (si ya no hay enemigo)
        if (enemigo == null || enemigo.vidaActual <= 0)
        {
            Victoria();
        }
    }

    // Esta función la llama el enemigo físicamente al chocar el tag "Player"
    public void DerrotaFisica()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        Debug.Log("¡¡DERROTA!! El enemigo tocó físicamente al Fisicoculturista. Fin del juego.");
    }

    private void Victoria()
    {
        juegoTerminado = true;
        Debug.Log("¡¡VICTORIA!! El enemigo fue desintegrado antes de llegar.");
    }
}