using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.Text;


public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] Board boardPref;
    [SerializeField] private int totalLevelCount;

    private void Start()
    {
        
        if (SaveSystem.CheckIfAllLevelIndexesDownloaded())
        {
            LevelBarsContainer.Instance.CreateLevelCellsToSelect(totalLevelCount);

        }
        else
        {
            TakeLevelsFromServer();
            LevelBarsContainer.Instance.CreateLevelCellsToSelect(totalLevelCount);

        }

        Board board=Instantiate(boardPref,new Vector3(0,0,0),Quaternion.identity);
        DontDestroyOnLoad(board);
    }

    public int GetTotalLevelCount()
    {

        return totalLevelCount;
    }
    public void TakeLevelsFromServer() //take levels from server
    {

        

        for (int i = 1; i < totalLevelCount+1; i++)
        {
            // take as string arr
            //so we have some kind of string arr like below
            string[,] indexes = { { "i", "j", "k" }, { "x", "y", "z" } };

            string pathForLevelDownload = "https://engineering-case-study.s3.eu-north-1.amazonaws.com/LS_Case_Level-" + i.ToString();

            WebClient webClient = new WebClient();
            byte[] data = webClient.DownloadData(pathForLevelDownload);
            string levelIndexes = Encoding.Default.GetString(data);

            string[] rowArray = levelIndexes.Split('\n'); // array is now like {"0,0,0,1" , "0,1,1,0"}


            List<List<string>> levelIndexesList = new List<List<string>>();

            for (int j = rowArray.Length - 1; j >=0; j--)
            {
                string[] currentRowIndexes = rowArray[j].Split(',');
                List<String> currentRowIndexesList = new List<string>();
        
                for (int k = 0; k < currentRowIndexes.Length; k++)
                {
                    currentRowIndexesList.Add(currentRowIndexes[k]);
                }
                levelIndexesList.Add(currentRowIndexesList);
            }


            SaveSystem.SaveLevelIndexesForFirstTime(i,levelIndexesList);
            //print level indexes
            /*
            Debug.Log(i+"----------------");
            for (int m = 0; m < levelIndexesList.Count; m++)
            {
                string sss = "";
                for (int l = 0; l < levelIndexesList[m].Count; l++)
                {
                    sss = sss + levelIndexesList[m][l];
                }
                Debug.Log(sss);
            }
            Debug.Log("----------------");
            */

            

        }
    }



}
