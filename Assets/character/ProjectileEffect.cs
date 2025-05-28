using UnityEngine;

public class ProjectileEffect : MonoBehaviour
{
    [Header("Configurações de Dano")]
    public int damageAmount = 10;
    
    [Header("Configurações de Mana")]
    public int manaToRestore = 0; // Padrão 0 para projéteis normais
    
    [Header("Efeitos")]
    public bool destroyOnHit = true;
    public GameObject hitEffect;
    public GameObject manaEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Sistema de dano
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && damageAmount > 0)
            {
                playerHealth.TakeDamage(damageAmount);
                
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, transform.position, Quaternion.identity);
                }
            }

            // Sistema de mana
            ManaSystem manaSystem = other.GetComponentInChildren<ManaSystem>();
            if (manaSystem != null && manaToRestore > 0)
            {
                manaSystem.RestoreMana(manaToRestore);
                
                if (manaEffect != null)
                {
                    Instantiate(manaEffect, transform.position, Quaternion.identity);
                }
            }

            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
        else if (!other.isTrigger)
        {
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}