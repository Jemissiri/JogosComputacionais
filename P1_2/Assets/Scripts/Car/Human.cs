using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

class Human : Driver
{
    private Vector2 _deltaMove;
    private int hits;

    [SerializeField] private TextMeshProUGUI hitTextUI;

    public void Start() 
    {
        hits = 0;
        UpdateHitUI();
    }

    public override Vector3 Move(float maxSpeed) 
    {
        float dz = _deltaMove.y;
        float dx = _deltaMove.x;
        return new Vector3(dx, 0, dz) * maxSpeed;
    }

    public void OnMove(InputValue value) 
    {
        _deltaMove = value.Get<Vector2>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Transform root = collision.transform.root;

        bool collidedWithVehicle = root.GetComponent<MoveCar>() != null;
        bool collidedWithBall = root.name.StartsWith("Ball");

        if (collidedWithVehicle || collidedWithBall)
        {
            hits++;
            UpdateHitUI();
        }
    }

    private void UpdateHitUI()
    {
        if (hitTextUI != null)
            hitTextUI.text = "Hits: " + hits;
    }

}
