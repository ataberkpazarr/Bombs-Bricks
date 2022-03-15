using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using System;
using System.Linq;

public class Board : Singleton<Board>
{
    
    [SerializeField] private GameObject tileNormalPrefab;
    [SerializeField] private GameObject tileBrickPrefab;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject tickPrefab;

    public static Action onNewBombPlaced;


    GameObject[,] allTiles;
    GameObject[,] allBricks;

    List<Tuple<int, int>> activeBrickIndexes;
    List<Tuple<int, int>> activeBrickIndexesToCalculateMinBomb;
    List<Tuple<int, int>> activeBrickPositionsDuringLevel;

    List<List<string>> currentIndexes;
    List<GameObject> placedBombs;

    private int alreadyEarnedStarsForThisLevel;
    private int width;
    private int height;
    private int currentLevel;
    private int totalRequiredBombs;


    void Start()
    {
       
        SetupTiles(9, 9); 

    }
    // setup a 9x9 board since it is maximum size and deactivate them all. The logic is, creating all tiles once and
    // activate required tiles and reloacte the camera, according to level requirements
    private void SetupTiles(int wid, int hei)
    {

        allTiles = new GameObject[wid, hei];
        allBricks = new GameObject[wid, hei];
        for (int x = 0; x < wid; x++)
        {
            for (int y = 0; y < hei; y++)
            {
                Vector3 currentTilePos = new Vector3(x + x * 0.01f, y + y * 0.01f, 0);
                GameObject tile = Instantiate(tileNormalPrefab, currentTilePos, Quaternion.identity);

                tile.name = "Tile (" + x.ToString() + "," + y.ToString() + ")"; //for the ease of debugging
                allTiles[x, y] = tile;
                tile.transform.parent = transform;
                tile.gameObject.SetActive(false);

                GameObject brick = Instantiate(tileBrickPrefab, currentTilePos, Quaternion.identity);
                allBricks[x, y] = brick;
                brick.transform.parent = transform;
                brick.SetActive(false);
            }
        }


    }
    
    public void SetCurrentBoardSpecifications(int x, int y, List<List<string>> currentIndexes_, int level,int earnedStars) //resetting board
    {
        width = x;
        height = y;
        currentIndexes = currentIndexes_;
        currentLevel = level;
        totalRequiredBombs = 0;
        placedBombs = new List<GameObject>();
        alreadyEarnedStarsForThisLevel = earnedStars;

    }

