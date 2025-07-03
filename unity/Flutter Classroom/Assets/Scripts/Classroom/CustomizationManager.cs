using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CustomizationManager : MonoBehaviour
{
    [System.Serializable]
    public class ObjectTypeModels
    {
        public string objectType;
        public List<GameObject> models; // Prefabs of different models for this type
    }

    // Changed to public for UIManager to access directly for image display
    public List<ObjectTypeModels> availableCustomizationOptions; // Populated in Inspector

    private string selectedObjectType;
    private string selectedModelName;
    public ClassroomObject currentlySelectedClassroomObject; // Changed to public

    // Public methods for UIManager to call
    public List<string> GetAvailableObjectTypes()
    {
        return availableCustomizationOptions.Select(o => o.objectType).ToList();
    }

    public List<string> GetModelsForType(string type)
    {
        ObjectTypeModels options = availableCustomizationOptions.FirstOrDefault(o => o.objectType == type);
        if (options != null)
        {
            return options.models.Select(m => m.name).ToList();
        }
        return new List<string>();
    }

    public void SetSelectedObjectType(string type)
    {
        selectedObjectType = type;
        selectedModelName = string.Empty; // Reset selected model when type changes
    }

    public void SetSelectedModel(string modelName)
    {
        selectedModelName = modelName;
    }

    // Method to be called when a ClassroomObject in the scene is clicked
    public void SelectClassroomObject(ClassroomObject obj)
    {
        if (currentlySelectedClassroomObject != null)
        {
            currentlySelectedClassroomObject.SetHighlight(false); // Remove highlight from previous
        }
        currentlySelectedClassroomObject = obj;
        currentlySelectedClassroomObject.SetHighlight(true); // Add highlight to new

        // Optionally, pre-select the object type and model in the UI based on the clicked object
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.OnObjectTypeSelected(obj.ObjectType); // This will also populate models
            // Further logic to select the specific model in the UI if needed
        }
    }

    public void ApplySelectedCustomization()
    {
        if (currentlySelectedClassroomObject == null)
        {
            Debug.LogWarning("No classroom object selected for customization.");
            FindObjectOfType<UIManager>()?.ShowMessage("Please select an object first!");
            return;
        }

        if (string.IsNullOrEmpty(selectedModelName))
        {
            Debug.LogWarning("No model selected for customization.");
            FindObjectOfType<UIManager>()?.ShowMessage("Please select a model first!");
            return;
        }

        // Find the GameObject prefab for the selected model
        GameObject newModelPrefab = GetModelPrefab(selectedObjectType, selectedModelName);
        if (newModelPrefab != null)
        {
            currentlySelectedClassroomObject.UpdateModel(newModelPrefab, selectedModelName);
            FindObjectOfType<UIManager>()?.ShowMessage($"Applied {selectedModelName} to {currentlySelectedClassroomObject.name}");
        }
        else
        {
            Debug.LogError($"Model prefab not found for type {selectedObjectType} and model {selectedModelName}");
            FindObjectOfType<UIManager>()?.ShowMessage("Error: Model not found!");
        }
    }

    public GameObject GetModelPrefab(string type, string modelName) // Changed to public
    {
        ObjectTypeModels options = availableCustomizationOptions.FirstOrDefault(o => o.objectType == type);
        if (options != null)
        {
            return options.models.FirstOrDefault(m => m.name == modelName);
        }
        return null;
    }
}

