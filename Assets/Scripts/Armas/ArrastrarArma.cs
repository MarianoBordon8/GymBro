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

    void Start()
    {
        camaraPrincipal = Camera.main;
        gridManager = FindAnyObjectByType<GridManager>();
        componenteArma = GetComponent<ItemArma>();

        posicionInicialFuera = transform.position;
        posicionInicialFuera.z = -2f;
        transform.position = posicionInicialFuera;
    }

    void OnMouseDown()
    {
        seEstaArrastrando = true;

        // --- NUEVA LÓGICA DE LIBERACIÓN ---
        // Si el arma ya estaba en la cuadrícula y la volvemos a agarrar, liberamos sus casilleros viejos
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
        if (!seEstaArrastrando) return;

        Vector2 mousePosActual = Mouse.current.position.ReadValue();
        Vector3 posicionMouse = camaraPrincipal.ScreenToWorldPoint(mousePosActual);

        Vector3 nuevaPosicion = posicionMouse + desajustePosicion;
        nuevaPosicion.z = -2f;

        transform.position = nuevaPosicion;
    }

    void OnMouseUp()
    {
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

            // Si no se pudo colocar en el nuevo lugar, regresa a su base fuera del inventario
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
        Debug.Log("Regresando el arma a su posición base inicial.");
        transform.position = posicionInicialFuera;

        // Nos aseguramos de que el arma sepa que ya no está en la cuadrícula si volvió a la base
        if (componenteArma != null) componenteArma.estaEnCuadriceula = false;
    }
}