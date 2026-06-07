using UnityEngine;

public class ItemArma : MonoBehaviour
{
    [Header("Dimensiones en la cuadricula")]
    public int ancho = 2;
    public int alto = 1;

    [HideInInspector] public int filaActual;
    [HideInInspector] public int columnaActual;

    [HideInInspector] public bool estaEnCuadriceula = false;
}
