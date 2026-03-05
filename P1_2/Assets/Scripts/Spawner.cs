using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject ball;
    public GameObject[] vehicles;
    public float BallSpawnRate = 5f;
    public float VehicleTopLeftSpawnRate = 5f;
    public float VehicleBottomRightSpawnRate = 10f;

    void Start()
    {
        InvokeRepeating("AddBall", 3, BallSpawnRate);
        InvokeRepeating("AddTopLeftVehicle", 3, VehicleTopLeftSpawnRate);
        InvokeRepeating("AddBottomRightVehicle", 3, VehicleBottomRightSpawnRate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddBall()
    {
        const int xmin = -3;
        const int xmax = 4;
        const int ymin = 15;
        const int ymax = 25;
        const int zmin = 95;
        const int zmax = 195;
        float x = Random.Range(xmin, xmax);
        float y = Random.Range(ymin, ymax);
        float z = Random.Range(zmin, zmax);
        Vector3 position = new Vector3(x, y, z);

        Debug.Log(position);

        Instantiate(ball, position, Quaternion.identity);
    }
    
    void addVehicles(Vector3 pos, float speed, Quaternion rot) 
    {
        int v = (int) Mathf.Round(Random.Range(0,vehicles.Length));
        GameObject vehicle = Instantiate(vehicles[v], pos, rot);
        vehicle.AddComponent<Despawner>();
        AI ai = vehicle.AddComponent<AI>();
        MoveCar mc = vehicle.AddComponent<MoveCar>();
        mc.fuel = 100;
        mc.maxSpeed = speed;
        mc.driver = ai;
    }

    void AddTopLeftVehicle()
    {
        addVehicles(new Vector3(-1.85f, 10, 180), 3f, Quaternion.Euler(0, 180, 0));
    }

    void AddBottomRightVehicle()
    {
        addVehicles(new Vector3(1.85f, 2, 5), 3f, Quaternion.Euler(0, 0, 0));
    }
}
