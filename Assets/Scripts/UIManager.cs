using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHomeManager : MonoBehaviour
{
    public void GameStartButtonAction()
    {
        PlayerController.currentHp = 3;
        SceneManager.LoadScene("Level_1");
    }
}
