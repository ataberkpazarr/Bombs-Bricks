using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class LevelCell : MonoBehaviour
{
    [SerializeField] private GameObject filledStar1, filledStar2, filledStar3;
    [SerializeField] private GameObject unfilledStar1, unfilledStar2, unfilledStar3;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject lockedButton;
    [SerializeField] private GameObject progressBar;
    [SerializeField] private Slider sliderToFillProgressBar;
    [SerializeField] private TextMeshProUGUI progressText;




    [SerializeField] private Text levelText;

    private int level;

    
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

    public void ActivateProgressBar(int currentEarnedStars,int requiredStars)
    {
        unfilledStar1.SetActive(false);
        unfilledStar2.SetActive(false);
        unfilledStar3.SetActive(false);

        progressBar.SetActive(true);


        float currentRatio = (float)currentEarnedStars / (float)requiredStars;

        sliderToFillProgressBar.normalizedValue = currentRatio;
        sliderToFillProgressBar.onValueChanged.AddListener(delegate { PreventSliding(currentRatio); });

        progressText.text = currentEarnedStars.ToString() + "/" + requiredStars.ToString();

    }

    private void PreventSliding(float val) 
    {
        sliderToFillProgressBar.normalizedValue = val;
    }

    public void LoadLevelToPlay()
    {

        Debug.Log("s");
        LevelData levelData= SaveSystem.LoadLevel(level);
        List<List<string>> indexes = levelData.indexes;
        int y = indexes.Count;
        int x = indexes[0].Count;
        Board.Instance.SetCurrentBoardSpecifications(x,y,indexes,level,levelData.earnedStarCount);
        SceneManager.LoadScene("GamePlay");

    }

   
}
