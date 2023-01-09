using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndingController : MonoBehaviour
{

    // Use this for initialization
    UIPanel blackPanel;
    GameObject blockTouch;
    GameObject nextButton;

    public List<UIPanel> listSequences = new List<UIPanel>();
    int currentSequenceIndex = -1;
    bool isCanClickNext = false;

    void Awake()
    {
        AssignObjects();
    }

    void Start()
    {
        StartEndingFirst();
        Master.Audio.PlayBackgroundMusic("bg_ending", 0.7f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void StartEnding()
    {
        Instantiate(Master.GetGameObjectInPrefabs("UI/Ending"));
    }

    void AssignObjects()
    {
        blackPanel = Master.GetChildByName(gameObject, "BlackPanel").GetComponent<UIPanel>();
        blockTouch = Master.GetChildByName(gameObject, "BlockTouch");
        nextButton = Master.GetChildByName(gameObject, "NextSequenceButton");
        Transform sequences = Master.GetChildByName(gameObject, "Sequences").transform;
        listSequences.Clear();
        foreach (Transform t in sequences)
        {
            listSequences.Add(t.gameObject.GetComponent<UIPanel>());
            t.gameObject.SetActive(false);
        }
        blackPanel.alpha = 0;
        blockTouch.SetActive(false);
        nextButton.SetActive(true);
    }

    public void StartEndingFirst()
    {
        Time.timeScale = 1;
        isCanClickNext = false;
        currentSequenceIndex = 0;
        blackPanel.gameObject.SetActive(true);
        blackPanel.alpha = 0;
        TweenAlpha.Begin(blackPanel.gameObject, 5, 1).SetOnFinished(() =>
        {
            ShowSequence();
        });
    }

    void ShowSequence()
    {
        isCanClickNext = false;

        foreach (UIPanel sequence in listSequences)
        {
            TweenAlpha.Begin(sequence.gameObject, 0.5f, 0).SetOnFinished(() =>
            {
                sequence.gameObject.SetActive(false);
            });
        }

        if (currentSequenceIndex >= 0)
        {
            Master.WaitAndDo(0.6f, () =>
            {
                if (currentSequenceIndex >= listSequences.Count)
                {
                    Application.LoadLevel("FirstMenu");
                    return;
                }

                Debug.Log(listSequences[currentSequenceIndex].gameObject.name);
                listSequences[currentSequenceIndex].gameObject.SetActive(true);
                listSequences[currentSequenceIndex].alpha = 0;

                TweenAlpha.Begin(listSequences[currentSequenceIndex].gameObject, 2, 1).SetOnFinished(() =>
                {
                    isCanClickNext = true;
                });
            }, this);
        }
        else
        {
            listSequences[currentSequenceIndex].gameObject.SetActive(true);
            listSequences[currentSequenceIndex].alpha = 0;

            TweenAlpha.Begin(listSequences[currentSequenceIndex].gameObject, 2, 1).SetOnFinished(() =>
            {
                isCanClickNext = true;
            });
        }

    }

    public void NextButton_OnClick()
    {
        if (isCanClickNext)
        {
            currentSequenceIndex++;
            ShowSequence();
        }
    }
}
