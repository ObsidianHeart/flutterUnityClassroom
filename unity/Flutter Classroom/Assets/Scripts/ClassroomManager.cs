using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class ClassroomManager : MonoBehaviour
{
    public List<GameObject> classroomTypes; // Assign your different classroom prefabs/objects here
    public Transform respawnPoint; // Assign an empty GameObject as the respawn point
    public Button chooseClassroomButton;
    public Button saveButton;

    private GameObject currentClassroom;
    private int currentClassroomIndex = -1;

    void Start()
    {
        chooseClassroomButton.onClick.AddListener(ChooseNextClassroom);
        saveButton.onClick.AddListener(SaveClassroomAndExit);

        // Load initial classroom if saved, otherwise load the first one
        LoadSavedClassroom();
    }

    void ChooseNextClassroom()
    {
        if (classroomTypes.Count == 0)
        {
            Debug.LogWarning("No classroom types assigned.");
            return;
        }

        // Destroy current classroom if exists
        if (currentClassroom != null)
        {
            Destroy(currentClassroom);
        }

        currentClassroomIndex = (currentClassroomIndex + 1) % classroomTypes.Count;
        currentClassroom = Instantiate(classroomTypes[currentClassroomIndex], Vector3.zero, Quaternion.identity);

        // Teleport player to respawn point
        TeleportPlayerToRespawnPoint();
    }

    void TeleportPlayerToRespawnPoint()
    {
        if (respawnPoint != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player"); // Assuming player has 'Player' tag
            if (player != null)
            {
                player.transform.position = respawnPoint.position;
                player.transform.rotation = respawnPoint.rotation;
            }
            else
            {
                Debug.LogWarning("Player GameObject not found. Make sure it has the 'Player' tag.");
            }
        }
        else
        {
            Debug.LogWarning("Respawn Point not assigned.");
        }
    }

    void SaveClassroomAndExit()
    {
        if (currentClassroomIndex != -1)
        {
            string playerName = PlayerPrefs.GetString("PlayerName", "");
            string registrationNumber = PlayerPrefs.GetString("RegistrationNumber", "");
            string classroomTypeName = classroomTypes[currentClassroomIndex].name;

            PlayerSaveData data = new PlayerSaveData
            {
                playerName = playerName,
                registrationNumber = registrationNumber,
                classroomType = classroomTypeName
            };

            string json = JsonUtility.ToJson(data, true);
            string filePath = "";

#if UNITY_ANDROID && !UNITY_EDITOR
            // On Android, save to the public Downloads folder
            filePath = "/storage/emulated/0/Download/player_data.json";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            // On Windows, save to the user's Downloads folder
            string downloadsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Downloads";
            filePath = Path.Combine(downloadsPath, "player_data.json");
#else
            // Fallback to persistentDataPath
            filePath = Application.persistentDataPath + "/player_data.json";
#endif

            try
            {
                File.WriteAllText(filePath, json);
                Debug.Log("Player data saved to: " + filePath);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to save player data: " + ex.Message);
            }

            Debug.Log("Thanks for playing!");
            Application.Quit();
        }
        else
        {
            Debug.LogWarning("No classroom selected to save.");
        }
    }

    void LoadSavedClassroom()
    {
        string filePath = Application.persistentDataPath + "/player_data.json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            // Find the classroom by name and instantiate it
            for (int i = 0; i < classroomTypes.Count; i++)
            {
                if (classroomTypes[i].name == data.classroomType)
                {
                    currentClassroomIndex = i;
                    currentClassroom = Instantiate(classroomTypes[currentClassroomIndex], Vector3.zero, Quaternion.identity);
                    Debug.Log("Loaded saved classroom: " + data.classroomType);
                    TeleportPlayerToRespawnPoint();
                    break;
                }
            }
        }
        else
        {
            Debug.Log("No saved player data found. Loading first classroom.");
            ChooseNextClassroom(); // Load the first classroom if no data is saved
        }
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public string playerName;
    public string registrationNumber;
    public string classroomType;
}


