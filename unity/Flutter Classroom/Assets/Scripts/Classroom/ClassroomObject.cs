using UnityEngine;

public class ClassroomObject : MonoBehaviour
{
    [SerializeField] private string objectID; // Unique ID for this instance
    [SerializeField] private string objectType; // e.g., "Chair", "Desk", "Whiteboard"
    [SerializeField] private string currentModelName; // Name of the currently applied model

    // Reference to the actual 3D model GameObject that will be swapped
    private GameObject currentModelInstance;
    private Renderer objectRenderer; // For highlighting
    private Material originalMaterial; // To restore original material after highlight

    public string ObjectID => objectID;
    public string ObjectType => objectType;
    public string CurrentModelName => currentModelName;

    void Awake()
    {
        // Ensure objectID is unique, especially if instantiated at runtime
        if (string.IsNullOrEmpty(objectID))
        {
            objectID = System.Guid.NewGuid().ToString();
        }

        // Get the initial model instance (assuming it's a child of this GameObject)
        if (transform.childCount > 0)
        {
            currentModelInstance = transform.GetChild(0).gameObject;
            objectRenderer = currentModelInstance.GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                originalMaterial = objectRenderer.material;
            }
        }
    }

    void OnMouseDown()
    {
        // This object was clicked, notify the CustomizationManager
        CustomizationManager customizationManager = FindFirstObjectByType<CustomizationManager>();
        if (customizationManager != null && customizationManager.GetCurrentlySelectedClassroomObject() == this)
        {
            customizationManager.SelectClassroomObject(this);
        }
    }

    public void UpdateModel(GameObject newModelPrefab, string newModelName)
    {
        // Destroy the old model instance
        if (currentModelInstance != null)
        {
            Destroy(currentModelInstance);
        }

        // Instantiate the new model prefab as a child of this GameObject
        currentModelInstance = Instantiate(newModelPrefab, transform);
        currentModelInstance.transform.localPosition = Vector3.zero;
        currentModelInstance.transform.localRotation = Quaternion.identity;
        currentModelInstance.transform.localScale = Vector3.one; // Ensure scale is correct

        currentModelName = newModelName;

        // Update renderer for highlighting
        objectRenderer = currentModelInstance.GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }

        // Re-apply highlight if this object was previously selected
        CustomizationManager customizationManager = FindFirstObjectByType<CustomizationManager>();
        if (customizationManager != null && customizationManager.GetCurrentlySelectedClassroomObject() == this)
        {
            SetHighlight(true);
        }
    }

    public void SetHighlight(bool highlight)
    {
        if (objectRenderer != null)
        {
            if (highlight)
            {
                // Apply a highlight material (e.g., a bright emissive color)
                objectRenderer.material = new Material(Shader.Find("Standard")); // Create new material
                objectRenderer.material.color = Color.yellow; // Example highlight color
                objectRenderer.material.EnableKeyword("_EMISSION");
                objectRenderer.material.SetColor("_EmissionColor", Color.yellow * 0.5f);
            }
            else
            {
                // Restore original material
                objectRenderer.material = originalMaterial;
            }
        }
    }
}

