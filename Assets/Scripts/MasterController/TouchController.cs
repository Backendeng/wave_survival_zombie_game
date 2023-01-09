using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchController : MonoBehaviour
{

    // Use this for initialization


    public Vector3 mousePositionGameplay;
    public Vector3 mousePositionUI;

    public List<GameObject> listGameObjectsAtMousePosition = new List<GameObject>();
    public enum TouchType
    {
        TouchIn, TouchUp, Touching
    }
    private List<System.Action> TouchInEvents = new List<System.Action>();
    private List<System.Action> TouchUpEvents = new List<System.Action>();
    private List<System.Action> TouchingEvents = new List<System.Action>();

    public bool isTouching;

    void Awake()
    {
        if (Master.Touch == null)
        {
            Master.Touch = this;
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Touch();
        GetMousePosition();
    }

    void Touch()
    {


        isTouching = Input.GetMouseButton(0);

        if (Input.GetMouseButton(0))
        {
            SendMessengerToGameObjectAtMousePosition("OnTouching");
            RunEvent(TouchType.Touching);
        }

        if (Input.GetMouseButtonDown(0))
        {
            SendMessengerToGameObjectAtMousePosition("OnTouchIn");
            RunEvent(TouchType.TouchIn);
        }

        if (Input.GetMouseButtonUp(0))
        {
            SendMessengerToGameObjectAtMousePosition("OnTouchUp");
            RunEvent(TouchType.TouchUp);
        }
    }

    private void SendMessengerToGameObjectAtMousePosition(string messenger)
    {
        GetGameObjectAtMousePosition();
        bool isBlockTouch = false;
        foreach (GameObject go in listGameObjectsAtMousePosition)
        {
            if (go.name.Contains("BlockTouch") && go.activeSelf)
            {
                isBlockTouch = true;
            }
        }

        if (isBlockTouch)
        {
            foreach (GameObject go in listGameObjectsAtMousePosition)
            {
                if (go.tag == "CanNotBeBlockTouch")
                {
                    go.SendMessage(messenger, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        else
        {
            foreach (GameObject go in listGameObjectsAtMousePosition)
            {
                go.SendMessage(messenger, SendMessageOptions.DontRequireReceiver);
            }
        }


    }

    public List<GameObject> GetGameObjectAtMousePosition()
    {
        listGameObjectsAtMousePosition.Clear();
        //gameplay root
        if (Master.Gameplay != null)
        {
            Ray ray = Master.Gameplay.camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit)
                {
                    if (!listGameObjectsAtMousePosition.Contains(hit.collider.gameObject))
                    {
                        listGameObjectsAtMousePosition.Add(hit.collider.gameObject);
                    }
                }
            }
        }

        //ui root
        if (Master.UIGameplay != null)
        {
            Ray ray = Master.UIGameplay.camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit)
                {
                    if (!listGameObjectsAtMousePosition.Contains(hit.collider.gameObject))
                    {
                        listGameObjectsAtMousePosition.Add(hit.collider.gameObject);
                    }
                }
            }
        }

        //ui root
        if (Master.UIMenu != null)
        {
            Ray ray = Master.UIMenu.camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit)
                {
                    if (!listGameObjectsAtMousePosition.Contains(hit.collider.gameObject))
                    {
                        listGameObjectsAtMousePosition.Add(hit.collider.gameObject);
                    }
                }
            }
        }

        return listGameObjectsAtMousePosition;
    }



    public void AddTouchEvent(TouchType touchType, System.Action ev)
    {
        switch (touchType)
        {
            case TouchType.TouchIn:
                if (!TouchInEvents.Contains(ev))
                {
                    TouchInEvents.Add(ev);
                }
                break;
            case TouchType.TouchUp:
                if (!TouchUpEvents.Contains(ev))
                {
                    TouchUpEvents.Add(ev);
                }
                break;
            case TouchType.Touching:
                if (!TouchingEvents.Contains(ev))
                {
                    TouchingEvents.Add(ev);
                }
                break;
        }
    }

    public void RemoveTouchEvent(TouchType touchType, System.Action ev)
    {
        switch (touchType)
        {
            case TouchType.TouchIn:
                if (TouchInEvents.Contains(ev))
                {
                    TouchInEvents.Remove(ev);
                }
                break;
            case TouchType.TouchUp:
                if (TouchUpEvents.Contains(ev))
                {
                    TouchUpEvents.Remove(ev);
                }
                break;
            case TouchType.Touching:
                if (TouchingEvents.Contains(ev))
                {
                    TouchingEvents.Remove(ev);
                }
                break;
        }
    }

    void RunEvent(TouchType touchType)
    {
        switch (touchType)
        {
            case TouchType.TouchIn:
                if (TouchInEvents.Count > 0)
                {
                    foreach (System.Action ev in TouchInEvents)
                    {
                        if (ev != null)
                        {
                            ev();
                        }
                    }
                }
                break;

            case TouchType.TouchUp:
                if (TouchUpEvents.Count > 0)
                {
                    foreach (System.Action ev in TouchUpEvents)
                    {
                        if (ev != null)
                        {
                            ev();
                        }
                    }
                }
                break;

            case TouchType.Touching:
                if (TouchingEvents.Count > 0)
                {
                    foreach (System.Action ev in TouchingEvents)
                    {
                        if (ev != null)
                        {
                            ev();
                        }
                    }
                }
                break;
        }
    }


    public void GetMousePosition()
    {
        if (Input.mousePosition != null)
        {
            if (Master.Gameplay != null)
            {
                mousePositionGameplay = Master.Gameplay.camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
            }

            if (Master.UIGameplay != null)
            {
                mousePositionUI = Master.UIGameplay.camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
            }

            if (Master.UIMenu != null)
            {
                mousePositionUI = Master.UIMenu.camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
            }
        }
    }

}
