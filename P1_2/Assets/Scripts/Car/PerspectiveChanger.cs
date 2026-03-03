using UnityEngine;

public class PerspectiveChanger : MonoBehaviour
{

    [SerializeField] private Camera frontCam;
    [SerializeField] private Camera backCam;
    void OnPerspective()
    {
        frontCam.enabled = !frontCam.enabled;
        backCam.enabled = !backCam.enabled;
    }
}
