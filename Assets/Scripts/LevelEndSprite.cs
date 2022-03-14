using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelEndSprite : MonoBehaviour
{
    [SerializeField] GameObject star1,star2,star3;
    [SerializeField] GameObject unfilledStar1, unfilledStar2, unfilledStar3;

    [SerializeField] TextMeshProUGUI HeaderText;



    public void SetLevelEndSprite(bool isItWin,int earnedStars)
    {
        if (isItWin)
        {
            HeaderText.text = "CONGRATS!";
            if (earnedStars==1)
            {

                star1.SetActive(true);
                unfilledStar2.SetActive(true);
                unfilledStar3.SetActive(true);

                star2.SetActive(false);
                star3.SetActive(false);


            }
            else if (earnedStars == 2)
            {

                star1.SetActive(true);
                star2.SetActive(true);
                unfilledStar3.SetActive(true);

                star3.SetActive(false);

            }
            else if (earnedStars == 3)
            {

                star1.SetActive(true);
                star2.SetActive(true);
                star3.SetActive(true);

            }
        }
        else
        {
            HeaderText.text = "GAMEOVER!";
            unfilledStar1.SetActive(true);
            unfilledStar2.SetActive(true);
            unfilledStar3.SetActive(true);

        }
    }
}
