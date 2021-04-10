using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public string savefileName;


    public void WriteSaveFile(string contents)
    {
        string filepath = Application.persistentDataPath + "/" + savefileName;
        File.WriteAllText(filepath, contents);
    }

    public string ReadSaveFile()
    {
        string filepath = Application.persistentDataPath + "/" + savefileName;
        if (File.Exists(filepath))
        {
            return File.ReadAllText(filepath);
        }
        return "0";
    }


}
