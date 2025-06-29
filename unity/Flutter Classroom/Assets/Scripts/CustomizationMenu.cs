
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CustomizationMenu : MonoBehaviour
{
    public GameObject menuPanel; // Assign a UI Panel in the Inspector
    public Button materialOptionButtonPrefab; // Prefab for material option buttons
    public Button colorOptionButtonPrefab; // Prefab for color option buttons
    public Transform materialOptionsParent; // Parent transform for material buttons
    public Transform colorOptionsParent; // Parent transform for color buttons
    public Button saveButton; // Button to save changes

    private CustomizableObject currentCustomizableObject;

    void Start()
    {
        HideMenu();
        saveButton.onClick.AddListener(SaveCustomization);
    }

    public void ShowMenu(CustomizableObject obj)
    {
        currentCustomizableObject = obj;
        menuPanel.SetActive(true);
        PopulateOptions();
    }

    public void HideMenu()
    {
        menuPanel.SetActive(false);
        ClearOptions();
    }

    void PopulateOptions()
    {
        ClearOptions();

        // Populate Material Options
        if (currentCustomizableObject.materialOptions != null)
        {
            for (int i = 0; i < currentCustomizableObject.materialOptions.Length; i++)
            {
                int index = i; // Local copy for closure
                Button button = Instantiate(materialOptionButtonPrefab, materialOptionsParent);
                button.GetComponentInChildren<Text>().text = currentCustomizableObject.materialOptions[i].name;
                button.onClick.AddListener(() => currentCustomizableObject.ApplyMaterial(index));
            }
        }

        // Populate Color Options
        if (currentCustomizableObject.colorOptions != null)
        {
            for (int i = 0; i < currentCustomizableObject.colorOptions.Length; i++)
            {
                int index = i; // Local copy for closure
                Button button = Instantiate(colorOptionButtonPrefab, colorOptionsParent);
                // You might want to set the button's image color to the actual color option
                button.GetComponentInChildren<Text>().text = "Color " + (i + 1);
                button.onClick.AddListener(() => currentCustomizableObject.ApplyColor(index));
            }
        }
    }

    void ClearOptions()
    {
        foreach (Transform child in materialOptionsParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in colorOptionsParent)
        {
            Destroy(child.gameObject);
        }
    }

    void SaveCustomization()
    {
        // This is where you'd save the current customization to a text file.
        // For now, let's just log it.
        string customizationLog = $"{currentCustomizableObject.objectName} customized: Material Index = {currentCustomizableObject.currentMaterialIndex}, Color Index = {currentCustomizableObject.currentColorIndex}";
        Debug.Log(customizationLog);

        // In a real application, you would write this to a file.
        // Example: System.IO.File.AppendAllText("customization_log.txt", customizationLog + "\n");
        System.IO.File.AppendAllText(Application.persistentDataPath + "/customization_log.txt", customizationLog + "\n");

        HideMenu();
    }
}


