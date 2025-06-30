using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CustomizationMenu : MonoBehaviour
{
    public GameObject menuPanel; // Assign a UI Panel in the Inspector
    public Button prefabOptionButtonPrefab; // Prefab for prefab option buttons
    public Transform prefabOptionsParent; // Parent transform for prefab buttons
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

        // Populate Prefab Options
        if (currentCustomizableObject.prefabOptions != null)
        {
            for (int i = 0; i < currentCustomizableObject.prefabOptions.Length; i++)
            {
                int index = i; // Local copy for closure
                Button button = Instantiate(prefabOptionButtonPrefab, prefabOptionsParent);
                button.GetComponentInChildren<Text>().text = currentCustomizableObject.prefabOptions[i].name;
                button.onClick.AddListener(() => currentCustomizableObject.ApplyPrefab(index));
            }
        }
    }

    void ClearOptions()
    {
        foreach (Transform child in prefabOptionsParent)
        {
            Destroy(child.gameObject);
        }
    }

    void SaveCustomization()
    {
        string customizationLog = $"{currentCustomizableObject.objectName} customized: Prefab Index = {currentCustomizableObject.currentPrefabIndex}";
        Debug.Log(customizationLog);

        System.IO.File.AppendAllText(Application.persistentDataPath + "/customization_log.txt", customizationLog + "\n");

        HideMenu();
    }
}


