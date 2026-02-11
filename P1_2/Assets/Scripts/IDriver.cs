using UnityEngine;

interface IDriver
{
    Vector3 move(float maxSpeed);
}


abstract public class Driver: MonoBehaviour, IDriver {
    public abstract Vector3 move(float maxSpeed);
}
