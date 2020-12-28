using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveData(GameManager gamemanager)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + /*"/gjdata.ajd"*/"C:/Users/hp/Documents/Unity/GDL/Assets/SaveData/gjdata.dat";

        FileStream stream = new FileStream(path, FileMode.Create);

        //GlobalData data = new GlobalData(gamemanager);

        //formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GlobalData LoadData()
    {
        string path = Application.persistentDataPath + /*"/gjdata.ajd"*/"C:/Users/hp/Documents/Unity/GDL/Assets/SaveData/gjdata.dat";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GlobalData data = formatter.Deserialize(stream) as GlobalData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("No Save File Found");
            return null;
        }
    }
} 
