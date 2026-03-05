using UnityEngine;

public class Despawner : MonoBehaviour
{
    void Update()
    {
        if (transform.position.y < -100f) Destroy(gameObject);
    }
}
