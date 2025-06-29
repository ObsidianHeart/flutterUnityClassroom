
using UnityEngine;

public class CustomizableObject : MonoBehaviour
{
    public string objectName; // e.g., "Chair", "Table", "Whiteboard"
    public Material[] materialOptions; // Array of materials for customization
    public Color[] colorOptions; // Array of colors for customization
    public int currentMaterialIndex = 0;
    public int currentColorIndex = 0;

    private Renderer objectRenderer;

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("CustomizableObject: No Renderer found on this GameObject.");
        }
    }

    public void ApplyMaterial(int index)
    {
        if (objectRenderer != null && materialOptions != null && index >= 0 && index < materialOptions.Length)
        {
            objectRenderer.material = materialOptions[index];
            currentMaterialIndex = index;
        }
    }

    public void ApplyColor(int index)
    {
        if (objectRenderer != null && colorOptions != null && index >= 0 && index < colorOptions.Length)
        {
            objectRenderer.material.color = colorOptions[index];
            currentColorIndex = index;
        }
    }
}


