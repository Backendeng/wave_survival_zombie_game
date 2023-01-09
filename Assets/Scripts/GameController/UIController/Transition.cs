using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Transition : MonoBehaviour
{

    // Use this for initialization
    public System.Action tempOnHalfComplete;
    public System.Action tempOnComplete;

    GameObject leftDoor;
    GameObject rightDoor;
    GameObject loadingLabel;
    GameObject logo;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void doTransition(System.Action onHalfComplete = null, System.Action onComplete = null, float time = 0.6f, float delay = 0.2f)
    {
        leftDoor = Master.GetChildByName(gameObject, "LeftDoor");
        rightDoor = Master.GetChildByName(gameObject, "RightDoor");
        loadingLabel = Master.GetChildByName(gameObject, "LoadingLabel");
        logo = Master.GetChildByName(gameObject, "Logo");

        logo.GetComponent<UITexture>().color = new Color(1, 1, 1, 0);
        TweenAlpha.Begin(logo, time, 1).ignoreTimeScale = true;
        loadingLabel.SetActive(false);
        float firstPos = leftDoor.transform.position.x;

        Master.Audio.PlaySound("snd_transition", 1);

        leftDoor.transform.DOMoveX(0, time).SetUpdate(true);
        rightDoor.transform.DOMoveX(0, time).SetUpdate(true).OnComplete(() =>
        {
            loadingLabel.SetActive(true);
            Master.Audio.StopSound();
            if (tempOnHalfComplete != null)
            {
                tempOnHalfComplete();
                tempOnHalfComplete = null;
            }
            else if (onHalfComplete != null)
            {
                onHalfComplete();
            }

            Master.WaitAndDo(delay, () =>
            {
                loadingLabel.SetActive(false);
                Master.Audio.PlaySound("snd_transition", 1);
                TweenAlpha.Begin(logo, time * 0.5f, 0).ignoreTimeScale = true;
                leftDoor.transform.DOMoveX(firstPos, time).SetUpdate(true);
                rightDoor.transform.DOMoveX(-firstPos, time).SetUpdate(true).OnComplete(() =>
                {
                    Master.Audio.StopSound();

                    if (tempOnComplete != null)
                    {
                        tempOnComplete();
                        tempOnComplete = null;
                    }
                    else if (onComplete != null)
                    {
                        onComplete();
                    }
                    Destroy(gameObject);
                });
            }, this, true);

        });
    }
}
