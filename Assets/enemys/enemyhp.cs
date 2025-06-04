using UnityEngine;
using System; // Adicionado para usar Action

public class EnemyHP : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private AudioClip deathSound;

    // Evento adicionado
    public event Action OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Inimigo atingido! Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Inimigo derrotado!");

        // Dispara o evento antes de destruir o objeto
        OnDeath?.Invoke();

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        Destroy(gameObject);
    }

    public void DebugHealth()
    {
        Debug.Log($"Vida atual: {currentHealth}/{maxHealth}");
    }
}