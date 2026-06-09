using UnityEngine;

public class BalaBomba : MonoBehaviour
{
    public float velocidadBomba = 6f;
    public float radioExplosion = 2.5f; // Qué tan grande es el área de daño
    public float danoArea = 500f;       // Daño fijo o base para la explosión

    private Rigidbody2D rb;
    private bool yaExploto = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Buscamos al enemigo más cercano para apuntar el tiro
        Enemigo[] todosLosEnemigos = FindObjectsByType<Enemigo>();
        Enemigo enemigoMasCercano = null;
        float distanciaMinima = float.MaxValue;

        foreach (Enemigo e in todosLosEnemigos)
        {
            float distancia = Vector3.Distance(transform.position, e.transform.position);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                enemigoMasCercano = e;
            }
        }

        if (enemigoMasCercano != null)
        {
            Vector3 direccion = enemigoMasCercano.transform.position - transform.position;
            direccion.Normalize();
            rb.linearVelocity = direccion * velocidadBomba;
        }
        else
        {
            rb.linearVelocity = transform.right * velocidadBomba;
        }

        Destroy(gameObject, 5f); // Seguro por si falla el tiro
    }

    void OnTriggerEnter2D(Collider2D otroObjeto)
    {
        if (yaExploto) return;

        // Si tocamos a cualquier enemigo, detonamos la bomba
        if (otroObjeto.GetComponent<Enemigo>() != null)
        {
            Explotar();
        }
    }

    private void Explotar()
    {
        yaExploto = true;
        Debug.Log("<color=orange>¡¡BOOM!! La bomba detonó.</color>");

        // Lanzamos un círculo invisible en la física 2D para detectar a quiénes agarra la explosión
        Collider2D[] objetosAlcanzados = Physics2D.OverlapCircleAll(transform.position, radioExplosion);

        foreach (Collider2D col in objetosAlcanzados)
        {
            Enemigo enemigoEnArea = col.GetComponent<Enemigo>();
            if (enemigoEnArea != null)
            {
                // Le aplicamos el daño en área a cada uno
                enemigoEnArea.RecibirDano(danoArea);
            }
        }

        // Le avisamos al GameController que el disparo de esta arma ya terminó
        if (GameController.Instancia != null)
        {
            GameController.Instancia.FinalizarDisparoJugador();
        }

        Destroy(gameObject); // Desaparece la bomba
    }

    // Esto te permite ver el tamaño real de tu explosión en la ventana "Scene" de Unity como un círculo rojo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioExplosion);
    }
}