    // this function fins minimum number of bombs for the current level, it calls itself recursively
    // the main logic is as following;
    /*
     * 
     1) find all possible bomb locations which will destroy current level's bricks which are been on activeBrickIndexesToCalculateMinBomb list

     2)  pair possible bomb location and its casualty in bombPositionsAndTheirCasualities,
     for example if a bomb exists at (2,2) and it is able to destroy 3 bricks then our pair will be (2,2),3

     3) find the bomb location which has max casualty, check which bricks are being destroyed by that bomb,
      remove those bricks from activeBrickIndexesToCalculateMinBomb list and call this function again with the
      updated activeBrickIndexesToCalculateMinBomb list

    //this logic will continue recursively until activeBrickIndexesToCalculateMinBomb list will reach to 0 length

    */
    public void FindMinimumNumberOfBombs() 
    {

        List<Tuple<Tuple<int, int>, List<Tuple<int, int>>>> brickAndItsPossibleBombPositions = new List<Tuple<Tuple<int, int>, List<Tuple<int, int>>>>();


        for (int i = 0; i < activeBrickIndexesToCalculateMinBomb.Count; i++)
        {
            brickAndItsPossibleBombPositions.Add (Tuple.Create(activeBrickIndexesToCalculateMinBomb[i], new List<Tuple<int, int>>()));
        }



        List<Tuple<int, int>> possibleBombLocations = new List<Tuple<int, int>>();
        
        List<Tuple<Tuple<int, int>, int>> bombPositionsAndTheirCasualities = new List<Tuple<Tuple<int, int>, int>>();

        List<Tuple<Tuple<int, int>, List<Tuple<int, int>>>> BombsAndAffectedBrickPositions = new List<Tuple<Tuple<int, int>, List<Tuple<int, int>>>>();

        for (int i = 0; i < activeBrickIndexesToCalculateMinBomb.Count; i++)
        {
            Tuple<int, int> brickPosition = activeBrickIndexesToCalculateMinBomb[i];

            Tuple<int, int> possibleBombPosition1 = Tuple.Create(activeBrickIndexesToCalculateMinBomb[i].Item1 + 1, activeBrickIndexesToCalculateMinBomb[i].Item2);
            Tuple<int, int> possibleBombPosition2 = Tuple.Create(activeBrickIndexesToCalculateMinBomb[i].Item1 - 1, activeBrickIndexesToCalculateMinBomb[i].Item2);
            Tuple<int, int> possibleBombPosition3 = Tuple.Create(activeBrickIndexesToCalculateMinBomb[i].Item1, activeBrickIndexesToCalculateMinBomb[i].Item2 + 1);
            Tuple<int, int> possibleBombPosition4 = Tuple.Create(activeBrickIndexesToCalculateMinBomb[i].Item1, activeBrickIndexesToCalculateMinBomb[i].Item2 - 1);

            Tuple<int, int>[] possibleBombPositions = new Tuple<int, int>[4];
            possibleBombPositions[0] = possibleBombPosition1;
            possibleBombPositions[1] = possibleBombPosition2;
            possibleBombPositions[2] = possibleBombPosition3;
            possibleBombPositions[3] = possibleBombPosition4;


            int[] DestroyedBrickCounts = new int[4];
            for (int k = 0; k < 4; k++)
            {
                if (CheckIfCoordinateWithinTheBoard(possibleBombPositions[k]) && !CheckIfCoordinateHaveBrick(possibleBombPositions[k]))
                {
                    BombsAndAffectedBrickPositions.Add(Tuple.Create(possibleBombPositions[k], new List<Tuple<int, int>>()));

                    possibleBombLocations.Add(possibleBombPositions[k]);
                    List<Tuple<int, int>> affectedBricksByRelevantBomb = GetBrickPositionsToBeDestroyed(possibleBombPositions[k]);

                  
                    for (int m = 0; m < affectedBricksByRelevantBomb.Count; m++)
                    {
                        
                        int index = brickAndItsPossibleBombPositions.FindIndex(t => t.Item1.Item1 ==affectedBricksByRelevantBomb[m].Item1 && t.Item1.Item2 == affectedBricksByRelevantBomb[m].Item2);
                        
                        
                        if (index>=0) // if it found the index
                        {
                            brickAndItsPossibleBombPositions[index].Item2.Add(possibleBombPositions[k]);
                            BombsAndAffectedBrickPositions.Last().Item2.Add(affectedBricksByRelevantBomb[m]);
                        }
                        

                    }

                    Tuple<Tuple<int, int>, int> newBombToAddToList  = Tuple.Create(possibleBombPositions[k], affectedBricksByRelevantBomb.Count);
                  
                    if (!bombPositionsAndTheirCasualities.Contains(newBombToAddToList))
                    {

                        bombPositionsAndTheirCasualities.Add(newBombToAddToList);


                    }
  
                }
            }



        }
    
        int maxCasualities=bombPositionsAndTheirCasualities.Max(t => t.Item2);

        int index_ = bombPositionsAndTheirCasualities.FindIndex(t=>t.Item2==maxCasualities);

        Tuple<int,int> pos =bombPositionsAndTheirCasualities[index_].Item1;
        
        int index_2 = BombsAndAffectedBrickPositions.FindIndex(t => t.Item1 == pos);



        List<Tuple<int,int>> bricksToBeDestrroyed= BombsAndAffectedBrickPositions[index_2].Item2;

        for (int l = 0; l < bricksToBeDestrroyed.Count; l++)
        {
            for (int i = 0; i < activeBrickIndexesToCalculateMinBomb.Count; i++)
            {
                if (bricksToBeDestrroyed[l].Item1==activeBrickIndexesToCalculateMinBomb[i].Item1 && bricksToBeDestrroyed[l].Item2 == activeBrickIndexesToCalculateMinBomb[i].Item2)
                {
                    activeBrickIndexesToCalculateMinBomb.RemoveAt(i);
                 
                }
            }
        }


        totalRequiredBombs = totalRequiredBombs + 1;

        if (activeBrickIndexesToCalculateMinBomb.Count<=0) // found min required bomb number, sat it to canvas controller so that it can be seen at the gameplay scene
        {
            GamePlayCanvasController.Instance.SetTotalBombAmount(totalRequiredBombs+2);
        }
        else
        {
            FindMinimumNumberOfBombs();
        }



    }

