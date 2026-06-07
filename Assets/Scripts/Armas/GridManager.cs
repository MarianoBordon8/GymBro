using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Transform[] casillerosVisuales = new Transform[25];
    private bool[,] matrizOcupacion = new bool[5, 5];

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

                // Ocupamos la celda
                matrizOcupacion[f, c] = true;
            }
        }

        posicionCentro /= celdasOcupadas;
        posicionCentro.z = -2f;

        arma.transform.position = posicionCentro;

        // El arma ahora recuerda con éxito sus coordenadas actuales
        arma.filaActual = filaInicio;
        arma.columnaActual = columnaInicio;
        arma.estaEnCuadriceula = true;

        Debug.Log($"Arma colocada en Fila {filaInicio}, Columna {columnaInicio}");
        return true;
    }

    // --- NUEVA FUNCIÓN: LIBERAR CASILLEROS ---
    public void LiberarEspacio(ItemArma arma)
    {
        // Recorremos las celdas que ocupaba el arma según sus coordenadas guardadas
        for (int f = arma.filaActual; f < arma.filaActual + arma.alto; f++)
        {
            for (int c = arma.columnaActual; c < arma.columnaActual + arma.ancho; c++)
            {
                // Las volvemos a poner en false (vacías)
                matrizOcupacion[f, c] = false;
            }
        }

        arma.estaEnCuadriceula = false;
        Debug.Log($"Casilleros liberados desde Fila {arma.filaActual}, Columna {arma.columnaActual}");
    }
}