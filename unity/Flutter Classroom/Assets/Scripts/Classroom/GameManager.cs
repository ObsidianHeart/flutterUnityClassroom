using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public ClassroomConfiguration CurrentClassroomConfig { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist GameManager across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Attempt to load existing configuration on startup
        DataManager dataManager = FindFirstObjectByType<DataManager>();
        if (dataManager != null)
        {
            CurrentClassroomConfig = dataManager.LoadClassroomConfiguration();
            if (CurrentClassroomConfig != null)
            {
                // If a config exists, load classroom scene directly
                LoadClassroomScene();
            }
            else
            {
                // No config, load registration scene
                LoadRegistrationScene();
            }
        }
        else
        {
            Debug.LogError("DataManager not found in scene!");
            // Fallback to registration scene if DataManager is missing
            LoadRegistrationScene();
        }
    }

    public void SetCurrentClassroomConfig(ClassroomConfiguration config)
    {
        CurrentClassroomConfig = config;
    }

    public void LoadRegistrationScene()
    {
        SceneManager.LoadScene("RegistrationScene");
    }

    public void LoadClassroomScene()
    {
        SceneManager.LoadScene("ClassroomScene");
        SceneManager.sceneLoaded += OnClassroomSceneLoaded; // Subscribe to scene loaded event
    }

    private void OnClassroomSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "ClassroomScene")
        {
            SceneManager.sceneLoaded -= OnClassroomSceneLoaded; // Unsubscribe to prevent multiple calls

            if (CurrentClassroomConfig != null && CurrentClassroomConfig.classroomObjects != null)
            {
                // Clear any existing placeholder objects in the scene if necessary
                ClearExistingClassroomObjects(); 

                // Instantiate objects based on loaded configuration
                foreach (ClassroomObjectData objectData in CurrentClassroomConfig.classroomObjects)
                {
                    InstantiateClassroomObject(objectData);
                }
            }
            else
            {
                Debug.LogWarning("No saved configuration or classroom objects found. Loading default classroom.");
                // Optionally, populate with default objects if no saved config exists
                PopulateDefaultClassroom();
            }
        }
    }

    private void ClearExistingClassroomObjects()
    {
        // Find all existing ClassroomObject instances and destroy them
        ClassroomObject[] existingObjects = FindObjectsByType<ClassroomObject>(FindObjectsSortMode.None);
        foreach (ClassroomObject obj in existingObjects)
        {
            Destroy(obj.gameObject);
        }
    }

    private void InstantiateClassroomObject(ClassroomObjectData objectData)
    {
        // Get the prefab for the object type (e.g., Chair_Parent prefab)
        GameObject basePrefab = GetBasePrefabForObjectType(objectData.objectType);

        if (basePrefab != null)
        {
            // Instantiate the base prefab
            GameObject newObjectGO = Instantiate(basePrefab, objectData.position, objectData.rotation);
            newObjectGO.name = objectData.objectID; // Assign the unique ID as name for easy lookup

            ClassroomObject classroomObjectScript = newObjectGO.GetComponent<ClassroomObject>();
            if (classroomObjectScript != null)
            {
                // Find the actual model prefab to apply
                CustomizationManager customizationManager = FindFirstObjectByType<CustomizationManager>();
                GameObject modelPrefab = customizationManager != null ? customizationManager.GetModelPrefabPublic(objectData.objectType, objectData.modelName) : null;
                if (modelPrefab != null)
                {   
                    classroomObjectScript.UpdateModel(modelPrefab, objectData.modelName);
                }
                else
                {
                    Debug.LogError($"Model prefab not found for {objectData.objectType}/{objectData.modelName} during load.");
                }
            }
        }
        else
        {
            Debug.LogError($"Base prefab not found for object type: {objectData.objectType}");
        }
    }

    // Helper method to get the base prefab for a given object type
    private GameObject GetBasePrefabForObjectType(string objectType)
    {
        return Resources.Load<GameObject>("Prefabs/" + objectType + "_Parent"); 
    }

    private void PopulateDefaultClassroom()
    {
        Debug.Log("Populating default classroom...");
        // Example: Instantiate a default chair
        // GameObject defaultChairPrefab = Resources.Load<GameObject>("Prefabs/Chair_Parent");
        // Instantiate(defaultChairPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void SaveCurrentClassroomConfiguration()
    {
        UserRegistrationData currentUserData = CurrentClassroomConfig.userRegistrationData;

        List<ClassroomObjectData> currentObjectsData = new List<ClassroomObjectData>();
        ClassroomObject[] allClassroomObjects = FindObjectsByType<ClassroomObject>(FindObjectsSortMode.None);

        foreach (ClassroomObject obj in allClassroomObjects)
        {
            if (!string.IsNullOrEmpty(obj.ObjectID) && !string.IsNullOrEmpty(obj.CurrentModelName))
            {
                currentObjectsData.Add(new ClassroomObjectData(
                    obj.ObjectID,
                    obj.ObjectType,
                    obj.CurrentModelName,
                    obj.transform.position,
                    obj.transform.rotation
                ));
            }
            else
            {
                Debug.LogWarning($"Skipping object {obj.name} due to missing ID or model name.");
            }
        }

        ClassroomConfiguration configToSave = new ClassroomConfiguration
        {
            userRegistrationData = currentUserData,
            classroomObjects = currentObjectsData
        };

        DataManager dataManager = FindFirstObjectByType<DataManager>();
        if (dataManager != null)
        {
            dataManager.SaveClassroomConfiguration(configToSave);
            CurrentClassroomConfig = configToSave;
        }
        else
        {
            Debug.LogError("DataManager not found in scene! Cannot save configuration.");
        }
    }
}

