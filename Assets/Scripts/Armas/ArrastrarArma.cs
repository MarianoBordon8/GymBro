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
    private ControlTurno controlTurno;

    void Start()
    {
        camaraPrincipal = Camera.main;
        gridManager = FindAnyObjectByType<GridManager>();
        componenteArma = GetComponent<ItemArma>();
        controlTurno = FindAnyObjectByType<ControlTurno>();

        // Guardamos la posición exacta del inventario que le pusiste en el editor de Unity
        posicionInicialFuera = transform.position;
    }

    void OnMouseDown()
    {
        // Bloqueo de turno
        if (controlTurno != null && controlTurno.botonIniciarUI != null)
        {
            if (!controlTurno.botonIniciarUI.gameObject.activeSelf)
            {
                Debug.LogWarning("¡El turno ya inició! Las armas están congeladas.");
                return;
            }
        }

        seEstaArrastrando = true;

        // Si ya estaba en la cuadrícula, liberamos el espacio para poder moverla
        if (componenteArma != null && componenteArma.estaEnCuadriceula && gridManager != null)
        {
            gridManager.LiberarEspacio(componenteArma);
        }

        // Calculamos la posición del mouse de forma inmediata
        Vector2 mousePosActual = Mouse.current.position.ReadValue();
        Vector3 posicionMouse = camaraPrincipal.ScreenToWorldPoint(mousePosActual);

        // Mantenemos el mismo eje Z actual del objeto para que no se desfase
        desajustePosicion = transform.position - posicionMouse;
        desajustePosicion.z = 0;
    }

    void OnMouseDrag()
    {
        if (!seEstaArrastrando) return;

        Vector2 mousePosActual = Mouse.current.position.ReadValue();
        Vector3 posicionMouse = camaraPrincipal.ScreenToWorldPoint(mousePosActual);

        Vector3 nuevaPosicion = posicionMouse + desajustePosicion;

        // Mantenemos el Z original del objeto para evitar que se meta detrás del fondo
        nuevaPosicion.z = posicionInicialFuera.z;

        transform.position = nuevaPosicion;
    }

    void OnMouseUp()
    {
        if (!seEstaArrastrando) return;

        seEstaArrastrando = false;

        if (gridManager == null || componenteArma == null)
        {
            RegresarALaBase();
            return;
        }

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
            // --- NUEVO: VALIDACIÓN DE LA TIENDA ---
            if (TiendaOleadas.Instancia != null)
            {
                // Si la tienda dice que ya compraste, te obliga a regresar a la base de abajo
                if (!TiendaOleadas.Instancia.IntentarEquiparArmaDeTienda(gameObject))
                {
                    RegresarALaBase();
                    return;
                }
            }

            bool exito = gridManager.ColocarArma(componenteArma, filaMasCercana, columnaMasCercana);

            if (!exito)
            {
                RegresarALaBase();
            }
            else
            {
                Vector3 posFijada = transform.position;
                posFijada.z = posicionInicialFuera.z;
                transform.position = posFijada;

                // --- NUEVO: NOTIFICAR COMPRA EXITOSA ---
                if (TiendaOleadas.Instancia != null)
                {
                    TiendaOleadas.Instancia.ConfirmarCompraDeArma(gameObject);
                }
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