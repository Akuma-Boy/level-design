using UnityEngine;

public class ManaRestoreOnHit : MonoBehaviour
{
    [Header("Configurações de Mana")]
    public int manaToRestore = 15; // Quantidade de mana a ser restaurada
    public bool destroyOnHit = true;
    public GameObject manaEffect; // Efeito visual opcional

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Tenta pegar o ManaSystem do player
            ManaSystem manaSystem = other.GetComponentInChildren<ManaSystem>();
            
            if (manaSystem != null)
            {
                manaSystem.RestoreMana(manaToRestore);
                
                // Efeito visual (opcional)
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
    }
}