using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public static class SaveManager
{
    private static string GetFilepath(string savefileName)
    {
        return Application.persistentDataPath + "/" + savefileName;
    }

    public static void WriteSaveFile(string contents, string savefileName)
    {
        string filepath = GetFilepath(savefileName);
        File.WriteAllText(filepath, contents);
    }

    public static void WriteSaveFile<T>(T data, string savefileName)
    {
        return;
        string filepath = GetFilepath(savefileName);
        FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);
        BinaryFormatter formatter = new BinaryFormatter();

        try
        {
            Debug.Log("Serializing " + filepath);
            formatter.Serialize(fs, data);
        }
        catch (SerializationException e)
        {
            Debug.LogError("Save failded! " + e);
            throw;
        }
        finally
        {
            fs.Close();
        }
    }

    public static string ReadSaveFileAsString(string savefileName)
    {
        string filepath = GetFilepath(savefileName);
        if (File.Exists(filepath))
        {
            return File.ReadAllText(filepath);
        }
        return "";
    }

    public static T ReadSaveFile<T>(string savefileName)
    {
        return default(T);
        string filepath = GetFilepath(savefileName);
        FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);
        BinaryFormatter formatter = new BinaryFormatter();
        T data = default;

        try
        {
            data = (T)formatter.Deserialize(fs);
        }
        catch (SerializationException e)
        {
            Debug.LogWarning("Loading error! " + e);
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
    public int[] creatureShopGrades;
}