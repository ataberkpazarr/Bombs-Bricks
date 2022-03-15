using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GamePlayCanvasController : Singleton<GamePlayCanvasController>
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI bombText;
    [SerializeField] LevelEndSprite levelEndSprite;


    private int totalAmountOfBomb;


    public void GoBackToLevelSelectionScene()
    {

        SceneManager.LoadScene("LevelSelection");
        Board.Instance.DeActivateBoard();
    }

    public void UpdateLevelText(int currentLevel)
    {
        levelText.text = "Level-"+currentLevel.ToString();
        levelText.gameObject.SetActive(true);
    }

    public void DecreaseRemainingBombAmount()
    {
        totalAmountOfBomb -= 1;
        bombText.text = totalAmountOfBomb.ToString();

    }
  

    public void SetTotalBombAmount(int totalBombForThisLevel)
    {
        totalAmountOfBomb = totalBombForThisLevel;
        bombText.text = totalAmountOfBomb.ToString();

    }

    public void LoadLevelEndSprite(bool isItWin,int earnedStars)
    {
        StartCoroutine(LoadLevelEndSpriteRoutine(isItWin,earnedStars));
       
        
    }

    private IEnumerator LoadLevelEndSpriteRoutine(bool isItWin,int earnedStars)
    {
        yield return new WaitForSeconds(1f);
        levelEndSprite.gameObject.SetActive(true);
        levelEndSprite.SetLevelEndSprite(isItWin, earnedStars);
    }

    public void PlayThisLevelAgain()
    {
        Board.Instance.DeActivateBoard();

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);



    }

    public int GetRemainingAmountOfBombs()
    {
        return totalAmountOfBomb;
    }

  
}
