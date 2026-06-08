using UnityEngine;

public class Enemigo : MonoBehaviour
{
    [Header("Estadísticas de Vida")]
    public float vidaMax;
    public float vidaActual;

    void Start()
    {
        // Al arrancar, elegimos una vida al azar entre 1000 y 1250 (usamos 1251f porque el rango máximo en números enteros es exclusivo)
        vidaMax = Random.Range(1000, 1251);
        vidaActual = vidaMax;

        Debug.Log($"¡Enemigo spawneado con {vidaActual} puntos de vida!");
    }

    // Esta función pública la llamará la bala cuando impacte contra el círculo rojo
    public void RecibirDano(float cantidadDano)
    {
        vidaActual -= cantidadDano;
        Debug.Log($"¡Enemigo golpeado! Recibió {cantidadDano} de daño. Vida restante: {vidaActual}/{vidaMax}");

        // Si la vida llega a cero o menos, el enemigo muere
        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Debug.Log("¡El enemigo ha sido derrotado!");
        Destroy(gameObject); // Desaparece de la escena
    }
}