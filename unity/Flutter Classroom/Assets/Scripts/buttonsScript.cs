using UnityEngine;

public class buttonsScript : MonoBehaviour
{
    // Call this method from your UI Button's OnClick event
    public void ExitGame()
    {
        // If running in the Unity Editor, stop play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running as a built application, quit the application
        Application.Quit();
#endif
    }
}