    private List<Tuple<int, int>> GetBrickPositionsToBeDestroyed(Tuple<int,int> bombPos) // return the bricks that will be destroyed by the specific bombPos
    {
        List < Tuple<int, int> > affectedBricksByRelevantBomb = new List<Tuple<int, int>>();


        Tuple<int, int> explosionLocation1 = Tuple.Create(bombPos.Item1 + 1, bombPos.Item2);
        Tuple<int, int> explosionLocation2 = Tuple.Create(bombPos.Item1 - 1, bombPos.Item2);
        Tuple<int, int> explosionLocation3 = Tuple.Create(bombPos.Item1, bombPos.Item2 + 1);
        Tuple<int, int> explosionLocation4 = Tuple.Create(bombPos.Item1, bombPos.Item2 - 1);

        Tuple<int, int>[] possibleExlosionPositions = new Tuple<int, int>[4];
        possibleExlosionPositions[0] = explosionLocation1;
        possibleExlosionPositions[1] = explosionLocation2;
        possibleExlosionPositions[2] = explosionLocation3;
        possibleExlosionPositions[3] = explosionLocation4;

        for (int k = 0; k < 4; k++)
        {
            if (CheckIfCoordinateWithinTheBoard(possibleExlosionPositions[k]) && (CheckIfCoordinateHaveBrick(possibleExlosionPositions[k])))
            {

                affectedBricksByRelevantBomb.Add(possibleExlosionPositions[k]);

            }
        }


        return affectedBricksByRelevantBomb;

    }
        

    

    private bool CheckIfCoordinateHaveBrick(Tuple<int,int> coordinate)
    {
        if (allBricks[coordinate.Item1,coordinate.Item2].gameObject.activeInHierarchy)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckIfCoordinateWithinTheBoard(Tuple<int, int> coordinate)
    {

        if (coordinate.Item1 >= 0 &&coordinate.Item2>=0 &&coordinate.Item1<width && coordinate.Item2<height)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    private bool CheckIfAdjacentOrNot(Tuple<int,int> coordinate1, Tuple<int, int> coordinate2)
    {
        if (Math.Abs(coordinate1.Item1 -coordinate2.Item1) == 1 && coordinate1.Item2 == coordinate2.Item2)
        {
            return true;
        }
        else if (Math.Abs(coordinate1.Item2 - coordinate2.Item2) == 1 && coordinate1.Item1 == coordinate2.Item1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetupBoard()
    {
        
        activeBrickIndexes =new List<Tuple<int, int>>();
        activeBrickIndexesToCalculateMinBomb =new List<Tuple<int, int>>();
        activeBrickPositionsDuringLevel = new List<Tuple<int, int>>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                allTiles[x, y].gameObject.SetActive(true);
                if (currentIndexes[y][x]=="1")
                {
                    allBricks[x, y].SetActive(true);
                    activeBrickIndexes.Add(Tuple.Create(x,y));
                    activeBrickIndexesToCalculateMinBomb.Add(Tuple.Create(x, y));
                    activeBrickPositionsDuringLevel.Add(Tuple.Create(x,y));



                }
            }
        }

    }
    public void SetupCamera() 
    {

        GamePlayCanvasController.Instance.UpdateLevelText(currentLevel);
        Camera.main.transform.position = new Vector3((float)(width - 1)/2, (float)(height - 1) /2,-10f);

        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticalSize = (float)height / 2f + 2f;//1f for border size
        float horizontalSize = (float)width+((float)width/5f);//1f for border size


        if (verticalSize>horizontalSize)
        {
            Camera.main.orthographicSize = verticalSize;
        }
        else
        {
            Camera.main.orthographicSize = horizontalSize;
        }


    }

    public void DeActivateBoard()
    {
        totalRequiredBombs = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                allTiles[x, y].gameObject.SetActive(false);
                if (currentIndexes[y][x] == "1")
                {
                    allBricks[x, y].SetActive(false);
                }
            }
        }

    }

