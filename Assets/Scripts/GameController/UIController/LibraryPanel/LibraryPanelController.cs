using UnityEngine;
using System.Collections;

public class LibraryPanelController : MonoBehaviour
{

    // Use this for initialization
    UIGrid enemyCardGrid;
    GameObject pf_enemyCard;

    void Awake()
    {
        AssignObjects();
    }


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void AssignObjects()
    {
        enemyCardGrid = Master.GetChildByName(gameObject, "ListZombies").GetComponent<UIGrid>();
        pf_enemyCard = Master.GetGameObjectInPrefabs("UI/ZombieCard");
    }

    public void OnOpen()
    {
        //clear current
        NGUITools.DestroyChildren(enemyCardGrid.gameObject.transform);

        SetListEnemyCard();

    }

    public void SetListEnemyCard()
    {
        foreach (EnemyDataController.EnemyData enemyData in Master.EnemyData.listEnemyData)
        {
            GameObject enemyCard = NGUITools.AddChild(enemyCardGrid.gameObject, pf_enemyCard);
            enemyCard.name = "EnemyCard_" + enemyData.EnemyID;
            bool isUnlock = Master.EnemyData.IsEnemyUnlock(enemyData.EnemyID);
            if (isUnlock)
            {
                enemyCard.GetComponentInChildren<UITexture>().mainTexture = Resources.Load<Texture2D>("Textures/Characters/Enemies/Enemy_" + enemyData.EnemyID + "/Card");
                enemyCard.GetComponentInChildren<BoxCollider2D>().enabled = true;
            }
            else
            {
                enemyCard.GetComponentInChildren<UITexture>().mainTexture = Resources.Load<Texture2D>("Textures/Characters/Enemies/Enemy_" + enemyData.EnemyID + "/Card_Lock");
                enemyCard.GetComponentInChildren<BoxCollider2D>().enabled = false;
            }
        }

        //add enemy 10 (unknow)
        GameObject enemyCard_10 = NGUITools.AddChild(enemyCardGrid.gameObject, pf_enemyCard);
        enemyCard_10.name = "EnemyCard_10";
        enemyCard_10.GetComponentInChildren<UITexture>().mainTexture = Resources.Load<Texture2D>("Textures/Characters/Enemies/Card_Lock_10");
        enemyCard_10.GetComponentInChildren<BoxCollider2D>().enabled = false;

        enemyCardGrid.Reposition();
    }

    public void Card_OnClick(GameObject go)
    {
        string enemyID = go.transform.parent.name.Split('_')[1];
        Master.UI.ShowDialog(UIController.Dialog.ListDialogs.EnemyInfoDialog, 0.4f, new string[] { enemyID });
    }
}
