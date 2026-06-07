using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Los 25 casilleros ordenados del 0 al 24 en el Inspector
    public Transform[] casillerosVisuales = new Transform[25];

    // Matriz lógica de 5x5
    private bool[,] matrizOcupacion = new bool[5, 5];

    // Función para convertir Fila y Columna (0 a 4) en el índice plano (0 a 24)
    private int ObtenerIndiceCasillero(int fila, int columna)
    {
        return (fila * 5) + columna;
    }

    // NUEVA FUNCIÓN: Coloca el arma automáticamente en una fila y columna inicial
    public bool ColocarArma(ItemArma arma, int filaInicio, int columnaInicio)
    {
        // 1. Validar límites de la cuadrícula de 5x5
        if (filaInicio + arma.alto > 5 || columnaInicio + arma.ancho > 5)
        {
            Debug.LogWarning("¡El arma no entra en esa posición, se sale de la cuadrícula!");
            return false;
        }

        // 2. Verificar si el espacio está libre
        for (int f = filaInicio; f < filaInicio + arma.alto; f++)
        {
            for (int c = columnaInicio; c < columnaInicio + arma.ancho; c++)
            {
                if (matrizOcupacion[f, c])
                {
                    Debug.LogWarning("¡Espacio ocupado por otra arma!");
                    return false;
                }
            }
        }

        // 3. CALCULAR LA POSICIÓN VISUAL EXACTA
        // Para que un arma de 1x2 quede bien centrada entre dos casilleros,
        // sumamos las posiciones de todos los casilleros que ocupa y sacamos el promedio (el centro).
        Vector3 posicionCentro = Vector3.zero;
        int celdasOcupadas = 0;

        for (int f = filaInicio; f < filaInicio + arma.alto; f++)
        {
            for (int c = columnaInicio; c < columnaInicio + arma.ancho; c++)
            {
                int indice = ObtenerIndiceCasillero(f, c);
                posicionCentro += casillerosVisuales[indice].position;
                celdasOcupadas++;

                // Marcamos la matriz como ocupada
                matrizOcupacion[f, c] = true;
            }
        }

        // Dividimos la suma por el total de celdas para obtener el centro real en el mundo 2D
        posicionCentro /= celdasOcupadas;

        // Mantenemos el arma un poquito al frente en el eje Z para que no quede detrás del fondo
        posicionCentro.z = -0.1f;

        // 4. Mover el arma físicamente a ese centro calculando
        arma.transform.position = posicionCentro;

        // Guardar sus coordenadas lógicas en el arma
        arma.filaActual = filaInicio;
        arma.columnaActual = columnaInicio;

        Debug.Log($"Arma colocada con éxito en la Fila {filaInicio}, Columna {columnaInicio}");
        return true;
    }
    // ESTO ES SOLO PARA PRUEBAS (LUEGO SE QUITA)
    [Header("Prueba Automática")]
    public ItemArma armaParaTestear;
    public int testFila = 0;    // 0 es la primera fila de arriba
    public int testColumna = 0; // 0 es la primera columna de la izquierda

    void Start()
    {
        if (armaParaTestear != null)
        {
            // Intentará colocar el arma en la posición designada al arrancar
            ColocarArma(armaParaTestear, testFila, testColumna);
        }
    }
}