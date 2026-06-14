using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDebug : MonoBehaviour
{
    [SerializeField] private Vector3 resetPosition1 = new Vector3(0f, 1f, 0f);
    [SerializeField] private Vector3 resetPosition2 = new Vector3(0f, 1f, 0f);

    private CharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
            ResetPosition1();
        if (Keyboard.current.oKey.wasPressedThisFrame)
            ResetPosition2();
    }

    private void ResetPosition1()
    {
        if (_cc != null)
        {
            _cc.enabled = false;
            transform.position = resetPosition1;
            _cc.enabled = true;
        }
        else
        {
            transform.position = resetPosition1;
        }
    }

    private void ResetPosition2()
    {
        if (_cc != null)
        {
            _cc.enabled = false;
            transform.position = resetPosition2;
            _cc.enabled = true;
        }
        else
        {
            transform.position = resetPosition2;
        }
    }
}
