using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject ball;
    public float SpawnRate = 5f;

    void Start()
    {
        InvokeRepeating("AddBall", 3, SpawnRate);
        InvokeRepeating("AddBall", 3, SpawnRate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddTopLeftVehicle()
    {
        
    }

    void AddBottomRightVehicle()
    {
        
    }
}
