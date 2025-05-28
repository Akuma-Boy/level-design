using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ManaPickup : MonoBehaviour
{
    public int manaAmount = 25;
    public AudioClip pickupSound;
    public GameObject pickupEffect;
    public float rotateSpeed = 90f; // graus por segundo

    private void Update()
    {
        // Faz o objeto girar constantemente
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        ManaSystem manaSystem = other.GetComponent<ManaSystem>() ?? other.GetComponentInParent<ManaSystem>();

        if (manaSystem != null)
        {
            manaSystem.RestoreMana(manaAmount);

            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
