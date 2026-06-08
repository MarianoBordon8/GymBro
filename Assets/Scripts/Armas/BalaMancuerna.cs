using UnityEngine;

public class BalaMancuerna : MonoBehaviour
{
    public float velocidadBala = 8f; // Qué tan rápido vuela
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Le aplicamos una fuerza hacia la derecha en cuanto aparece
        rb.linearVelocity = transform.right * velocidadBala;

        // Autodestruimos la bala después de 5 segundos para que no llene la memoria de la computadora si no choca con nada
        Destroy(gameObject, 5f);
    }
}