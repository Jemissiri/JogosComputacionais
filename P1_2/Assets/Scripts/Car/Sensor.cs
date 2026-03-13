using UnityEngine;

public class Sensor : MonoBehaviour
{
    protected bool detected;
    protected GameObject target;

    void Start()
    {
        detected = false;
        InvokeRepeating("Check", 0, 0.1f);
    }

    public void Check()
    {   
        float d = Vector3.Distance(transform.position, target.transform.position);
        detected = d < 10;
    }

    public bool Detected {
        get {
            return detected;
        }
    }
    public GameObject Target {
        get {
            return target;
        }
        set {
            target = value;
        }
    }
}

