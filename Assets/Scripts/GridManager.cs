using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Transform[] casillerosVisuales = new Transform[25];

    private bool[,] matrizOcupacion = new bool[5, 5];

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
}