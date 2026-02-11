using UnityEngine;

public class AddBalls : MonoBehaviour
{
    public GameObject ball;
    public float SpawnRate = 5f;
    void Start()
    {
        InvokeRepeating("AddBall", 3, SpawnRate);
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
}
