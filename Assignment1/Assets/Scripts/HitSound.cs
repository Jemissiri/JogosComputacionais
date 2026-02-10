using UnityEngine;

public class HitSound : MonoBehaviour
{
    AudioSource source;
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        source.Play();
    }

    // void Update()
    // {
        
    // }
}
