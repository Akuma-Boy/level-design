using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private AudioClip deathSound;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Inimigo atingido! Vida restante: {currentHealth}"); // Log para debug
        
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Inimigo derrotado!"); // Log para debug
        
        if(deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        
        if(deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        
        Destroy(gameObject);
    }

    // Método para debug - mostra a vida atual no console
    public void DebugHealth()
    {
        Debug.Log($"Vida atual: {currentHealth}/{maxHealth}");
    }
}