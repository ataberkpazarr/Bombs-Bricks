using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSetup : MonoBehaviour
{
    private void Start()
    { 
        Board.Instance.SetupCamera();
        Board.Instance.SetupBoard();
        Board.Instance.FindMinimumNumberOfBombs();
    }
}
