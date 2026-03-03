using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float fuel = 100;
    private Vector2 _deltaMove;
    [SerializeField] private Camera frontCam;
    [SerializeField] private Camera backCam;


    void OnPerspective()
    {
        if (frontCam.enabled)
        {
            frontCam.enabled = false;
            backCam.enabled = true;
        }
        else
        {
            frontCam.enabled = true;
            backCam.enabled = false;
        }
    }
    void OnMove(InputValue value) {
        _deltaMove = value.Get<Vector2>();
    }
    void Update()
    {
        if (fuel > 0)
        {
            Vector3 deltaPosition = new Vector3(_deltaMove.x, 0, _deltaMove.y);
            transform.position += deltaPosition * (speed * Time.deltaTime);
            if (deltaPosition.sqrMagnitude > 0) // basicamente ver se o vetor nao e (0,0,0)
            {
                fuel -= Time.deltaTime;
                if (fuel < 0) fuel = 0;
            }        
        }
    }
}
