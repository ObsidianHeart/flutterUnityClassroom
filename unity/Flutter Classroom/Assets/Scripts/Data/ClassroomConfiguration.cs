using System.Collections.Generic;

[System.Serializable]
public class ClassroomConfiguration
{
    public UserRegistrationData userRegistrationData;
    public List<ClassroomObjectData> classroomObjects;

    public ClassroomConfiguration()
    {
        userRegistrationData = new UserRegistrationData();
        classroomObjects = new List<ClassroomObjectData>();
    }
}

