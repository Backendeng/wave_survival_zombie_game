using UnityEngine;
using System.Collections;

public class LevelButton : MonoBehaviour
{

    // Use this for initialization
    public int levelIndex;
    public bool isActive;
    private UITexture bgTexture;
    private UITexture starTexture;
    private UILabel levelIndexLabel;


    void Start()
    {

        //SetAttribute();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SetAttribute(int levelIndex)
    {
        this.levelIndex = levelIndex;

        bgTexture = Master.GetChildByName(gameObject, "BG").GetComponent<UITexture>();
        starTexture = Master.GetChildByName(gameObject, "Star").GetComponent<UITexture>();
        levelIndexLabel = Master.GetChildByName(gameObject, "LevelIndex").GetComponent<UILabel>();

        levelIndexLabel.text = levelIndex.ToString();

        if (levelIndex > (Master.LevelData.lastLevel + 1))
        {
            isActive = false;
        }
        else
        {
            isActive = true;
        }

        SetCollider();
        SetBackground();
        SetStar();

        if (!isActive)
        {
            gameObject.SetActive(false);
        }

    }

    void SetBackground()
    {
        if (isActive)
        {
            bgTexture.mainTexture = Resources.Load<Texture2D>("Textures/UI/Menu/Buttons/BG_LevelButton");
        }
        else
        {
            bgTexture.mainTexture = Resources.Load<Texture2D>("Textures/UI/Menu/Buttons/BG_LevelButton_Inactive");
        }
    }

    void SetStar()
    {
        if (!isActive)
        {
            starTexture.mainTexture = Resources.Load<Texture2D>("Textures/UI/Menu/Stars/0");
        }
        else
        {
            int star = Master.LevelData.GetStarAtLevel(levelIndex);
            if (levelIndex <= Master.LevelData.lastLevel)
            {
                if (star == 0)
                {
                    star = 1;
                }
                starTexture.mainTexture = Resources.Load<Texture2D>("Textures/UI/Menu/Stars/" + star);
            }
            else if (levelIndex > Master.LevelData.lastLevel)
            {
                starTexture.mainTexture = Resources.Load<Texture2D>("Textures/UI/Menu/Stars/0");
            }
        }
    }

    void SetCollider()
    {
        if (!isActive)
        {
            foreach (Collider2D item in GetComponentsInChildren<Collider2D>())
            {
                Destroy(item);
            }

            foreach (UIButton item in GetComponentsInChildren<UIButton>())
            {
                Destroy(item);
            }
        }
    }

    public void onClick()
    {
        Debug.Log(1);
        Master.PlaySoundButtonClick();
        Debug.Log(2);

        if (Master.Stats.Energy > 0)
        {
            Debug.Log(3);
            Master.Tutorial.CheckAndContinueNextStepTutorial(TutorialController.TutorialsIndex.BuildUnitInGameplay, 1);
            Debug.Log(4);

            // Master.Ad.CheckAndShowAd(() =>
            // {
                Debug.Log(5);
                Master.UI.Transition(() =>
                {
                    Debug.Log(6);
                    Master.LevelData.currentLevel = levelIndex;
                    Application.LoadLevel("Play");
                });
            // });
        }
        else
        {
            Debug.Log(7);
            Master.UIMenu.ShowDialog(UIController.Dialog.ListDialogs.FillEnergyDialog, 0.3f, new string[] { "GoToLevel", levelIndex.ToString() });
        }

    }

}
