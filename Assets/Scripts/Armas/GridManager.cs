using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Referencias de la Cuadrícula")]
    public Transform[] casillerosVisuales = new Transform[25];
    private bool[,] matrizOcupacion = new bool[5, 5];

    [Header("Arma Principal Obligatoria")]
    // Arrastrá acá tu mancuerna de la escena para que el tablero la acomode al iniciar
    public ItemArma mancuernaPrincipal;
    public int filaInicialMancuerna = 0;    // Fila donde querés que aparezca (0 a 4)
    public int columnaInicialMancuerna = 0; // Columna donde querés que aparezca (0 a 4)

    [Header("Referencias del Jugador (Opcional)")]
    public GameObject armaEnMano;

    void Start()
    {
        // --- NUEVO: AUTO-COLOCACIÓN AL INICIAR EL JUEGO ---
        if (mancuernaPrincipal != null)
        {
            // Forzamos al tablero a colocar la mancuerna en la posición inicial asignada
            bool colocadoExitoso = ColocarArma(mancuernaPrincipal, filaInicialMancuerna, columnaInicialMancuerna);

            if (colocadoExitoso)
            {
                Debug.Log($"<color=green>¡Mancuerna auto-colocada con éxito en la posición [{filaInicialMancuerna},{columnaInicialMancuerna}]!</color>");
            }
            else
            {
                Debug.LogError("No se pudo auto-colocar la mancuerna. Revisa que las filas/columnas iniciales estén dentro del tablero 5x5.");
            }
        }
    }

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
        posicionCentro.z = -2f; // Mantenemos el arma en Z = -2 para que se vea sobre el tablero

        arma.transform.position = posicionCentro;

        arma.filaActual = filaInicio;
        arma.columnaActual = columnaInicio;
        arma.estaEnCuadriceula = true;

        if (armaEnMano != null)
        {
            armaEnMano.SetActive(true);
        }

        return true;
    }

    public void LiberarEspacio(ItemArma arma)
    {
        if (arma.estaEnCuadriceula)
        {
            for (int f = arma.filaActual; f < arma.filaActual + arma.alto; f++)
            {
                for (int c = arma.columnaActual; c < arma.columnaActual + arma.ancho; c++)
                {
                    matrizOcupacion[f, c] = false;
                }
            }
        }

        arma.estaEnCuadriceula = false;
    }
}