using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void ExitGame()
    {
        Application.Quit();

        UnityEditor.EditorApplication.isPlaying = false;

        Debug.Log("게임이 종료되었습니다.");
    }
}