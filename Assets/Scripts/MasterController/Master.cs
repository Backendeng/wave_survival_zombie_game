using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Master : MonoBehaviour
{

    // Use this for initialization
    //for assign
    //for global controller
    public static Master instance;
    public static AudioController Audio;
    public static AdController Ad;
    public static IAPController IAP;
    public static SocialController Social;
    public static TouchController Touch;
    public static EffectController Effect;

    public static UnitDataController UnitData;
    public static EnemyDataController EnemyData;
    public static SkillDataController SkillData;
    public static ProductDataController ProductData;
    public static QuestDataController QuestData;
    public static StatsController Stats;

    public static GameplayController Gameplay;
    public static UIController UI;
    public static UIControllerGameplay UIGameplay;
    public static UIControllerMenu UIMenu;
    public static BackgroundController Background;
    public static LaneController Lane;
    public static LevelDataController LevelData;
    public static LevelController Level;
    public static TutorialController Tutorial;
    public static PushNotificationController PushNotification;

    public static bool isGameStart;
    public static bool isLevelComplete;
    public static bool isGameOver;
    public static bool isMenu;

    public int timesLevelCompleteToShowRatingDialog = 5;

    public static int defaultPanelMenu = 0;

    public enum Platform
    {
        Android, iOS
    }
    [HideInInspector]
    public Platform platform;
    public string linkGooglePlay;
    public string linkAppStore;
    public string linkGooglePlayDeveloperStore;
    public string linkAppsStoreDeveloperStore;
    public string linkInMarket;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

#if UNITY_ANDROID
        platform = Platform.Android;
        linkInMarket = linkGooglePlay;
#endif

#if UNITY_IOS
       platform = Platform.iOS;
       linkInMarket = linkAppStore;

#endif

    }


    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        CancelInvoke();
        StopAllCoroutines();
    }


    //for get gameobject in prefabs
    public static GameObject GetGameObjectInPrefabs(string path)
    {
        return Resources.Load<GameObject>("Prefabs/" + path);
    }

    public static GameObject GetUnitPrefabByID(string unitID)
    {
        return Resources.Load<GameObject>("Prefabs/Characters/Units/Unit_" + unitID);
    }

    public static GameObject GetEnemyPrefabByID(string enemyID)
    {
        return Resources.Load<GameObject>("Prefabs/Characters/Enemies/Enemy_" + enemyID);
    }

    public static GameObject GetSkillPrefabByID(string skillID)
    {
        return Resources.Load<GameObject>("Prefabs/Skills/Skill_" + skillID);
    }

    public static GameObject GetChildByName(GameObject parent, string childName)
    {
        foreach (Transform item in parent.GetComponentsInChildren<Transform>(true))
        {
            string childNameItem = item.gameObject.name;
            if (childNameItem == childName)
            {
                return item.gameObject;
            }
        }
        return null;
    }

    public void CheckShowRatingDialog(System.Action actionAfterClose = null)
    {
        if (Master.Stats.isClickedRating)
        {
            if (actionAfterClose != null)
            {
                actionAfterClose();
            }
        }
        else
        {
            if (Master.Stats.TimesLevelComplete % timesLevelCompleteToShowRatingDialog == 0)
            {
                Master.UI.ShowDialog(UIController.Dialog.ListDialogs.RatingDialog, 0.3f, null, null, actionAfterClose);
            }
            else
            {
                if (actionAfterClose != null)
                {
                    actionAfterClose();
                }
            }
        }
    }

    //for wait time

    public static void WaitAndDo(float time, System.Action onComplete, MonoBehaviour monoBehaviour = null, bool isRunWithoutTimeSale = false)
    {
        if (monoBehaviour != null)
        {
            monoBehaviour.StartCoroutine(instance.WaitAndDoIE(time, onComplete, monoBehaviour, isRunWithoutTimeSale));
        }
        else if (instance != null)
        {
            instance.StartCoroutine(instance.WaitAndDoIE(time, onComplete, instance, isRunWithoutTimeSale));
        }
    }

    IEnumerator WaitAndDoIE(float time, System.Action onComplete = null, MonoBehaviour monoBehaviour = null, bool isRunWithoutTimeScale = false)
    {
        //yield return new WaitForSeconds(time);
        //if (onComplete != null)
        //{
        //    onComplete();
        //}

        if (isRunWithoutTimeScale)
        {
            if (monoBehaviour != null)
            {
                yield return monoBehaviour.StartCoroutine(Master.WaitForRealSeconds(time));
            }
            else if (instance != null)
            {

                yield return instance.StartCoroutine(Master.WaitForRealSeconds(time));
            }

            if (onComplete != null)
            {
                onComplete();
            }
        }
        else
        {
            yield return new WaitForSeconds(time);
            if (onComplete != null)
            {
                onComplete();
            }
        }
    }
    public static IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }

    //for animation 

    public static void PlayAnimation(Object imageComponent, string pathToTuxturesAnimation, int startIndex, int endIndex, float delay, bool isLoop, System.Action onComplete = null, MonoBehaviour mono = null)
    {
        //check if is UITexture
        if (mono != null) mono.StopAllCoroutines();

        SpriteRenderer spriteRenderer = (SpriteRenderer)imageComponent;
        Object[] listTextures;
        bool isSprite;
        if (spriteRenderer)
        {
            listTextures = Resources.LoadAll(pathToTuxturesAnimation, typeof(Sprite)).Cast<Sprite>().OrderBy(x => int.Parse(x.name)).ToArray();
            isSprite = true;
        }
        else
        {
            listTextures = Resources.LoadAll(pathToTuxturesAnimation, typeof(Texture2D)).Cast<Texture2D>().OrderBy(x => int.Parse(x.name)).ToArray();
            isSprite = false;
        }
        //Debug.Log(listTextures.Length);
        doPlayAnimation(startIndex, imageComponent, isSprite, listTextures, startIndex, endIndex, delay, isLoop, onComplete, mono);
    }

    private static void doPlayAnimation(int index, Object imageComponent, bool isSprite, Object[] listTextures, int startIndex, int endIndex, float delay, bool isLoop, System.Action onComplete, MonoBehaviour mono)
    {
        WaitAndDo(delay, () =>
        {

            if (imageComponent == null)
            {
                return;
            }

            SpriteRenderer spriteRenderer = null;
            UITexture uiTexture = null;
            Sprite[] listSprites = null;
            Texture2D[] listTextures2D = null;
            if (isSprite)
            {
                spriteRenderer = (SpriteRenderer)imageComponent;
                listSprites = (Sprite[])listTextures;
            }
            else
            {
                uiTexture = (UITexture)imageComponent;
                listTextures2D = (Texture2D[])listTextures;
            }

            if (startIndex != -1)
            {
                if (index > endIndex || index >= listTextures.Length)
                {
                    if (isLoop)
                    {
                        index = startIndex;
                    }
                    else
                    {
                        if (onComplete != null)
                        {
                            onComplete();
                        }
                        return;
                    }
                }

                if (index >= startIndex && index <= endIndex)
                {
                    if (isSprite)
                    {
                        spriteRenderer.sprite = listSprites[index];
                    }
                    else
                    {
                        uiTexture.mainTexture = listTextures2D[index];
                    }
                    index++;
                }
            }
            else
            {
                if (index >= listTextures.Length)
                {
                    if (isLoop)
                    {
                        index = 0;
                    }
                    else
                    {
                        onComplete();
                        return;
                    }
                }

                if (isSprite)
                {
                    spriteRenderer.sprite = listSprites[index];

                }
                else
                {
                    uiTexture.mainTexture = listTextures2D[index];
                }
                index++;
            }
            doPlayAnimation(index, imageComponent, isSprite, listTextures, startIndex, endIndex, delay, isLoop, onComplete, mono);
        }, mono);
    }

    public static void PlaySoundButtonClick()
    {
        Audio.PlaySound("snd_click", 0.3f);
    }

    public static float IncreaseValues(float value, int times, float increasePercent)
    {
        for (int i = 0; i < times; i++)
        {
            value += (float)(value * increasePercent) / 100;
        }
        value = Mathf.Round(value * 10f) / 10f;
        return value;
    }

    public bool isInternetConnected()
    {
        WWW www = new WWW("http://google.com");
        if (www.error != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
