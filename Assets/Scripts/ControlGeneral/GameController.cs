using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController Instancia { get; private set; }

    [Header("Estados del Juego")]
    public bool esTurnoDelJugador = true;
    public bool juegoTerminado = false;

    [Header("Sistema de Oleadas (Waves)")]
    public GameObject enemigoPrefab;
    public Transform puntoSpawneo;
    public int oleadaActual = 0;

    [Header("Referencias de la Escena")]
    public ControlTurno controlTurno;
    public GridManager gridManager;
    public ArrastrarArma scriptArrastreArma;
    public GameObject fisicoculturista;

    private bool esperandoSiguienteOleada = false;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Debug.Log("--- JUEGO INICIADO ---");
        // --- REGLA 1: Spawnea al primer enemigo apenas carga el juego para que el jugador planee la Ola 1 ---
        SpawnearEnemigoDeOleada();
    }

    // Esta función ahora SOLO crea al enemigo en el mapa y le sube las estadísticas
    public void SpawnearEnemigoDeOleada()
    {
        if (juegoTerminado) return;

        oleadaActual++;
        Debug.Log($"<color=cyan>--- ¡CREANDO ENEMIGO DE OLEADA {oleadaActual}! ---</color>");

        if (enemigoPrefab != null && puntoSpawneo != null)
        {
            GameObject nuevoEnemigoClon = Instantiate(enemigoPrefab, puntoSpawneo.position, puntoSpawneo.rotation);

            Enemigo scriptEnemigo = nuevoEnemigoClon.GetComponent<Enemigo>();
            if (scriptEnemigo != null)
            {
                // Escalado de vida y dificultad según la oleada
                scriptEnemigo.vidaMax += (oleadaActual - 1) * 300f;
                scriptEnemigo.vidaActual = scriptEnemigo.vidaMax;
                scriptEnemigo.distanciaPaso += (oleadaActual - 1) * 0.05f;
            }
        }
    }

    // --- MODIFICADO: Esto es lo que ejecuta el botón INICIAR ahora ---
    public void IniciarCombateDeOleada()
    {
        esperandoSiguienteOleada = false; // Abrimos el cerrojo, el combate empieza oficialmente
        Debug.Log($"<color=orange>--- ¡COMBATE INICIADO EN OLEADA {oleadaActual}! ---</color>");
    }

    public void FinalizarDisparoJugador()
    {
        if (juegoTerminado) return;
        esTurnoDelJugador = false;
        StartCoroutine(RutinaTurnoEnemigo());
    }

    IEnumerator RutinaTurnoEnemigo()
    {
        yield return new WaitForSeconds(0.2f);

        Enemigo enemigo = FindAnyObjectByType<Enemigo>();
        if (enemigo != null && enemigo.vidaActual > 0)
        {
            enemigo.AvanzarUnTramo();
        }
    }

    public void FinalizarTurnoEnemigo()
    {
        if (juegoTerminado) return;
        StartCoroutine(RutinaSiguienteDisparoAutomatico());
    }

    IEnumerator RutinaSiguienteDisparoAutomatico()
    {
        // Pequeño delay de 0.6 segundos para que no sea todo instantáneo
        yield return new WaitForSeconds(0.6f);

        esTurnoDelJugador = true;

        if (controlTurno != null && !juegoTerminado && !esperandoSiguienteOleada)
        {
            // --- NUEVO: RECARGAMOS EL CARGADOR SEGÚN EL TABLERO ACTUAL ---
            // Le pedimos al ControlTurno que vuelva a escanear qué armas están puestas 
            // y cargue la colaDeDisparos de forma ordenada.
            controlTurno.CalcularYPlanificarTurno();

            // Una vez que el cargador está lleno, disparamos la primera bala automática del nuevo ciclo
            controlTurno.DispararBalaAutomatica();
        }
    }

    public void ReportarMuerteEnemigo()
    {
        if (juegoTerminado) return;
        if (esperandoSiguienteOleada) return;

        esperandoSiguienteOleada = true; // Cerramos el cerrojo para procesar la transición
        StartCoroutine(RutinaRegresarAFasePreparacion());
    }

    IEnumerator RutinaRegresarAFasePreparacion()
    {
        Debug.Log("<color=green>¡Enemigo eliminado! Preparando el siguiente rival de antemano...</color>");
        yield return new WaitForSeconds(1.0f);

        SpawnearEnemigoDeOleada();

        if (controlTurno != null && controlTurno.botonIniciarUI != null)
        {
            controlTurno.botonIniciarUI.gameObject.SetActive(true);
            var textoBoton = controlTurno.botonIniciarUI.GetComponentInChildren<TMPro.TMP_Text>();
            if (textoBoton != null) textoBoton.text = $"Iniciar Ola {oleadaActual}";
        }

        ArrastrarArma[] todasLasArmas = FindObjectsByType<ArrastrarArma>();
        foreach (ArrastrarArma arma in todasLasArmas)
        {
            arma.enabled = true;
        }

        if (controlTurno != null)
        {
            controlTurno.ResetearFilaDeDisparos();
        }

        // --- NUEVO: RENOVAR LA TIENDA DE ARMAS PARA LA PRÓXIMA RONDA ---
        if (TiendaOleadas.Instancia != null)
        {
            TiendaOleadas.Instancia.GenerarNuevasOfertas();
        }
    }

    public void DerrotaFisica()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        Debug.Log($"<color=red>¡¡GAME OVER!! Te aplastaron en la Oleada {oleadaActual}.</color>");
    }
}