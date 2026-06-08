using UnityEngine;

public class BalaMancuerna : MonoBehaviour
{
    public float velocidadBala = 8f;
    private Rigidbody2D rb;
    private bool yaGolpeo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 1. DIRIGIR LA BALA HACIA EL ENEMIGO
        // Buscamos al enemigo en la escena usando el script que le acabamos de poner
        Enemigo objetivoEnemigo = FindAnyObjectByType<Enemigo>();

        if (objetivoEnemigo != null)
        {
            // Calculamos el vector de dirección: Restamos (Posición Destino - Posición Origen)
            Vector3 direccion = objetivoEnemigo.transform.position - transform.position;
            direccion.Normalize(); // Normalizar hace que el vector mida 1, manteniendo solo la dirección pura

            // Hacemos que la bala rote visualmente para "mirar" al enemigo
            float anguloZ = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, anguloZ);

            // Le aplicamos la velocidad física en esa dirección exacta hacia el objetivo
            rb.linearVelocity = direccion * velocidadBala;
        }
        else
        {
            // Si por alguna razón no hay enemigo en el mapa, dispara recto hacia la derecha por defecto
            rb.linearVelocity = transform.right * velocidadBala;
        }

        Destroy(gameObject, 5f); // Seguro de vida por si erra
    }

    // 2. DETECTAR EL IMPACTO Y APLICAR DAÑO ALEATORIO
    // Esta función de Unity se ejecuta sola cuando el Collider entra en el Trigger del enemigo
    void OnTriggerEnter2D(Collider2D otroObjeto)
    {
        // Si esta bala ya registró un golpe en este frame, ignoramos cualquier otro choque
        if (yaGolpeo) return;

        Enemigo enemigoGolpeado = otroObjeto.GetComponent<Enemigo>();

        if (enemigoGolpeado != null)
        {
            yaGolpeo = true; // Cerramos el cerrojo inmediatamente

            float danoAleatorio = Random.Range(250, 401);
            enemigoGolpeado.RecibirDano(danoAleatorio);

            if (GameController.Instancia != null)
            {
                GameController.Instancia.FinalizarDisparoJugador();
            }

            Destroy(gameObject);
        }
    }
}