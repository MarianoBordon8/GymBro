using UnityEngine;

public class ItemArma : MonoBehaviour
{
    public enum TipoDeArma { Mancuerna, Bomba }

    [Header("Configuración de Tipo")]
    public TipoDeArma tipoArma;

    [Header("Prefabs de Proyectiles (Balas)")]
    // Cada arma ahora guarda su propio proyectil correspondiente
    public GameObject proyectilPrefab;

    [Header("Dimensiones en la cuadrícula")]
    public int ancho = 2;
    public int alto = 1;

    [HideInInspector] public int filaActual;
    [HideInInspector] public int columnaActual;
    [HideInInspector] public bool estaEnCuadriceula = false;

    // --- NUEVA FUNCIÓN: El arma real ejecuta su propio disparo ---
    public GameObject EjecutarDisparo(Transform puntoDisparo)
    {
        if (!estaEnCuadriceula) return null;

        if (proyectilPrefab != null && puntoDisparo != null)
        {
            // Clonamos la bala real en la escena
            GameObject balaClon = Instantiate(proyectilPrefab, puntoDisparo.position, puntoDisparo.rotation);

            Debug.Log($"<color=cyan>¡{gameObject.name} creó proyectil en escena!</color>");

            // Devolvemos el clon directo al ControlTurno para que lo vigile
            return balaClon;
        }

        return null;
    }
}