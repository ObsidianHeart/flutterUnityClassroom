using UnityEngine;

[System.Serializable]
public class ClassroomObjectData
{
    public string objectID;         // A unique identifier for the object instance (e.g., GUID)
    public string objectType;       // The category of the object (e.g., "Chair", "Desk")
    public string modelName;        // The name of the specific model/style applied (e.g., "ModernChair", "WoodenDesk")
    public SerializableVector3 position; // Custom serializable Vector3
    public SerializableQuaternion rotation; // Custom serializable Quaternion

    public ClassroomObjectData(string id, string type, string model, Vector3 pos, Quaternion rot)
    {
        objectID = id;
        objectType = type;
        modelName = model;
        position = new SerializableVector3(pos.x, pos.y, pos.z);
        rotation = new SerializableQuaternion(rot.x, rot.y, rot.z, rot.w);
    }
}

