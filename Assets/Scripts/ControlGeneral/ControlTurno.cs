using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ControlTurno : MonoBehaviour
{
    [Header("Referencias del Combate")]
    public Transform puntoDisparo;
    public Button botonIniciarUI;
    public ArrastrarArma scriptArrastreArma;

    [Header("Contenedor de Armas Activas")]
    public Transform contenedorArmas;

    private Queue<ItemArma> colaDeDisparos = new Queue<ItemArma>();

    public void PresionarIniciar()
    {
        ItemArma[] todasLasArmas = FindObjectsByType<ItemArma>();
        bool mancuernaEstaEquipada = false;

        foreach (ItemArma arma in todasLasArmas)
        {
            if (arma.tipoArma == ItemArma.TipoDeArma.Mancuerna && arma.estaEnCuadriceula)
            {
                mancuernaEstaEquipada = true;
                break;
            }
        }

        if (!mancuernaEstaEquipada)
        {
            Debug.LogWarning("<color=yellow>¡No puedes iniciar! La MANCUERNA debe estar colocada en el tablero.</color>");
            return;
        }

        ArrastrarArma[] scriptsArrastre = FindObjectsByType<ArrastrarArma>();
        foreach (ArrastrarArma arrastre in scriptsArrastre)
        {
            arrastre.enabled = false;
        }

        if (botonIniciarUI != null) botonIniciarUI.gameObject.SetActive(false);

        if (GameController.Instancia != null)
        {
            GameController.Instancia.IniciarCombateDeOleada();
        }

        CalcularYPlanificarTurno();
    }

    public void CalcularYPlanificarTurno()
    {
        colaDeDisparos.Clear();

        ItemArma[] armasEnEscena = FindObjectsByType<ItemArma>();
        List<ItemArma> bombasA_Disparar = new List<ItemArma>();
        List<ItemArma> mancuernasA_Disparar = new List<ItemArma>();

        foreach (ItemArma arma in armasEnEscena)
        {
            if (arma.estaEnCuadriceula)
            {
                if (contenedorArmas != null)
                {
                    arma.transform.SetParent(contenedorArmas);
                }

                if (arma.tipoArma == ItemArma.TipoDeArma.Bomba)
                    bombasA_Disparar.Add(arma);
                else if (arma.tipoArma == ItemArma.TipoDeArma.Mancuerna)
                    mancuernasA_Disparar.Add(arma);
            }
            else
            {
                arma.transform.SetParent(null);
            }
        }

        foreach (ItemArma bomba in bombasA_Disparar) colaDeDisparos.Enqueue(bomba);
        foreach (ItemArma mancuerna in mancuernasA_Disparar) colaDeDisparos.Enqueue(mancuerna);

        DispararBalaAutomatica();
    }

    public void DispararBalaAutomatica()
    {
        if (GameController.Instancia != null && GameController.Instancia.juegoTerminado) return;

        if (colaDeDisparos.Count > 0)
        {
            StartCoroutine(RutinaRafagaEscalonada());
        }
        else
        {
            if (GameController.Instancia != null) GameController.Instancia.FinalizarDisparoJugador();
        }
    }

    // --- CORRECCIÓN CRUCIAL: RÁFAGA RÍTMICA CONTROLADA ---
    IEnumerator RutinaRafagaEscalonada()
    {
        while (colaDeDisparos.Count > 0)
        {
            if (GameController.Instancia != null && GameController.Instancia.juegoTerminado) yield break;

            if (puntoDisparo != null)
            {
                ItemArma armaAtacante = colaDeDisparos.Dequeue();

                if (armaAtacante != null)
                {
                    // --- NUEVO: Atrapamos el clon que nos devuelve el ItemArma ---
                    GameObject balaCreada = armaAtacante.EjecutarDisparo(puntoDisparo);

                    // ESPERA INTELIGENTE 1: Mientras el clon de la bala exista físicamente en la escena,
                    // el ControlTurno detiene el tiempo y no permite que avance el código.
                    while (balaCreada != null)
                    {
                        yield return null; // Espera al próximo frame y vuelve a chequear
                    }

                    // ESPERA INTELIGENTE 2: La bala ya chocó y se destruyó. 
                    // Metemos un respiro obligatorio de medio segundo antes de habilitar el próximo gatillazo.
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        Debug.Log("<color=white>--- Ráfaga finalizada por completo. Ahora sí se moverá el enemigo. ---</color>");

        // REGLA DE ORO: La cola se vació por completo y todas las balas estallaron. 
        // Le damos el permiso oficial de caminar al enemigo.
        if (GameController.Instancia != null)
        {
            GameController.Instancia.FinalizarDisparoJugador();
        }
    }

    public void ResetearFilaDeDisparos()
    {
        colaDeDisparos.Clear();
    }
}