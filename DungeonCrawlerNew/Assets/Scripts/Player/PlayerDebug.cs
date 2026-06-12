using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDebug : MonoBehaviour
{
    [SerializeField] private Vector3 resetPosition = new Vector3(0f, 1f, 0f);

    private CharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
            ResetPosition();
    }

    private void ResetPosition()
    {
        if (_cc != null)
        {
            _cc.enabled = false;
            transform.position = resetPosition;
            _cc.enabled = true;
        }
        else
        {
            transform.position = resetPosition;
        }
    }
}
