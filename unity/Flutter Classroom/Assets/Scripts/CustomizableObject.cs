using UnityEngine;

public class CustomizableObject : MonoBehaviour
{
    public string objectName; // e.g., "Chair", "Table", "Whiteboard"
    public GameObject[] prefabOptions; // Array of prefabs for customization (e.g., different chair models)
    public int currentPrefabIndex = 0;

    [HideInInspector] public Material originalMaterial; // To store the original material for highlighting

    public void ApplyPrefab(int index)
    {
        if (prefabOptions != null && index >= 0 && index < prefabOptions.Length)
        {
            // Store current position and rotation before destroying
            Vector3 oldPosition = transform.position;
            Quaternion oldRotation = transform.rotation;
            Transform oldParent = transform.parent;

            // Instantiate the new prefab
            GameObject newPrefabInstance = Instantiate(prefabOptions[index], oldPosition, oldRotation);
            newPrefabInstance.transform.parent = oldParent; // Keep the same parent

            // Transfer customization data to the new prefab's CustomizableObject component
            CustomizableObject newCustomizableObject = newPrefabInstance.GetComponent<CustomizableObject>();
            if (newCustomizableObject != null)
            {
                newCustomizableObject.objectName = objectName;
                newCustomizableObject.prefabOptions = prefabOptions; // Pass the array of options
                newCustomizableObject.currentPrefabIndex = index; // Set the current index
            }

            // Notify ObjectSelection about the prefab swap
            ObjectSelection objectSelection = FindObjectOfType<ObjectSelection>();
            if (objectSelection != null)
            {
                objectSelection.OnPrefabSwapped(this, newCustomizableObject);
            }

            // Destroy the old object
            Destroy(gameObject);
        }
    }
}


