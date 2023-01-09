using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
    // Use this for initialization
    [HideInInspector]
    public GameObject uiRoot;
    [HideInInspector]
    public Camera camera;

    private GameObject pf_transition;
    private GameObject dialogPanel;

    public virtual void OnAwake() { }
    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void AssignObject() { }

    [HideInInspector]
    public Transform[] pos = new Transform[5];


    public class Dialog
    {
        public static Camera camera;
        public static GameObject root;
        public static GameObject blackPanel;
        public static GameObject blockTouch;
        public static List<GameObject> listDialogShowing = new List<GameObject>();

        public class ListDialogs
        {
            public static string FillEnergyDialog = "FillEnergyDialog";
            public static string FreeRewardDialog = "FreeRewardDialog";
            public static string GotRewardDialog = "GotRewardDialog";
            public static string LevelCompleteDialog = "LevelCompleteDialog";
            public static string NewUnitUnlockDialog = "NewUnitUnlockDialog";
            public static string NewSkillUnlockDialog = "NewSkillUnlockDialog";
            public static string PauseGame = "PauseGame";
            public static string RatingDialog = "RatingDialog";
            public static string EnemyInfoDialog = "EnemyInfoDialog";
            public static string ExitGameDialog = "ExitGameDialog";
        }
    }



    void Awake()
    {
        uiRoot = GameObject.Find("UI Root");
        camera = Master.GetChildByName(uiRoot, "Camera").GetComponent<Camera>();
        pf_transition = Master.GetGameObjectInPrefabs("UI/Transition");
        dialogPanel = Master.GetChildByName(uiRoot, "Dialogs");

        if (Master.GetChildByName(uiRoot, "PosUI") == null)
            NGUITools.AddChild(uiRoot, Master.GetGameObjectInPrefabs("PosUI")).name = "PosUI";

        pos[0] = Master.GetChildByName(uiRoot, "Pos_Left").transform;
        pos[1] = Master.GetChildByName(uiRoot, "Pos_Top").transform;
        pos[2] = Master.GetChildByName(uiRoot, "Pos_Right").transform;
        pos[3] = Master.GetChildByName(uiRoot, "Pos_Bottom").transform;
        pos[4] = Master.GetChildByName(uiRoot, "Pos_Center").transform;

        //check dialog root
        if (GameObject.Find("Dialog Root") == null)
        {
            Dialog.root = (GameObject)Instantiate(Master.GetGameObjectInPrefabs("UI/Dialogs/Dialog Root"), Vector3.zero, Quaternion.identity);
        }
        Dialog.blackPanel = Master.GetChildByName(Dialog.root, "Blackpanel");
        Dialog.blockTouch = Master.GetChildByName(Dialog.root, "BlockTouch");
        Dialog.camera = Master.GetChildByName(Dialog.root, "Camera").GetComponent<Camera>();
        Dialog.root.SetActive(false);

        AssignObject();
        OnAwake();
    }

    void Start()
    {
        OnStart();
    }

    void Update()
    {
        OnUpdate();
    }

    void LateUpdate()
    {
        OnLateUpdate();
    }

    void OnDestroy()
    {
        CancelInvoke();
        StopAllCoroutines();
    }

    public virtual void Transition(System.Action onHalfComplete = null, System.Action onComplete = null, float time = 0.6f, float delay = 0.2f)
    {
        GameObject transition = (GameObject)Instantiate(pf_transition, Vector3.zero, Quaternion.identity);
        transition.GetComponent<Transition>().doTransition(onHalfComplete, onComplete, time, delay);
        //GameObject leftDoor = Master.GetChildByName(transition, "LeftDoor");
        //GameObject rightDoor = Master.GetChildByName(transition, "RightDoor");
        //float firstPos = leftDoor.transform.position.x;

        //leftDoor.transform.DOMoveX(0, time);
        //rightDoor.transform.DOMoveX(0, time).OnComplete(() =>
        //{
        //    if (onHalfComplete != null)
        //    {
        //        onHalfComplete();
        //    }

        //    Master.WaitAndDo(delay, () =>
        //    {
        //        leftDoor.transform.DOMoveX(firstPos, time);
        //        rightDoor.transform.DOMoveX(-firstPos, time).OnComplete(() =>
        //        {
        //            if (onComplete != null)
        //            {
        //                onComplete();
        //            }
        //            Destroy(transition.gameObject);
        //        });
        //    }, this);

        //});

    }

    public void ShowDialog(string dialogName, float duration = 0.4f, string[] agruments = null, System.Action onComplete = null, System.Action onCloseComplete = null, DialogController.ShowDialogType showDialogType = DialogController.ShowDialogType.FromTop, bool isBlockTouch = true, bool isTouchAnywhereToCloseDialog = false, bool isHightlight = true)
    {
        // Master.Ad.Admob.ShowTopBanner();
        Dialog.root.SetActive(true);

        Dialog.blockTouch.SetActive(isBlockTouch);
        Dialog.blackPanel.SetActive(isHightlight);

        if (Dialog.listDialogShowing.Count > 0)
        {
            for (int i = 0; i < Dialog.listDialogShowing.Count; i++)
            {
                Dialog.listDialogShowing[i].GetComponent<UIPanel>().depth = 48 - i;
            }
        }

        GameObject pf_dialog = Master.GetGameObjectInPrefabs("UI/Dialogs/" + dialogName);
        GameObject dialog = NGUITools.AddChild(Dialog.root, pf_dialog);
        dialog.GetComponent<DialogController>().OnOpen(agruments, onCloseComplete);
        //dialog.SendMessage("OnOpen", agruments, SendMessageOptions.DontRequireReceiver);

        dialog.GetComponent<UIPanel>().alpha = 0;
        TweenAlpha.Begin(dialog, duration, 1);
        Dialog.listDialogShowing.Add(dialog);

        switch (showDialogType)
        {
            case DialogController.ShowDialogType.FromLeft:
                dialog.transform.position = pos[0].position;
                break;
            case DialogController.ShowDialogType.FromTop:
                dialog.transform.position = pos[1].position;
                break;
            case DialogController.ShowDialogType.FromRight:
                dialog.transform.position = pos[2].position;
                break;
            case DialogController.ShowDialogType.FromBottom:
                dialog.transform.position = pos[3].position;
                break;
            case DialogController.ShowDialogType.Center:
                dialog.transform.position = pos[4].position;
                break;
        }
        // dialog.transform.setUp
        dialog.transform.DOMove(pos[4].position, duration).SetUpdate(true).OnComplete(() =>
        {
            //dialog.SendMessage("OnShowComplete", SendMessageOptions.DontRequireReceiver);
            dialog.GetComponent<DialogController>().OnShowComplete();
            if (onComplete != null)
            {
                onComplete();
            }
        });
    }

    public void CloseDialog(GameObject dialog, float speed = 0.4f, System.Action onComplete = null, DialogController.CloseDialogType closeDialogType = DialogController.CloseDialogType.ToTop, bool isBlockTouch = true, bool isTouchAnywhereToCloseDialog = false, bool isHightlight = true)
    {
        Vector3 position = pos[1].position;

        switch (closeDialogType)
        {
            case DialogController.CloseDialogType.ToLeft:
                position = pos[0].position;
                break;
            case DialogController.CloseDialogType.ToTop:
                position = pos[1].position;
                break;
            case DialogController.CloseDialogType.ToRight:
                position = pos[2].position;
                break;
            case DialogController.CloseDialogType.ToBottom:
                position = pos[3].position;
                break;
        }

        dialog.GetComponent<UIPanel>().alpha = 1;
        TweenAlpha.Begin(dialog, speed, 0);

        dialog.transform.DOMove(position, speed).SetUpdate(true).OnComplete(() =>
        {
            if (Dialog.listDialogShowing.Count <= 1)
            {
                Dialog.root.SetActive(false);
                Dialog.blockTouch.SetActive(false);
                Dialog.blackPanel.SetActive(false);
            }
            else
            {
                Dialog.listDialogShowing[Dialog.listDialogShowing.Count - 2].GetComponent<UIPanel>().depth = 100;
            }


            if (onComplete != null)
            {
                onComplete();
            }
            Dialog.listDialogShowing.Remove(dialog);
            Destroy(dialog);

            Master.Ad.Admob.HideBanner();

        });
    }



}
