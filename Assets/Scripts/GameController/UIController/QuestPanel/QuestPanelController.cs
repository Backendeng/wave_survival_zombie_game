using UnityEngine;
using System.Collections;

public class QuestPanelController : MonoBehaviour {

    // Use this for initialization
    private UIGrid questGrid;
    private GameObject pf_questItem;
    void Awake()
    {
        AssignObject();
    }

    void AssignObject()
    {
        questGrid = Master.GetChildByName(gameObject, "ListQuests").GetComponent<UIGrid>();
        pf_questItem = Master.GetGameObjectInPrefabs("UI/QuestItem");

    }

	void Start () {
	
	}
	
    public void OnOpen()
    {
        SetListQuest();
    }

	// Update is called once per frame
	void Update () {
	
	}

    public void SetListQuest()
    {
        Master.QuestData.LoadQuestData();
        ClearQuestObject();
        foreach (QuestDataController.QuestData questData in Master.QuestData.questDataCollection.ListQuestData)
        {
            GameObject questItem = NGUITools.AddChild(questGrid.gameObject, pf_questItem);
            questItem.GetComponent<QuestItemController>().SetAttribute(questData);
        }
        questGrid.Reposition();

    }

    void ClearQuestObject()
    {
        while (questGrid.gameObject.transform.childCount > 0)
            NGUITools.Destroy(questGrid.gameObject.transform.GetChild(0).gameObject);
    }
}
