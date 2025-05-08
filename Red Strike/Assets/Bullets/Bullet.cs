using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5.0f; // Merminin ömrü (saniye)

    private void Start()
    {
        // Mermi ömrü dolduğunda yok olur
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Çarpışma olduğunda mermiyi yok et (isteğe bağlı)
        Destroy(gameObject);
    }
}