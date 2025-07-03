using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("Main UI Elements")]
    [SerializeField] private Button customizeButton;
    [SerializeField] private GameObject customizationPanel;
    [SerializeField] private Button saveChangesButton;
    [SerializeField] private Button closeCustomizationButton;
    [SerializeField] private Button applyChangesButton; // Added this line

    [Header("Object Type Selection")]
    [SerializeField] private Transform objectTypeButtonParent; // Parent for object type buttons
    [SerializeField] private GameObject objectTypeButtonPrefab; // Prefab for object type buttons

    [Header("Model Selection")]
    [SerializeField] private Transform modelSelectionParent; // Parent for model buttons/images
    [SerializeField] private GameObject modelDisplayPrefab; // Prefab for model display (image + name)
    [SerializeField] private TextMeshProUGUI selectedObjectTypeText;

    [Header("Messages")]
    [SerializeField] private TextMeshProUGUI messageText; // UI Text to display messages
    [SerializeField] private float messageDisplayDuration = 3f;
    private Coroutine messageCoroutine;

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

        // Ensure message text is initially empty
        if (messageText != null) messageText.text = "";
    }

    void Start()
    {
        // Populate object type buttons on start
        PopulateObjectTypeButtons();
    }

    void ToggleCustomizationPanel()
    {
        customizationPanel.SetActive(!customizationPanel.activeSelf);
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
        // We need the actual GameObject prefabs to get their sprites/textures
        CustomizationManager.ObjectTypeModels objectTypeData = customizationManager.availableCustomizationOptions.FirstOrDefault(o => o.objectType == objectType);

        if (objectTypeData != null)
        {
            foreach (GameObject modelPrefab in objectTypeData.models)
            {
                GameObject displayGO = Instantiate(modelDisplayPrefab, modelSelectionParent);

                // Find Image component in the prefab (assuming it's named 'ModelImage' or similar)
                Image modelImage = displayGO.GetComponentInChildren<Image>();
                if (modelImage != null)
                {
                    // Attempt to get a Sprite from the modelPrefab directly if it has a SpriteRenderer
                    SpriteRenderer spriteRenderer = modelPrefab.GetComponentInChildren<SpriteRenderer>();
                    if (spriteRenderer != null && spriteRenderer.sprite != null)
                    {
                        modelImage.sprite = spriteRenderer.sprite;
                    }
                    else
                    {
                        // Fallback: Try to get a texture from a MeshRenderer and convert to Sprite
                        Renderer modelRenderer = modelPrefab.GetComponentInChildren<Renderer>();
                        if (modelRenderer != null && modelRenderer.sharedMaterial != null && modelRenderer.sharedMaterial.mainTexture != null)
                        {
                            Texture2D texture = modelRenderer.sharedMaterial.mainTexture as Texture2D;
                            if (texture != null)
                            {
                                modelImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                            }
                            else
                            {
                                Debug.LogWarning($"Texture for {modelPrefab.name} is not a Texture2D. Cannot create Sprite.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"No SpriteRenderer or main texture found for model {modelPrefab.name}. Ensure models have a SpriteRenderer or a material with a main texture.");
                        }
                    }
                }

                // Find TextMeshProUGUI component for the name
                TextMeshProUGUI modelNameText = displayGO.GetComponentInChildren<TextMeshProUGUI>();
                if (modelNameText != null)
                {
                    modelNameText.text = modelPrefab.name;
                }

                Button button = displayGO.GetComponent<Button>();
                if (button != null)
                {
                    string currentModelName = modelPrefab.name; // Capture for closure
                    button.onClick.AddListener(() => OnModelSelected(currentModelName));
                }
                else
                {
                    Debug.LogError($"Model display prefab {modelDisplayPrefab.name} is missing a Button component.");
                }
            }
        }
    }

    void OnModelSelected(string modelName)
    {
        customizationManager.SetSelectedModel(modelName);
        Debug.Log($"Selected Model: {modelName}");
        ShowMessage($"Model selected: {modelName}");
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

    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            if (messageCoroutine != null)
            {
                StopCoroutine(messageCoroutine);
            }
            messageText.text = message;
            messageCoroutine = StartCoroutine(ClearMessageAfterDelay(messageDisplayDuration));
        }
        else
        {
            Debug.Log($"UI Message: {message}");
        }
    }

    private System.Collections.IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.text = "";
    }
}

