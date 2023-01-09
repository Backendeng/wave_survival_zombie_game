using UnityEngine;
using System.Collections;
using DG.Tweening;
public class GoldDrop : MonoBehaviour
{
    // Use this for initialization
    private Rigidbody2D rgBody2d;
    private float firstY;

    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("UI");
        gameObject.transform.parent = Master.UIGameplay.uiRoot.transform;
        rgBody2d = GetComponent<Rigidbody2D>();
        firstY = transform.position.y;
        SetMove();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < firstY)
        {
            Destroy(rgBody2d);
        }
    }

    void SetMove()
    {
        int[] randomX = new int[] { -20, -18, -16, 16, 18, 20 };
        rgBody2d.AddForce(new Vector3(randomX[Random.Range(0, randomX.Length)], Random.Range(60, 70)));
    }

    public void OnTouchIn()
    {
        Master.Audio.PlaySound("snd_getGold");
        transform.DOMove(Master.UIGameplay.totalGoldLabel.transform.position, 0.7f).OnComplete(() =>
        {
            Master.Gameplay.gold += StatsController.GoldPerCoin;
            Destroy(gameObject);
        });
    }


}
