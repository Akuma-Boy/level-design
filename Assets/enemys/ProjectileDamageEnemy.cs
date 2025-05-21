using UnityEngine;

public class ProjectileDamageEnemy : MonoBehaviour
{
    [Header("Configurações de Dano")]
    public int damageAmount = 10;
    public bool destroyOnHit = true;
    public GameObject hitEffect;

    void OnTriggerEnter(Collider other)
    {
        // Para jogos 2D, use OnTriggerEnter2D(Collider2D other)

        // Verifica se atingiu o player
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            // Efeito de hit (opcional)
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            // Destroi o projétil
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
        // Destroi se atingir qualquer coisa (opcional)
        else if (!other.isTrigger) // Ignora triggers
        {
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}