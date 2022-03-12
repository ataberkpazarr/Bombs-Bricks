using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelBarsContainer : Singleton<LevelBarsContainer>
{
    [SerializeField] private LevelCell levelBarPrefab;
    // Start is called before the first frame update
    void Start()
    {


        

    }

    public void CreateLevelCellsToSelect(int totalLevelCount)
    {
        for (int i = 1; i < totalLevelCount+1; i++)
        {
            LevelData lev = SaveSystem.LoadLevel(i);
            LevelCell levelCell = Instantiate(levelBarPrefab, transform);
            levelCell.FillStars(lev.earnedStarCount);
            levelCell.UpdateLevelText(i);
            levelCell.SetPlayButtonActive();
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

  
}
