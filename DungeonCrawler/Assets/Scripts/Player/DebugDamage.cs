using UnityEngine;
using UnityEngine.InputSystem;

public class DebugDamage : MonoBehaviour
{
    public int damageAmount = 10;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
            GetComponent<PlayerHealth>().TakeDamage(damageAmount);
    }
}
