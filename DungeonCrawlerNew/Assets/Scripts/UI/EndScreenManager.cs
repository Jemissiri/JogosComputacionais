using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    public void OnTryAgain()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
