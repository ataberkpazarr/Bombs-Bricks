using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using System;
using System.Linq;

public class Board : Singleton<Board>
{
    private int width;
    private int height;
    private int currentLevel;
    [SerializeField] private Tile tileNormalPrefab;
    [SerializeField] private GameObject tileBrickPrefab;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject tickPrefab;

    public static Action onNewBombPlaced;


    Tile[,] allTiles;
    GameObject[,] allBricks;
    List<Tuple<int, int>> activeBrickIndexes;
    List<Tuple<int, int>> activeBrickIndexesToCalculateMinBomb;
    List<Tuple<int, int>> activeBrickPositionsDuringLevel;


    int totalRequiredBombs;
    List<List<string>> currentIndexes;

    List<GameObject> placedBombs;

    private int alreadyEarnedStarsForThisLevel;
    // Start is called before the first frame update
    void Start()
    {
        //allTiles = new Tile[width,height];
        SetupTiles(9, 9);

    }

    private void SetupTiles(int wid, int hei)
    {

        allTiles = new Tile[wid, hei];
        allBricks = new GameObject[wid, hei];
        for (int x = 0; x < wid; x++)
        {
            for (int y = 0; y < hei; y++)
            {
                Vector3 currentTilePos = new Vector3(x + x * 0.01f, y + y * 0.01f, 0);
                Tile tile = Instantiate(tileNormalPrefab, currentTilePos, Quaternion.identity);

                tile.name = "Tile (" + x.ToString() + "," + y.ToString() + ")";
                allTiles[x, y] = tile;
                tile.transform.parent = transform;
                tile.gameObject.SetActive(false);

                GameObject brick = Instantiate(tileBrickPrefab, currentTilePos, Quaternion.identity);
                allBricks[x, y] = brick;
                brick.transform.parent = transform;
                brick.SetActive(false);
            }
        }

        //SetupCamera(wid,hei);
    }
    
    public void SetCurrentBoardSpecifications(int x, int y, List<List<string>> currentIndexes_, int level,int earnedStars)
    {
        width = x;
        height = y;
        currentIndexes = currentIndexes_;
        currentLevel = level;
        totalRequiredBombs = 0;
        placedBombs = new List<GameObject>();
        alreadyEarnedStarsForThisLevel = earnedStars;
        //StartCoroutine(SetLevelRoutine());

        GamePlayCanvasController.Instance.SetCurrentBoardSpecifications(width,height,currentIndexes,currentLevel);
        //FindMinimumNumberOfBombs();
    }

