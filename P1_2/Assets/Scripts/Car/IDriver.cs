using UnityEngine;

interface IDriver
{
    Vector3 Move(float maxSpeed);
}

abstract public class Driver: MonoBehaviour, IDriver 
{
    public abstract Vector3 Move(float maxSpeed);
}
