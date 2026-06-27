using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionsManager : MonoBehaviour
{
    public void OnBackButton()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}