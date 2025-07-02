using UnityEngine;
using UnityEngine.UI;
using TMPro; // If using TextMeshPro for UI text

public class RegistrationManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField; // Or public InputField nameInputField;
    [SerializeField] private TMP_InputField registrationNumberInputField; // Or public InputField registrationNumberInputField;
    [SerializeField] private Button registerButton;

    private DataManager dataManager; // Reference to the DataManager
    private GameManager gameManager; // Reference to the GameManager

    void Awake()
    {
        // Get references to other managers
        dataManager = FindObjectOfType<DataManager>();
        gameManager = FindObjectOfType<GameManager>();

        // Add listener to the register button
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
    }

    void OnRegisterButtonClicked()
    {
        string userName = nameInputField.text;
        string registrationNumber = registrationNumberInputField.text;

        // Basic validation (can be expanded)
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(registrationNumber))
        {
            Debug.LogWarning("Please enter both Name and Registration Number.");
            // Optionally, display a message to the user via UI
            return;
        }

        // Create UserRegistrationData object
        UserRegistrationData userData = new UserRegistrationData
        {
            userName = userName,
            registrationNumber = registrationNumber
        };

        // Create a new ClassroomConfiguration to store initial data
        ClassroomConfiguration initialConfig = new ClassroomConfiguration
        {
            userRegistrationData = userData,
            classroomObjects = new System.Collections.Generic.List<ClassroomObjectData>() // Empty list for initial state
        };

        // Save the initial configuration
        dataManager.SaveClassroomConfiguration(initialConfig);

        // Proceed to the classroom scene
        gameManager.LoadClassroomScene();
    }
}

