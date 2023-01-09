using UnityEngine;
using System.Collections;

public class RewardController : MonoBehaviour
{

    // Use this for initialization

    //for star
    static int starFirst = 50;
    static float increaseStarPercentPerLevel = 10; //%
    static float increaseStarPercentPerStarGot = 15; //%

    //for gem
    static int gemFirst = 3;
    static int increaseGemPercenetPerLevel = 5; //%
    static float increaseGemPercentPerStarGot = 10; //%

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static int GetStarReward(int level, int starGotAtLevel)
    {
        int star = starFirst;
        star = (int)Master.IncreaseValues(star, level, increaseStarPercentPerLevel);
        star = (int)Master.IncreaseValues(star, starGotAtLevel, increaseStarPercentPerStarGot);

        Master.Stats.Star += star;
        return star;
    }

    public static int GetGemReward(int level, int starGotAtLevel)
    {
        float gem = gemFirst;
        gem = Master.IncreaseValues(gem, level, increaseGemPercenetPerLevel);
        gem = Master.IncreaseValues(gem, starGotAtLevel, increaseGemPercentPerStarGot);

        if (level <= Master.LevelData.lastLevel)
        {
            gem = gem * 0.3f;
        }

        Master.Stats.Gem += (int)gem;
        return (int)gem;
    }

}