    private void OnScreenTouchDown(LeanFinger leanFinger) // when user input comes
    {
    
        Ray ray = leanFinger.GetStartRay();
        
        
        RaycastHit2D hitNormalTile = Physics2D.GetRayIntersection(ray); // ray to touched position

       // if there exists bomb or brick at touched position, then user will not be able to put a bomb to that tile
       // it only will be able to put a bomb to that position, if there exist normal, empty tile at touched position

        if (hitNormalTile&& hitNormalTile.collider.gameObject.CompareTag("TileNormal")) //if ray collided to something and that something is a normal tile
        {


            if (GamePlayCanvasController.Instance.GetRemainingAmountOfBombs()>0)
            {
                GameObject newBomb=Instantiate(bombPrefab, hitNormalTile.collider.gameObject.transform.position, Quaternion.identity);
                placedBombs.Add(newBomb);

                GamePlayCanvasController.Instance.DecreaseRemainingBombAmount();

                int x = (int)newBomb.transform.position.x;
                int y = (int)newBomb.transform.position.y;

                Tuple<int, int> newBombPos = Tuple.Create(x,y);
                UpdateRemainingActiveBrickAmount(newBombPos);
                onNewBombPlaced?.Invoke();

            }
        }

    }

    private void UpdateRemainingActiveBrickAmount(Tuple<int,int> bombCoordinate)
    {
        List<Tuple<int, int>> affectedBricksByRelevantBomb = GetBrickPositionsToBeDestroyed(bombCoordinate);
        for (int i = 0; i < affectedBricksByRelevantBomb.Count; i++)
        {
            if (activeBrickIndexes.Contains(affectedBricksByRelevantBomb[i]))
            {
                activeBrickIndexes.Remove(affectedBricksByRelevantBomb[i]);
            }
        }

        if (activeBrickIndexes.Count==0) //win condition, calculate the earned stars
        {
            
            for (int i = 0; i < activeBrickPositionsDuringLevel.Count; i++)
            {
                Vector2 pos= new Vector2(activeBrickPositionsDuringLevel[i].Item1, activeBrickPositionsDuringLevel[i].Item2);
                Instantiate(tickPrefab,pos,Quaternion.identity);

            }

            int remainingBombs = GamePlayCanvasController.Instance.GetRemainingAmountOfBombs();
            int earnedStars;

            earnedStars = remainingBombs + 1;

            if (earnedStars>alreadyEarnedStarsForThisLevel)
            {
                SaveSystem.SaveLevelEarnedStars(currentLevel, earnedStars);
                alreadyEarnedStarsForThisLevel = earnedStars;
            }

            GamePlayCanvasController.Instance.LoadLevelEndSprite(true,earnedStars);


        }
        else if (activeBrickIndexes.Count>0 &&GamePlayCanvasController.Instance.GetRemainingAmountOfBombs()==0)//gameover
        {
            GamePlayCanvasController.Instance.LoadLevelEndSprite(false, 0);
           

        }
    }



    private void OnEnable()
    {
        LeanTouch.OnFingerDown += OnScreenTouchDown;

    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= OnScreenTouchDown;

    }
}