    private IEnumerator SetLevelRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        SetupCamera();
        SetupBoard();

    }
    public void FindMinimumNumberOfBombs()
    {

        List<Tuple<int, int>> bricksToDestroy = activeBrickIndexes;
        //alttaki gibiydi, ürteriken length veriyodum
        //List<Tuple<Tuple<int, int>, List<Tuple<int, int>>>> brickAndItsPossibleBombPositions = new List<Tuple<Tuple<int, int>, List<Tuple<int, int>>>>(activeBrickIndexesToCalculateMinBomb.Count-1);
        List<Tuple<Tuple<int, int>, List<Tuple<int, int>>>> brickAndItsPossibleBombPositions = new List<Tuple<Tuple<int, int>, List<Tuple<int, int>>>>();


        //List<Tuple<int, int,List<Tuple<int,int>>>> brickAndItsPossibleBombPositions = new List<Tuple<int, int, List<Tuple<int, int>>>>(activeBrickIndexes.Count-1);
        /*
        for (int i = 0; i < brickAndItsPossibleBombPositions.Count; i++)
        {
            brickAndItsPossibleBombPositions[i] = Tuple.Create(activeBrickIndexesToCalculateMinBomb[i], new List<Tuple<int, int>>());
            
            
        }
        */
        for (int i = 0; i < activeBrickIndexesToCalculateMinBomb.Count; i++)
        {/*
            brickAndItsPossibleBombPositions[i].Item1 = activeBrickIndexes[i];
            brickAndItsPossibleBombPositions[i].Item2 = activeBrickIndexes[i].Item2;
            */
            //brickAndItsPossibleBombPositions[i] = Tuple.Create(activeBrickIndexes[i].Item1, activeBrickIndexes[i].Item2,new List<Tuple<int, int>>());
            //var k = activeBrickIndexesToCalculateMinBomb[i];
            brickAndItsPossibleBombPositions.Add (Tuple.Create(activeBrickIndexesToCalculateMinBomb[i], new List<Tuple<int, int>>()));
            //brickAndItsPossibleBombPositions[i] = Tuple.Create(Tuple.Create(activeBrickIndexesToCalculateMinBomb[i].Item1, activeBrickIndexesToCalculateMinBomb[i].Item2), new List<Tuple<int, int>>());
            Debug.Log(brickAndItsPossibleBombPositions[i].Item1);
        }



        List<Tuple<int, int>> possibleBombLocations = new List<Tuple<int, int>>();
        //List<Tuple<int,Tuple<int, int>>> bricksAndPossibleDestroyPositions = new List<Tuple<int,Tuple<int, int>>>();
        //////List<List<Tuple<int, int>>> bricksAndPossibleBombPositions = new List<List<Tuple<int, int>>>(activeBrickIndexesToCalculateMinBomb.Count-1);
        List<Tuple<Tuple<int, int>, int>> bombPositionsAndTheirCasualities = new List<Tuple<Tuple<int, int>, int>>();
        //List < Tuple<Tuple>>
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

                   // List<Tuple<int, int,List<Tuple<int,int>>>> brickAndItsPossibleBombPositions
                    for (int m = 0; m < affectedBricksByRelevantBomb.Count; m++)
                    {
                        
                        //Debug.Log(",,,"+brickAndItsPossibleBombPositions[m].Item1);
                        //Debug.Log(affectedBricksByRelevantBomb[m]);
                        int index = brickAndItsPossibleBombPositions.FindIndex(t => t.Item1.Item1 ==affectedBricksByRelevantBomb[m].Item1 && t.Item1.Item2 == affectedBricksByRelevantBomb[m].Item2);
                        
                        Debug.Log(index);
                        if (index>=0)
                        {
                            brickAndItsPossibleBombPositions[index].Item2.Add(possibleBombPositions[k]); //sorun olabilir
                            BombsAndAffectedBrickPositions.Last().Item2.Add(affectedBricksByRelevantBomb[m]);
                        }
                        
                        

                    }

                    Tuple<Tuple<int, int>, int> newBombToAddToList  = Tuple.Create(possibleBombPositions[k], affectedBricksByRelevantBomb.Count);
                    ////////////////////böyleydi bombPositionsAndTheirCasualities[i] = Tuple.Create(possibleBombPositions[k],affectedBricksByRelevantBomb.Count);
                    if (!bombPositionsAndTheirCasualities.Contains(newBombToAddToList))
                    {
                        //bombPositionsAndTheirCasualities.Add(Tuple.Create(possibleBombPositions[k], affectedBricksByRelevantBomb.Count));
                        bombPositionsAndTheirCasualities.Add(newBombToAddToList);


                    }
                    //bricksAndPossibleBombPositions[i] = affectedBricksByRelevantBomb;

                    /*
                    int m = GetTotalAmountOfDestroyedBricksForThisBomb(possibleBombPositions[k]);
                    DestroyedBrickCounts[k] = m;
                    */
                }
            }

            /*
            int maxIndex = Array.IndexOf(DestroyedBrickCounts, DestroyedBrickCounts.Max());


            possibleBombLocations.Add(possibleBombPositions[maxIndex]);
            */

        }
        for (int p = 0; p < bombPositionsAndTheirCasualities.Count; p++)
        {
            Debug.Log(bombPositionsAndTheirCasualities[p]);

        }
        int maxCasualities=bombPositionsAndTheirCasualities.Max(t => t.Item2);

        int index_ = bombPositionsAndTheirCasualities.FindIndex(t=>t.Item2==maxCasualities);
        Debug.Log("max cas"+maxCasualities.ToString());

        Debug.Log("sec index"+index_.ToString());

        ////bombPositionsAndTheirCasualities.RemoveAt(index_);
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
                    Debug.Log("aaaaaa");
                }
            }
        }
        totalRequiredBombs = totalRequiredBombs + 1;
        if (activeBrickIndexesToCalculateMinBomb.Count<=0)
        {
            GamePlayCanvasController.Instance.SetTotalBombAmount(totalRequiredBombs+2);
            Debug.Log("eee"+totalRequiredBombs.ToString());
        }
        else
        {
            FindMinimumNumberOfBombs();
        }



    }

    private List<Tuple<int, int>> GetBrickPositionsToBeDestroyed(Tuple<int,int> bombPos)
    {
        List < Tuple<int, int> > affectedBricksByRelevantBomb = new List<Tuple<int, int>>();

        //int i = 0;

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
        private void FindMinimumNumberOfBombsForThisLevel()
    {
        List<Tuple<int, int>> possibleBombLocations = new List<Tuple<int, int>>();
        for (int i = 0; i < activeBrickIndexes.Count; i++)
        {
            Tuple<int, int> brickPosition = activeBrickIndexes[i];

            Tuple<int, int> possibleBombPosition1 = Tuple.Create(activeBrickIndexes[i].Item1 + 1, activeBrickIndexes[i].Item2);
            Tuple<int, int> possibleBombPosition2 = Tuple.Create(activeBrickIndexes[i].Item1 - 1, activeBrickIndexes[i].Item2);
            Tuple<int, int> possibleBombPosition3 = Tuple.Create(activeBrickIndexes[i].Item1, activeBrickIndexes[i].Item2 + 1);
            Tuple<int, int> possibleBombPosition4 = Tuple.Create(activeBrickIndexes[i].Item1, activeBrickIndexes[i].Item2 - 1);

            Tuple<int, int>[] possibleBombPositions = new Tuple<int, int>[4];
            possibleBombPositions[0]=possibleBombPosition1;
            possibleBombPositions[1] = possibleBombPosition2;
            possibleBombPositions[2] = possibleBombPosition3;
            possibleBombPositions[3] = possibleBombPosition4;


            int[] DestroyedBrickCounts = new int[4];
            for (int k = 0; k < 4; k++)
            {
                if (CheckIfCoordinateWithinTheBoard(possibleBombPositions[k]))
                {
                    int m = GetTotalAmountOfDestroyedBricksForThisBomb(possibleBombPositions[k]);
                    DestroyedBrickCounts[k] = m;
                }
            }

            int maxIndex = Array.IndexOf(DestroyedBrickCounts,DestroyedBrickCounts.Max());


            possibleBombLocations.Add(possibleBombPositions[maxIndex]);

        }
    }

    private int GetTotalAmountOfDestroyedBricksForThisBomb(Tuple<int,int> coordinate)
    {
        int i = 0;

        Tuple<int, int> explosionLocation1 = Tuple.Create(activeBrickIndexes[i].Item1 + 1, activeBrickIndexes[i].Item2);
        Tuple<int, int> explosionLocation2 = Tuple.Create(activeBrickIndexes[i].Item1 - 1, activeBrickIndexes[i].Item2);
        Tuple<int, int> explosionLocation3 = Tuple.Create(activeBrickIndexes[i].Item1 , activeBrickIndexes[i].Item2 + 1);
        Tuple<int, int> explosionLocation4 = Tuple.Create(activeBrickIndexes[i].Item1 , activeBrickIndexes[i].Item2 - 1);

        Tuple<int, int>[] possibleExlosionPositions = new Tuple<int, int>[4];
        possibleExlosionPositions[0] = explosionLocation1;
        possibleExlosionPositions[1] = explosionLocation2;
        possibleExlosionPositions[2] = explosionLocation3;
        possibleExlosionPositions[3] = explosionLocation4;

        for (int k = 0; k < 4; k++)
        {
            if (CheckIfCoordinateWithinTheBoard(possibleExlosionPositions[k]) && (CheckIfCoordinateHaveBrick(explosionLocation1)) )
            {
                
                    i += 1;
                
            }
        }

        
        return i;
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
    //coordinate1.Item1+1==coordinate2.Item1 || coordinate1.Item2 - 1 == coordinate2.Item2 || coordinate1.Item2 + 1 == coordinate2.Item2)
    private bool checkIfAdjacentOrNot(Tuple<int,int> coordinate1, Tuple<int, int> coordinate2)
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

    public void SetupBoard()//used to be int wid, int hei
    {
        //width = wid;
        //height = hei;

        /*
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                allTiles[x, y].gameObject.SetActive(true);
            }
        }*/
        /*
        Debug.Log("width"+width.ToString());
        Debug.Log("height" + height.ToString());

        Debug.Log();
        Debug.Log();
        */
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

        //FindMinimumNumberOfBombs();
    }
    public void SetupCamera() // used to be int wid, int hei
    {

        GamePlayCanvasController.Instance.UpdateLevelText(currentLevel);
        Camera.main.transform.position = new Vector3((float)(width - 1)/2, (float)(height - 1) /2,-10f);

        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticalSize = (float)height / 2f + 2f;//1f for border size
        //float horizontalSize = (float)width / 2f + 4f;//1f for border size
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

    private void OnScreenTouchDown(LeanFinger leanFinger)
    {
        //leanFinger.GetRay
            Ray ray = leanFinger.GetStartRay();
        
        
        //Physics2D.Raycast();
 
        //(Physics.Raycast(ray, out hitInfo_))


        //2d collider


        
        
        //RaycastHit2D hitNormalTile = Physics2D.GetRayIntersection(ray,LayerMask.GetMask("TileNormal"));
        RaycastHit2D hitNormalTile = Physics2D.GetRayIntersection(ray);

        /*
        if (hitBrickTile.collider==null && hitNormalTile.collider!=null)
        {


            if (hitNormalTile.collider.gameObject.CompareTag("TileNormal"))
            {
                Instantiate(bombPrefab, hitNormalTile.collider.gameObject.transform.position,Quaternion.identity);

                //Debug.Log(hit_.transform.name);

            }
        }
        */
        if (hitNormalTile&& hitNormalTile.collider.gameObject.CompareTag("TileNormal"))
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
        /*
        if (hit)
        {
           
            if (hit.collider.gameObject.CompareTag("TileNormal"))
            {
                Debug.Log("aaa");
                Instantiate(bombPrefab,hit.collider.gameObject.transform);
            }
        }
        */

        //3d collider
        /*
        if (Physics.Raycast(ray, out hitInfo_))
        {

            if (hitInfo_.collider.gameObject.CompareTag("TileNormal"))
            {
                Debug.Log("aaa");
                Instantiate(bombPrefab, hitInfo_.collider.gameObject.transform);
            }
        }*/
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

        if (activeBrickIndexes.Count==0)
        {
            Debug.Log("all destroyed");
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
            //StartCoroutine(LoadLevelEndSpriteRoutine(true,earnedStars));
            /*
            if (remainingBombs==0)
            {
                
                StartCoroutine(LoadLevelEndSpriteRoutine(true,1));
            }
            else if (remainingBombs==1)
            {
                StartCoroutine(LoadLevelEndSpriteRoutine(true, 2));

            }
            else if (remainingBombs==2)
            {
                StartCoroutine(LoadLevelEndSpriteRoutine(true, 3));

            }*/

        }
        else if (activeBrickIndexes.Count>0 &&GamePlayCanvasController.Instance.GetRemainingAmountOfBombs()==0)
        {
            //gameover
            GamePlayCanvasController.Instance.LoadLevelEndSprite(false, 0);
            //StartCoroutine(LoadLevelEndSpriteRoutine(false, 0));

        }
    }


    private IEnumerator LoadLevelEndSpriteRoutine(bool isItWin,int earnedStars)
    {

        yield return new WaitForSeconds(0.2f);
        
            GamePlayCanvasController.Instance.LoadLevelEndSprite(isItWin,earnedStars);
        
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
