using UnityEngine;

public class PerspectiveChanger : MonoBehaviour
{

    [SerializeField] private Camera frontCam;
    [SerializeField] private Camera backCam;
    void OnPerspective()
    {
        if (frontCam.enabled)
        {
            frontCam.enabled = false;
            backCam.enabled = true;
        }
        else
        {
            frontCam.enabled = true;
            backCam.enabled = false;
        }
    }
}
