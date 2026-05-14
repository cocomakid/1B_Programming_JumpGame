using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelObject : MonoBehaviour
{
    public string nextLevel;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    public void MoveToNextLevel()
    {
        audioManager.PlaySFX(audioManager.portalIn);
        SceneManager.LoadScene(nextLevel);
    }
}
