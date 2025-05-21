using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Configurações do Projétil")]
    public int damage = 10; // Alterado para public
    public float speed = 15f; // Alterado para public
    [SerializeField] private float lifetime = 3f;
    
    [Header("Efeitos")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private AudioClip hitSound;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            EnemyHP enemy = other.GetComponent<EnemyHP>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        
        if(!other.CompareTag("Player") && !other.CompareTag("Projectile"))
        {
            if(hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            
            if(hitSound != null)
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
            
            Destroy(gameObject);
        }
    }
}