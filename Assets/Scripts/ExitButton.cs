using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    void Start()
    {
        Button exitButton = GetComponent<Button>();
        exitButton.onClick.AddListener(ExitGame);
    }

    public void ExitGame()
    {
        //Different behaviour between Dev and exe
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
