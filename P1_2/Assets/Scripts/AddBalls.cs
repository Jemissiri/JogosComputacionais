using UnityEngine;

public class AddBalls : MonoBehaviour
{
    public GameObject ball;
    void Start()
    {
        InvokeRepeating("AddBall", 3, 5);
    }

    void AddBall()
    {
        const int xmin = -3;
        const int xmax = 3;
        const int ymin = 20;
        const int ymax = 30;
        const int zmin = 145;
        const int zmax = 195;
        float x = Random.Range(xmin, xmax);
        float y = Random.Range(ymin, ymax);
        float z = Random.Range(zmin, zmax);
        Vector3 position = new Vector3(x, y, z);

        Debug.Log(position);

        Instantiate(ball, position, Quaternion.identity);
    }
}
