using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Main UI Elements")]
    [SerializeField] private Button customizeButton;
    [SerializeField] private GameObject customizationPanel;
    [SerializeField] private Button saveChangesButton;
    [SerializeField] private Button closeCustomizationButton;

    [Header("Object Type Selection")]
    [SerializeField] private Transform objectTypeButtonParent; // Parent for object type buttons
    [SerializeField] private GameObject objectTypeButtonPrefab; // Prefab for object type buttons

    [Header("Model Selection")]
    [SerializeField] private Transform modelSelectionParent; // Parent for model buttons/images
    [SerializeField] private GameObject modelButtonPrefab; // Prefab for model buttons
    [SerializeField] private TextMeshProUGUI selectedObjectTypeText;

    [Header("Customization Actions")]
    [SerializeField] private Button applyChangesButton;

    private CustomizationManager customizationManager;

    void Awake()
    {
        customizationManager = FindObjectOfType<CustomizationManager>();

        customizeButton.onClick.AddListener(ToggleCustomizationPanel);
        saveChangesButton.onClick.AddListener(OnSaveChangesClicked);
        closeCustomizationButton.onClick.AddListener(ToggleCustomizationPanel);
        applyChangesButton.onClick.AddListener(OnApplyChangesClicked);

        // Initially hide the customization panel
        customizationPanel.SetActive(false);
    }

    void Start()
    {
        // Populate object type buttons on start
        PopulateObjectTypeButtons();
    }

    void ToggleCustomizationPanel()
    {
        customizationPanel.SetActive(!customizationPanel.activeSelf);
        // When panel opens, ensure cursor is visible and unlocked for UI interaction
        if (customizationPanel.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // When panel closes, re-lock cursor for 3D navigation
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void PopulateObjectTypeButtons()
    {
        // Clear existing buttons
        foreach (Transform child in objectTypeButtonParent)
        {
            Destroy(child.gameObject);
        }

        // Get available object types from CustomizationManager
        List<string> objectTypes = customizationManager.GetAvailableObjectTypes();

        foreach (string type in objectTypes)
        {
            GameObject buttonGO = Instantiate(objectTypeButtonPrefab, objectTypeButtonParent);
            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = type;
            Button button = buttonGO.GetComponent<Button>();
            string currentType = type; // Capture for closure
            button.onClick.AddListener(() => OnObjectTypeSelected(currentType));
        }
    }

    public void OnObjectTypeSelected(string objectType)
    {
        selectedObjectTypeText.text = $"Selected Type: {objectType}";
        customizationManager.SetSelectedObjectType(objectType);
        PopulateModelSelection(objectType);
    }

    void PopulateModelSelection(string objectType)
    {
        // Clear existing models
        foreach (Transform child in modelSelectionParent)
        {
            Destroy(child.gameObject);
        }

        // Get available models for the selected object type
        List<string> models = customizationManager.GetModelsForType(objectType);

        foreach (string modelName in models)
        {
            GameObject buttonGO = Instantiate(modelButtonPrefab, modelSelectionParent);
            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = modelName;
            Button button = buttonGO.GetComponent<Button>();
            string currentModelName = modelName; // Capture for closure
            button.onClick.AddListener(() => OnModelSelected(currentModelName));
        }
    }

    void OnModelSelected(string modelName)
    {
        customizationManager.SetSelectedModel(modelName);
        Debug.Log($"Selected Model: {modelName}");
        // Visual feedback for selected model can be added here
    }

    void OnApplyChangesClicked()
    {
        customizationManager.ApplySelectedCustomization();
        Debug.Log("Apply Changes Clicked");
    }

    void OnSaveChangesClicked()
    {
        GameManager.Instance.SaveCurrentClassroomConfiguration();
        Debug.Log("Save Changes Clicked");
    }

    // Method to show feedback to the user (e.g., a success message)
    public void ShowMessage(string message)
    {
        // Implement UI element to display messages (e.g., a temporary text field)
        Debug.Log($"UI Message: {message}");
    }
}

