using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelCell : MonoBehaviour
{
    [SerializeField] private GameObject filledStar1, filledStar2, filledStar3;
    [SerializeField] private GameObject unfilledStar1, unfilledStar2, unfilledStar3;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject lockedButton;


    [SerializeField] private Text levelText;

    private int level;
    private int earnedStarCount;
    private bool isLevelLocked;
    
    public void FillStars(int totalStarNumToFill)
    {
        if (totalStarNumToFill==1)
        {
            unfilledStar1.SetActive(false);
            filledStar1.SetActive(true);
        }
        else if (totalStarNumToFill==2)
        {
            unfilledStar1.SetActive(false);
            unfilledStar2.SetActive(false);

            filledStar1.SetActive(true);
            filledStar2.SetActive(true);

        }
        else if (totalStarNumToFill==3)
        {
            unfilledStar1.SetActive(false);
            unfilledStar2.SetActive(false);
            unfilledStar3.SetActive(false);

            filledStar1.SetActive(true);
            filledStar2.SetActive(true);
            filledStar3.SetActive(true);

        }
    }

    public void UpdateLevelText(int level_)
    {
        level = level_;
        string levText = "Level-" + level_.ToString();
        levelText.text = levText;
    }

    public void SetPlayButtonActive()
    {
        lockedButton.SetActive(false);
        playButton.SetActive(true);


    }

    public void SetLockedButtonActive()
    {
        playButton.SetActive(false);
        lockedButton.SetActive(true);

    }
}
