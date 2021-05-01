using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class SaveManager : MonoBehaviour
{
    public string savefileName;

    private BinaryFormatter formatter;

    public void WriteSaveFile(string contents)
    {
        string filepath = Application.persistentDataPath + "/" + savefileName;
        File.WriteAllText(filepath, contents);
    }

    public void WriteSaveFile(SaveData data)
    {
        string filepath = Application.persistentDataPath + "/" + savefileName;
        FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);
        formatter = new BinaryFormatter();

        try
        {
            Debug.Log("Serializuje plik " + filepath);
            formatter.Serialize(fs, data);
        }
        catch (SerializationException e)
        {
            Debug.LogError("Save failded");
            throw;
        }
        finally
        {
            fs.Close();
        }
    }

    public string ReadSaveFileAsString()
    {
        string filepath = Application.persistentDataPath + "/" + savefileName;
        if (File.Exists(filepath))
        {
            return File.ReadAllText(filepath);
        }
        return "0";
    }

    public SaveData ReadSaveFile()
    {
        string filepath = Application.persistentDataPath + "/" + savefileName;
        FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);
        formatter = new BinaryFormatter();
        SaveData data = null;

        try 
        { 
            data = (SaveData)formatter.Deserialize(fs);
        }
        catch (SerializationException e)
        {
            Debug.Log("Błąd w czasie wczytywania gry. Restart");
        }
        finally
        {
            fs.Close();
        }

        return data;
    }

}



[System.Serializable]
public class SaveData
{
    public decimal money;
    public int[] creatures;

    public string[] creatureNames;
    public int[] spritesOrder;
}