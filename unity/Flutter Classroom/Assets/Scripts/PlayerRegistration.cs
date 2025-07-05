using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // Add this for TextMeshPro support

public class PlayerRegistration : MonoBehaviour
{
    public InputField nameInputField; // Changed from InputField to TMP_InputField
    public InputField registrationNumberInputField; // Changed from InputField to TMP_InputField
    public Button registerButton;

    void Start()
    {
        registerButton.onClick.AddListener(RegisterPlayer);
    }

    void RegisterPlayer()
    {
        string playerName = nameInputField.text;
        string registrationNumber = registrationNumberInputField.text;

        if (!string.IsNullOrEmpty(playerName) && !string.IsNullOrEmpty(registrationNumber))
        {
            PlayerPrefs.SetString("PlayerName", playerName);
            PlayerPrefs.SetString("RegistrationNumber", registrationNumber);
            SceneManager.LoadScene("ClassroomScene"); // Assuming your second scene is named ClassroomScene
        }
        else
        {
            Debug.LogWarning("Please enter both name and registration number.");
        }
    }
}


