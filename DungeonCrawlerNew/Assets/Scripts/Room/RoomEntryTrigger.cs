using UnityEngine;

public class RoomEntryTrigger : MonoBehaviour
{
    [SerializeField] private GameObject entryDoor;

    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;

        _triggered = true;

        if (entryDoor != null)
            entryDoor.SetActive(true);
    }
}
