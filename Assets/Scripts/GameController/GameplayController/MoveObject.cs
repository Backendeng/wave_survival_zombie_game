using UnityEngine;
using System.Collections;
using DG.Tweening;
public class MoveObject : MonoBehaviour
{

    // Use this for initialization
    [Header("Move Controller")]
    public bool isMove = true;
    public Move move = Move.Horizontal;

    public enum Move
    {
        Horizontal,
        LoopHorizontal,
        Vertical,
        LoopVertical,
        ToPosition,
        ToPositionLoop,
        ToPositionLoopFromStart,
    }

    public float moveSpeed;
    public float timeToMoveReverse;
    public float timeToMoveToPosition;
    public Transform toPosition;

    private bool isMovingToPosition;
    private bool isMoveRight;
    private bool isMoveUp;

    [Header("Scale Controller")]

    public bool isScale;

    public float maxSize;
    public float minSize;
    public float time;

    [Header("Rotation Controller")]
    public bool isRotation;
    public enum RotateDirection
    {
        X, Y, Z
    }
    public RotateDirection rotateDirection;
    public bool isRightRotate;
    public float rotateSpeed;

    [Header("Auto Destroy")]
    public bool isAutoDestroy = false;
    public float timeToDestroy;
    private Transform startPos;
    void Awake()
    {

    }

    void Start()
    {
        startPos = gameObject.transform;
        SetTimeToReverseMove();
        ScaleController();
        DestroyController();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMove)
        {
            MoveController();
        }
        RotationController();
    }

    void MoveController()
    {
        if (isMovingToPosition) return;

        float speed = (moveSpeed * Time.deltaTime) / 100;
        switch (move)
        {
            case Move.Horizontal:
                gameObject.transform.Translate(new Vector3(speed, 0, 0));
                break;
            case Move.LoopHorizontal:
                if (isMoveRight)
                {
                    gameObject.transform.Translate(new Vector3(speed, 0, 0));
                }
                else
                {
                    gameObject.transform.Translate(new Vector3(-speed, 0, 0));
                }
                break;
            case Move.Vertical:
                gameObject.transform.Translate(new Vector3(0, speed, 0));
                break;
            case Move.LoopVertical:
                if (isMoveUp)
                {
                    gameObject.transform.Translate(new Vector3(0, speed, 0));
                }
                else
                {
                    gameObject.transform.Translate(new Vector3(0, -speed, 0));
                }
                break;
            case Move.ToPosition:
                isMovingToPosition = true;
                gameObject.transform.DOMove(toPosition.position, timeToMoveToPosition);
                break;
            case Move.ToPositionLoop:
                isMovingToPosition = true;
                gameObject.transform.DOMove(toPosition.position, timeToMoveToPosition).SetEase(Ease.OutQuint).SetLoops(9999);
                break;
            case Move.ToPositionLoopFromStart:
                isMovingToPosition = true;
                gameObject.transform.Translate(new Vector3(speed, 0, 0));
                break;
        }
    }

    void SetTimeToReverseMove()
    {
        Master.WaitAndDo(timeToMoveReverse, () =>
        {
            isMoveRight = !isMoveRight;
            isMoveUp = !isMoveUp;
            SetTimeToReverseMove();
        });
    }

    void ScaleController()
    {
        if (isScale)
        {
            transform.DOScale(new Vector3(maxSize, maxSize, maxSize), time).OnComplete(() =>
            {
                transform.DOScale(new Vector3(minSize, minSize, minSize), time).OnComplete(() =>
                {
                    ScaleController();
                });
            });
        }
    }

    void RotationController()
    {
        if (isRotation)
        {
            Vector3 v = new Vector3();
            float speed = rotateSpeed * Time.deltaTime*20;
            if (rotateDirection == RotateDirection.X)
            {
                v = new Vector3(speed, 0, 0);
            }
            if (rotateDirection == RotateDirection.Y)
            {
                v = new Vector3(0, speed, 0);
            }
            if (rotateDirection == RotateDirection.Z)
            {
                v = new Vector3(0, 0, speed);
            }
            transform.Rotate(v);
        }
    }


    void DestroyController()
    {
        if (isAutoDestroy)
        {
            Destroy(gameObject, timeToDestroy);
        }
    }

}
