using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int level;
    public int earnedStarCount;
    public List<List<string>> indexes;


    public LevelData(int level_,int earnedStarCount_, List<List<string>> indexes_)
    {
        level = level_;
        earnedStarCount = earnedStarCount_;
        indexes = indexes_;
    }

}
