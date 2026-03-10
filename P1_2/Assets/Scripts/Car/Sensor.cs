using UnityEngine;

public class Sensor : MonoBehaviour
{
    protected bool detected;
    protected GameObject target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        detected = false;
        InvokeRepeating("Check", 0, 0.1f);
    }

    public void Check()
    {   
        float d = Vector3.Distance(transform.position, target.transform.position);
        if (d < 10) {
            //Debug.Log("Target is close");
            detected = true;
        }
        else
            detected = false;
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

