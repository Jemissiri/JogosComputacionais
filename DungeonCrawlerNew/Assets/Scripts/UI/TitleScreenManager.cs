using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public void OnPlayButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnInstructionsButton()
    {
        SceneManager.LoadScene("Instructions");
    }

    public void OnCreditsButton()
    {
        SceneManager.LoadScene("Credits");
    }
}