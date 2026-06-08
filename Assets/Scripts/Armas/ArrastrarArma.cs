using UnityEngine;
using UnityEngine.InputSystem;

public class ArrastrarArma : MonoBehaviour
{
    private bool seEstaArrastrando = false;
    private Vector3 desajustePosicion;
    private Vector3 posicionInicialFuera;
    private Camera camaraPrincipal;
    private GridManager gridManager;
    private ItemArma componenteArma;

    // Nueva referencia para encontrar el control del turno y revisar el botón
    private ControlTurno controlTurno;

    void Start()
    {
        camaraPrincipal = Camera.main;
        gridManager = FindAnyObjectByType<GridManager>();
        componenteArma = GetComponent<ItemArma>();

        // Buscamos el script que maneja el turno en la escena
        controlTurno = FindAnyObjectByType<ControlTurno>();

        posicionInicialFuera = transform.position;
        posicionInicialFuera.z = -2f;
        transform.position = posicionInicialFuera;
    }

    void OnMouseDown()
    {
        // --- NUEVA REGLA DE BLOQUEO DE TURNO ---
        // Si ya presionamos Iniciar, el botón se desactivó. 
        // Si el botón está apagado, bloqueamos por completo el arrastre del arma.
        if (controlTurno != null && controlTurno.botonIniciarUI != null)
        {
            if (!controlTurno.botonIniciarUI.gameObject.activeSelf)
            {
                Debug.LogWarning("¡El turno ya inició! Las armas están congeladas en su lugar.");
                return; // Corta la función acá y no te deja arrastrar nada
            }
        }

        seEstaArrastrando = true;

        if (componenteArma != null && componenteArma.estaEnCuadriceula && gridManager != null)
        {
            gridManager.LiberarEspacio(componenteArma);
        }

        Vector2 mousePosActual = Mouse.current.position.ReadValue();
        Vector3 posicionMouse = camaraPrincipal.ScreenToWorldPoint(mousePosActual);

        desajustePosicion = transform.position - posicionMouse;
        desajustePosicion.z = 0;
    }

    void OnMouseDrag()
    {
        // Si la regla de arriba te bloqueó, esto no se ejecutará
        if (!seEstaArrastrando) return;

        Vector2 mousePosActual = Mouse.current.position.ReadValue();
        Vector3 posicionMouse = camaraPrincipal.ScreenToWorldPoint(mousePosActual);

        Vector3 nuevaPosicion = posicionMouse + desajustePosicion;
        nuevaPosicion.z = -2f;

        transform.position = nuevaPosicion;
    }

    void OnMouseUp()
    {
        // Si no se estaba arrastrando (porque estaba bloqueado), ignoramos el soltar
        if (!seEstaArrastrando) return;

        seEstaArrastrando = false;

        if (gridManager == null || componenteArma == null) return;

        int filaMasCercana = -1;
        int columnaMasCercana = -1;
        float distanciaMinima = float.MaxValue;

        for (int f = 0; f < 5; f++)
        {
            for (int c = 0; c < 5; c++)
            {
                int indice = (f * 5) + c;
                Transform casillero = gridManager.casillerosVisuales[indice];

                if (casillero != null)
                {
                    float distancia = Vector2.Distance(transform.position, casillero.position);

                    if (distancia < distanciaMinima)
                    {
                        distanciaMinima = distancia;
                        filaMasCercana = f;
                        columnaMasCercana = c;
                    }
                }
            }
        }

        if (distanciaMinima < 1.5f)
        {
            bool exito = gridManager.ColocarArma(componenteArma, filaMasCercana, columnaMasCercana);

            if (!exito)
            {
                RegresarALaBase();
            }
        }
        else
        {
            RegresarALaBase();
        }
    }

    private void RegresarALaBase()
    {
        transform.position = posicionInicialFuera;
        if (componenteArma != null) componenteArma.estaEnCuadriceula = false;
    }
}