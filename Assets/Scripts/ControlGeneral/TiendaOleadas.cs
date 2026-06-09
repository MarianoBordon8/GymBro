using UnityEngine;
using System.Collections.Generic;

public class TiendaOleadas : MonoBehaviour
{
    public static TiendaOleadas Instancia { get; private set; }

    [Header("Catálogo de Armas Existentes")]
    // Aquí arrastrarás todos los prefabs de armas que existan en tu juego (Bomba, Mancuerna, etc.)
    public List<GameObject> listaTodasLasArmas = new List<GameObject>();

    [Header("Puntos de Spawneo de Ofertas")]
    // Tres objetos vacíos abajo del tablero que marquen dónde aparecen las 3 opciones
    public Transform[] posicionesTienda = new Transform[3];

    [Header("Estado de la Ronda")]
    public bool yaComproEnEstaRonda = false;

    // Lista para controlar los 3 clones que están actualmente en oferta
    private List<GameObject> armasOfertadasActuales = new List<GameObject>();

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        GenerarNuevasOfertas();
    }

    // Limpia la tienda vieja y saca 3 armas aleatorias del catálogo
    public void GenerarNuevasOfertas()
    {
        // 1. Destruimos las ofertas de la horda anterior que no se usaron
        foreach (GameObject armaVieja in armasOfertadasActuales)
        {
            if (armaVieja != null && !armaVieja.GetComponent<ItemArma>().estaEnCuadriceula)
            {
                Destroy(armaVieja);
            }
        }
        armasOfertadasActuales.Clear();

        yaComproEnEstaRonda = false; // Abrimos el cerrojo para la nueva oleada

        if (listaTodasLasArmas.Count == 0 || posicionesTienda.Length < 3) return;

        // 2. Elegimos 3 armas al azar (pueden repetirse o no, según el tamaño de tu catálogo)
        for (int i = 0; i < 3; i++)
        {
            if (posicionesTienda[i] == null) continue;

            int indiceAleatorio = Random.Range(0, listaTodasLasArmas.Count);
            GameObject prefabElegido = listaTodasLasArmas[indiceAleatorio];

            // Creamos el arma físicamente en la posición de la tienda
            GameObject nuevaOferta = Instantiate(prefabElegido, posicionesTienda[i].position, Quaternion.identity);

            // Nos aseguramos de que sepa que NO está en la cuadrícula todavía
            ItemArma item = nuevaOferta.GetComponent<ItemArma>();
            if (item != null) item.estaEnCuadriceula = false;

            armasOfertadasActuales.Add(nuevaOferta);
        }
        Debug.Log("<color=purple>¡Tienda actualizada con 3 armas aleatorias!</color>");
    }

    // El script ArrastrarArma llamará a esto para validar si puede poner el arma o no
    public bool IntentarEquiparArmaDeTienda(GameObject armaObj)
    {
        // Si pertenece a las ofertas actuales y ya gastó su elección, rebota el arrastre
        if (armasOfertadasActuales.Contains(armaObj) && yaComproEnEstaRonda)
        {
            Debug.LogWarning("¡Ya añadiste un arma en esta oleada! No puedes agregar más.");
            return false;
        }
        return true;
    }

    public void ConfirmarCompraDeArma(GameObject armaObj)
    {
        if (armasOfertadasActuales.Contains(armaObj))
        {
            yaComproEnEstaRonda = true; // Cerramos el cerrojo de compra
            Debug.Log("<color=magenta>Arma de la tienda equipada con éxito. Compra bloqueada hasta la próxima ola.</color>");

            // Opcional: Desvanecer o destruir las otras dos que no eligió de inmediato
            DesactivarOfertasRestantes();
        }
    }

    private void DesactivarOfertasRestantes()
    {
        foreach (GameObject oferta in armasOfertadasActuales)
        {
            if (oferta != null)
            {
                ItemArma item = oferta.GetComponent<ItemArma>();
                // Si la oferta no es la que se metió a la cuadrícula, la borramos
                if (item != null && !item.estaEnCuadriceula)
                {
                    // Le ponemos un color gris translúcido o directamente la borramos
                    Destroy(oferta);
                }
            }
        }
    }
}