using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    private string saveFilePath;

    void Awake()
    {
        // Define the save file path. Application.persistentDataPath is recommended for persistent data.
        // This path is platform-dependent and ensures data is saved in a location accessible by the app.
        saveFilePath = Path.Combine(Application.persistentDataPath, "classroom_config.json");
        Debug.Log($"Save file path: {saveFilePath}");
    }

    // Saves the current ClassroomConfiguration to a JSON file
    public void SaveClassroomConfiguration(ClassroomConfiguration config)
    {
        try
        {
            // Convert the ClassroomConfiguration object to a JSON string
            // The 'true' argument enables pretty printing for readability in the file
            string json = JsonUtility.ToJson(config, true);
            
            // Write the JSON string to the specified file path
            File.WriteAllText(saveFilePath, json);
            Debug.Log("Classroom configuration saved successfully to: " + saveFilePath);
            FindObjectOfType<UIManager>()?.ShowMessage("Classroom configuration saved!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save classroom configuration: {e.Message}");
            FindObjectOfType<UIManager>()?.ShowMessage("Failed to save configuration!");
        }
    }

    // Loads a ClassroomConfiguration from a JSON file
    public ClassroomConfiguration LoadClassroomConfiguration()
    {
        // Check if the save file exists
        if (File.Exists(saveFilePath))
        {
            try
            {
                // Read the entire JSON string from the file
                string json = File.ReadAllText(saveFilePath);
                
                // Convert the JSON string back into a ClassroomConfiguration object
                ClassroomConfiguration config = JsonUtility.FromJson<ClassroomConfiguration>(json);
                Debug.Log("Classroom configuration loaded successfully from: " + saveFilePath);
                return config;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load classroom configuration: {e.Message}");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("No saved classroom configuration found at: " + saveFilePath);
            return null;
        }
    }
}

