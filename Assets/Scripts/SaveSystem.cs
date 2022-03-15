using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public static class SaveSystem
{

    public static void SaveLevelIndexesForFirstTime(int level, List<List<string>> indexes)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/level" + level.ToString() + ".txt";
        FileStream stream = new FileStream(path, FileMode.Create);


        //string[,] indexes = { { "i", "j", "k" }, { "x", "y", "z" } }; 
        LevelData data = new LevelData(level,0,indexes); // 0 for earned level stars at the initial

        formatter.Serialize(stream,data);
        stream.Close();
    }

    public static void SaveLevelEarnedStars(int level, int earnedStarCount_)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/level" + level.ToString() + ".txt";
      
        LevelData alreadySavedLevelData = LoadLevel(level);
        alreadySavedLevelData.earnedStarCount = earnedStarCount_;

        if (File.Exists(path))
        {
            File.Delete(path);

        }
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, alreadySavedLevelData);
        stream.Close();
    }


    public static LevelData LoadLevel(int level)
    {
        string path = Application.persistentDataPath + "/level" + level.ToString() + ".txt";

        if (File.Exists(path)) 
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path,FileMode.Open);

            LevelData leveldata= formatter.Deserialize(stream) as LevelData;  //change back from binary to old readable format
            stream.Close();
            return leveldata;
        }
        else
        {
            Debug.Log("save file not found at" + path);
            return null;
        }

    }

    public static bool CheckIfAllLevelIndexesDownloaded()
    {
        int totalLevelCount = LevelManager.Instance.GetTotalLevelCount();
        string path; 
        for (int i = 1; i < totalLevelCount+1; i++)
        {
            path = Application.persistentDataPath + "/level" + i.ToString() + ".txt";
            if (File.Exists(path))
            {
                continue;
            }
            else
            {
                return false;
            }

        }

        return true;

    }

    public static void DeleteAllPersistentData()
    {
        int totalLevelCount = LevelManager.Instance.GetTotalLevelCount();
        for (int i = 1; i < totalLevelCount+1; i++)
        {
            string path = Application.persistentDataPath + "/level" + i.ToString() + ".txt";
            if (File.Exists(path))
            { 
                File.Delete(path);

            }
        }
    }

}
