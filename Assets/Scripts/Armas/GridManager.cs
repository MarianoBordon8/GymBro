using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Referencias de la Cuadrícula")]
    public Transform[] casillerosVisuales = new Transform[25];
    private bool[,] matrizOcupacion = new bool[5, 5];

    [Header("Referencias del Jugador")]
    // Aquí conectaremos el objeto "Arma_Equipada_Visual" que acabamos de crear
    public GameObject armaEnMano;

    private int ObtenerIndiceCasillero(int fila, int columna)
    {
        return (fila * 5) + columna;
    }

    public bool VerificarEspacioDisponible(int filaInicio, int columnaInicio, int anchoArma, int altoArma)
    {
        if (filaInicio + altoArma > 5 || columnaInicio + anchoArma > 5) return false;

        for (int f = filaInicio; f < filaInicio + altoArma; f++)
        {
            for (int c = columnaInicio; c < columnaInicio + anchoArma; c++)
            {
                if (matrizOcupacion[f, c]) return false;
            }
        }
        return true;
    }

    public bool ColocarArma(ItemArma arma, int filaInicio, int columnaInicio)
    {
        if (!VerificarEspacioDisponible(filaInicio, columnaInicio, arma.ancho, arma.alto))
        {
            Debug.LogWarning("¡Espacio inválido o ya ocupado!");
            return false;
        }

        Vector3 posicionCentro = Vector3.zero;
        int celdasOcupadas = 0;

        for (int f = filaInicio; f < filaInicio + arma.alto; f++)
        {
            for (int c = columnaInicio; c < columnaInicio + arma.ancho; c++)
            {
                int indice = ObtenerIndiceCasillero(f, c);
                posicionCentro += casillerosVisuales[indice].position;
                celdasOcupadas++;
                matrizOcupacion[f, c] = true;
            }
        }

        posicionCentro /= celdasOcupadas;
        posicionCentro.z = -2f; // Mantenemos el arma del inventario en Z = -2 por encima del tablero

        arma.transform.position = posicionCentro;

        arma.filaActual = filaInicio;
        arma.columnaActual = columnaInicio;
        arma.estaEnCuadriceula = true;

        // REGLA 2: Si acomoda el arma con éxito, el arma de la mano se activa
        if (armaEnMano != null)
        {
            armaEnMano.SetActive(true);
            Debug.Log("¡Arma visible en la mano del Fisicoculturista!");
        }

        return true;
    }

    public void LiberarEspacio(ItemArma arma)
    {
        for (int f = arma.filaActual; f < arma.filaActual + arma.alto; f++)
        {
            for (int c = arma.columnaActual; c < arma.columnaActual + arma.ancho; c++)
            {
                matrizOcupacion[f, c] = false;
            }
        }

        arma.estaEnCuadriceula = false;

        // REGLA 3: Si levanta el arma o la saca, el arma de la mano se desactiva
        if (armaEnMano != null)
        {
            armaEnMano.SetActive(false);
            Debug.Log("Arma de la mano oculta.");
        }
    }
}