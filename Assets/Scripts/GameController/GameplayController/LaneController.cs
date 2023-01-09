using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaneController : MonoBehaviour
{

    // Use this for initialization
    public GameObject unitsPositionRoot;
    public int maxLane = 3;
    public int maxPositionInLane = 7;
    public Dictionary<int, Transform> positionOfLane = new Dictionary<int, Transform>();

    [HideInInspector]
    public Dictionary<int, List<GameObject>> listLanes = new Dictionary<int, List<GameObject>>();

    [HideInInspector]
    public Dictionary<GameObject, GameObject> listUnitAtPosition = new Dictionary<GameObject, GameObject>();

    void Awake()
    {
        if (Master.Lane == null)
        {
            Master.Lane = this;
        }
       // SetLane(maxLane);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetLane(int maxLane, int maxPositionInLane)
    {
        for (int i = 1; i <= maxLane; i++)
        {
            List<GameObject> lane = new List<GameObject>();
            listLanes.Add(i, lane);

            //add lane
            GameObject pf_lane = Master.GetGameObjectInPrefabs("UnitPosition/Lane");
            GameObject obj_lane = NGUITools.AddChild(unitsPositionRoot, pf_lane);
            obj_lane.name = "Lane_" + i;
            unitsPositionRoot.gameObject.GetComponent<UIGrid>().Reposition();
            positionOfLane.Add(i, obj_lane.transform);
            //add position in lane
            for (int y = 1; y <= maxPositionInLane; y++)
            {
                GameObject pf_position = Master.GetGameObjectInPrefabs("UnitPosition/Position");
                GameObject obj_position = NGUITools.AddChild(obj_lane, pf_position);
                listUnitAtPosition.Add(obj_position, null);
                obj_position.SetActive(false);
            }
        }
    }

    public void SetUnitAtPosition(GameObject position, GameObject unit)
    {
        listUnitAtPosition[position] = unit;
    }

    public void RemoveUnitAtPosition(GameObject position)
    {
        listUnitAtPosition[position] = null;
    }

    public void ShowUnitPositionsAvailable()
    {
        foreach (GameObject position in listUnitAtPosition.Keys)
        {
            position.SetActive(true);
            if (listUnitAtPosition[position] != null)
            {
                position.SetActive(false);
            }
        }
    }

    public void ChangeColorPosition()
    {
        foreach (GameObject gameObjectAtMouse in Master.Touch.listGameObjectsAtMousePosition)
        {
            foreach (GameObject position in listUnitAtPosition.Keys)
            {
                GameObject icon = position.transform.Find("Icon").gameObject;

                if (gameObjectAtMouse == position)
                {
                    icon.GetComponent<SpriteRenderer>().color = new Color(0, 1, 1);
                }
                else
                {
                    icon.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                }
            }
        }
    }


    public void HideUnitPosition()
    {
        foreach (GameObject position in listUnitAtPosition.Keys)
        {
            position.SetActive(false);
        }
    }

    public void SetCharacterAtLane(GameObject character, int lane)
    {
        listLanes[lane].Add(character);
    }

    public List<GameObject> GetCharactersInLane(int lane)
    {
        return listLanes[lane];
    }

    public List<GameObject> GetCharactersInLaneByTag(int lane, string tag)
    {
        List<GameObject> characters = new List<GameObject>();

        foreach (GameObject character in listLanes[lane])
        {
            if (character != null)
            {
                if (character.tag == tag)
                {
                    characters.Add(character);
                }
            }
        }
        return characters;
    }

    public void RemoveCharacterAtLane(int lane, GameObject obj)
    {
        listLanes[lane].Remove(obj);
    }

    public bool isExistCharacterByTagInLane(int lane, string tag)
    {
        foreach (GameObject character in listLanes[lane])
        {
            if (character != null)
            {
                if (character.tag == tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool isExistCharacterByTagInAllLane(string tag)
    {
        for (int i = 1; i <= Master.Level.currentLevelData.NumberOfLanes; i++)
        {
            foreach (GameObject character in listLanes[i])
            {
                if (character != null)
                {
                    if (character.tag == tag)
                    {
                        return true;
                    }
                }
            }
        }
        
        return false;
    }
}
