using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelBarsContainer : Singleton<LevelBarsContainer>
{
    [SerializeField] private LevelCell levelBarPrefab;
    private bool lastUnlockedLevelFound = false;
    private int totalEarnedStarCount;

    public void CreateLevelCellsToSelect(int totalLevelCount)
    {
        for (int i = 1; i < totalLevelCount+1; i++)
        {
            LevelData lev = SaveSystem.LoadLevel(i);
            LevelCell levelCell = Instantiate(levelBarPrefab, transform);

            int earnedStarCountOfThisLevel = lev.earnedStarCount;
            totalEarnedStarCount = totalEarnedStarCount + earnedStarCountOfThisLevel;
            levelCell.FillStars(earnedStarCountOfThisLevel);

            levelCell.UpdateLevelText(i);

            if (lev.earnedStarCount>0)
            {
                levelCell.SetPlayButtonActive();

            }
            else if (!lastUnlockedLevelFound && earnedStarCountOfThisLevel == 0)
            {
                if (i%5==0) //syntax hatası olabilir
                {
                    if ((totalEarnedStarCount / (i-1)) >= 2)//syntax hatası olabilir
                    {
                        levelCell.SetPlayButtonActive();

                    }
                    else
                    {
                        levelCell.ActivateProgressBar(totalEarnedStarCount,((i-1)*2));
                        //time to fillable progress bar 
                    }
                }
                else
                {
                    levelCell.SetPlayButtonActive();
                    
                }
                lastUnlockedLevelFound = true;
            }
            else
            {
                levelCell.SetLockedButtonActive();
            }
            //levelCell.SetLockedButtonActive();

        }
        /*
        Instantiate(levelBarPrefab, transform);
        Instantiate(levelBarPrefab, transform);
        Instantiate(levelBarPrefab, transform);
        Instantiate(levelBarPrefab, transform);
        Instantiate(levelBarPrefab, transform);
        Instantiate(levelBarPrefab, transform);
        Instantiate(levelBarPrefab, transform);
        Instantiate(levelBarPrefab, transform);
        Instantiate(levelBarPrefab, transform);
        Instantiate(levelBarPrefab, transform);
        Instantiate(levelBarPrefab, transform);
        Instantiate(levelBarPrefab, transform);
        Instantiate(levelBarPrefab, transform);
        Instantiate(levelBarPrefab, transform);
        Instantiate(levelBarPrefab, transform);
        Instantiate(levelBarPrefab, transform);
        */
        


        ScrollRect sc = gameObject.GetComponentInParent<ScrollRect>();
        sc.verticalNormalizedPosition = 1; // scroll current view to the beginning of level list

    }

    public void DeleteAllPersistentData()
    {

        SaveSystem.DeleteAllPersistentData();
        SceneManager.LoadScene("LevelSelection");

    }

  
}
