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
    private int width, height;
    private List<List<string>> currentIndexes;
    private int currentLevel;

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
    public void SetCurrentBoardSpecifications(int x, int y, List<List<string>> currentIndexes_, int level)
    {
        width = x;
        height = y;
        currentIndexes = currentIndexes_;
        currentLevel = level;
    }

    public void SetTotalBombAmount(int totalBombForThisLevel)
    {
        totalAmountOfBomb = totalBombForThisLevel;
        bombText.text = totalAmountOfBomb.ToString();

    }

    public void LoadLevelEndSprite(bool isItWin,int earnedStars)
    {
            Debug.Log("aaa");
            levelEndSprite.gameObject.SetActive(true);
            levelEndSprite.SetLevelEndSprite(isItWin,earnedStars);
        
    }
   

    public void PlayThisLevelAgain()
    {
        Board.Instance.DeActivateBoard();
        //Scene scene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(scene.name);
        //Board.Instance.SetCurrentBoardSpecifications(width, height, currentIndexes, currentLevel);
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        
        //SceneManager.LoadScene("GamePlay");


    }

    public int GetRemainingAmountOfBombs()
    {
        return totalAmountOfBomb;
    }

    private void OnEnable()
    {
        //Board.onNewBombPlaced += DecreaseRemainingBombAmount;
    }
    private void OnDisable()
    {
        //Board.onNewBombPlaced -= DecreaseRemainingBombAmount;


    }
}
