using UnityEngine;
using System.Collections;

public class QuestItemController : MonoBehaviour
{

    // Use this for initialization
    QuestDataController.QuestData questData;
    UITexture bgTexture;
    UITexture iconTexture;
    UILabel questNameLabel;
    UISlider progressSlider;
    UILabel progressValueLabel;
    UILabel starValueLabel;
    UILabel gemValueLabel;
    public bool isComplete;

    void Awake()
    {
        AssignObject();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void AssignObject()
    {
        bgTexture = Master.GetChildByName(gameObject, "BG").GetComponent<UITexture>();
        iconTexture = Master.GetChildByName(gameObject, "Icon").GetComponent<UITexture>();
        questNameLabel = Master.GetChildByName(gameObject, "QuestName").GetComponent<UILabel>();
        starValueLabel = Master.GetChildByName(gameObject, "StarValueLabel").GetComponent<UILabel>();
        gemValueLabel = Master.GetChildByName(gameObject, "GemValueLabel").GetComponent<UILabel>();
        progressSlider = Master.GetChildByName(gameObject, "ProgressBar").GetComponent<UISlider>();
        progressValueLabel = Master.GetChildByName(gameObject, "ProgressValue").GetComponent<UILabel>();
    }

    public void SetAttribute(QuestDataController.QuestData questDataGet)
    {

        this.questData = questDataGet;

        if (questData.CurrentProgressValue >= questData.RequireValue.Value)
        {
            isComplete = true;
        }
        else
        {
            isComplete = false;
        }

        iconTexture.mainTexture = Resources.Load<Texture2D>("Textures/Quests/quest_icon_" + questData.QuestID);
        questNameLabel.text = questData.QuestName;
        progressSlider.value = (float)questData.CurrentProgressValue / questData.RequireValue.Value;
        progressValueLabel.text = questData.CurrentProgressValue + "/" + questData.RequireValue.Value;
        starValueLabel.text = questData.Reward.Star.ToString();
        gemValueLabel.text = questData.Reward.Gem.ToString();

        SetButton();
    }

    void SetButton()
    {
        if (isComplete)
        {
            bgTexture.color = new Color((float)60 / 255, (float)115 / 255, (float)158 / 255, 1);
            bgTexture.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            bgTexture.color = new Color((float)60 / 255, (float)60 / 255, (float)60 / 255, 1);
            bgTexture.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void OnTouchIn()
    {
        if (!isComplete) return;

        Master.Stats.Gem += questData.Reward.Gem;
        Master.Stats.Star += questData.Reward.Star;
        Master.QuestData.IncreaseStep(questData.QuestID);
        Master.QuestData.LoadQuestData();
        //SetAttribute(Master.QuestData.GetQuestDataByID(questData.QuestID));
        Master.UI.ShowDialog("GotRewardDialog", 0.5f, new string[] { questData.Reward.Gem.ToString(), questData.Reward.Star.ToString() });
        Master.UIMenu.panels[3].GetComponent<QuestPanelController>().SetListQuest();

    }

}
