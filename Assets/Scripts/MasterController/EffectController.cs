using UnityEngine;
using System.Collections;

public class EffectController : MonoBehaviour
{

    // Use this for initialization

    //for shake camera
    float shakeAmount = 3f;
    float shakeDecreaseFactor = 1.0f;
    float shakeTime;
    int cameraGetShake = 1;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Master.Effect != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Master.Effect = this;
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SetUnScaleTimeParticleSystem();
        doShakeCamera();
    }

    void SetUnScaleTimeParticleSystem()
    {
        if (Time.timeScale <= 0)
        {
            ParticleSystem[] particleSystems = FindObjectsOfType<ParticleSystem>();
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                particleSystem.Simulate(Time.unscaledDeltaTime, true, false);
            }
        }
    }

    public void Fill(Object textureComponent, float time, float startAmount, float endAmount, System.Action onComplete = null)
    {
        float valueMinus = 0.02f / (time / 2);

        try
        {
            UITexture uiTexture = (UITexture)textureComponent;
            uiTexture.fillAmount = startAmount;
        }
        catch
        {
            UI2DSprite uiSprite = (UI2DSprite)textureComponent;
            uiSprite.fillAmount = startAmount;
        }

        doFill(textureComponent, valueMinus, startAmount, endAmount, onComplete);
    }

    private void doFill(Object textureComponent, float valueMinus, float startAmount, float endAmount, System.Action onComplete = null)
    {
        Master.WaitAndDo(0.02f, () =>
        {
            UITexture uiTexture = null;
            UI2DSprite uiSprite = null;
            bool isUITexture;
            try
            {
                uiTexture = (UITexture)textureComponent;
                isUITexture = true;
            }
            catch
            {
                uiSprite = (UI2DSprite)textureComponent;
                isUITexture = false;
            }


            if (textureComponent != null)
            {
                bool isComplete = false;
                if (startAmount > endAmount)
                {
                    if (isUITexture)
                    {

                        uiTexture.fillAmount -= valueMinus;

                        if (uiTexture.fillAmount <= 0)
                        {
                            isComplete = true;
                        }
                    }
                    else
                    {
                        uiSprite.fillAmount -= valueMinus;
                        if (uiSprite.fillAmount <= 0)
                        {
                            isComplete = true;
                        }
                    }
                }
                else
                {
                    if (isUITexture)
                    {

                        uiTexture.fillAmount += valueMinus;

                        if (uiTexture.fillAmount >= endAmount)
                        {
                            isComplete = true;
                        }
                    }
                    else
                    {
                        uiSprite.fillAmount += valueMinus;
                        if (uiSprite.fillAmount >= endAmount)
                        {
                            isComplete = true;
                        }
                    }
                }

                if (isComplete)
                {
                    if (onComplete != null)
                        onComplete();
                }
                else
                {
                    doFill(textureComponent, valueMinus, startAmount, endAmount, onComplete);
                }
            }
        });
    }

    ///<summary>
    ///Shake camera
    ///<para>Camera will be get Shake: 1: Gameplay, 2: UI, 3: Background, 4: Dialog, 5: All</para>
    ///</summary>
    public void ShakeCamera(int cameraGetShake = 1, float shakeTime = 0.3f, float shakeAmount = 3, float shakeDecreaseFactor = 1)//1: Gameplaye camera, 2: UI camera
    {
        this.shakeAmount = shakeAmount;
        this.shakeDecreaseFactor = shakeDecreaseFactor;
        this.shakeTime = shakeTime;
        this.cameraGetShake = cameraGetShake;
    }

    public void doShakeCamera()
    {
        if (shakeTime > 0)
        {
            switch (cameraGetShake)
            {
                case 1:
                    Master.Gameplay.camera.transform.localPosition = Random.insideUnitSphere * shakeAmount;
                    break;
                case 2:
                    if (Master.UIGameplay != null)
                    {
                        Master.UIGameplay.camera.transform.localPosition = Random.insideUnitSphere * shakeAmount;
                    }
                    if (Master.UIMenu != null)
                    {
                        Master.UIMenu.camera.transform.localPosition = Random.insideUnitSphere * shakeAmount;
                    }
                    break;
                case 3:
                    Master.Background.camera.transform.localPosition = Random.insideUnitSphere * shakeAmount;
                    break;
                case 4:
                    UIController.Dialog.camera.transform.localPosition = Random.insideUnitSphere * shakeAmount;
                    break;
                case 5:
                    if (Master.UIGameplay != null)
                    {
                        Master.UIGameplay.camera.transform.localPosition = Random.insideUnitSphere * shakeAmount;
                    }
                    if (Master.UIMenu != null)
                    {
                        Master.UIMenu.camera.transform.localPosition = Random.insideUnitSphere * shakeAmount;
                    }
                    Master.Gameplay.camera.transform.localPosition = Random.insideUnitSphere * shakeAmount;
                    Master.Background.camera.transform.localPosition = Random.insideUnitSphere * shakeAmount;
                    UIController.Dialog.camera.transform.localPosition = Random.insideUnitSphere * shakeAmount;

                    break;
            }
            if (Time.deltaTime > 0)
            {
                shakeTime -= Time.deltaTime * shakeDecreaseFactor;
            }
            else
            {
                shakeTime -= 0.02f * shakeDecreaseFactor;

            }
        }
        else
        {
            shakeTime = 0;
        }
    }


    public void CreateEffect(string effectName, Vector3 position, float timeToDestroy = 2f)
    {
        GameObject effectRoot = GameObject.Find("Effect Root");
        if (effectRoot == null)
        {
            effectRoot = (GameObject)Instantiate(Master.GetGameObjectInPrefabs("UI/Effect Root"), Vector3.zero, Quaternion.identity);
            effectRoot.name = "Effect Root";
        }

        GameObject obj_eff = Resources.Load<GameObject>("Prefabs/Effects/" + effectName);
        GameObject obj_eff_created = NGUITools.AddChild(effectRoot, obj_eff);

        //if (isOnUI)
        //{
        //    if (Master.UIMenu != null)
        //    {
        //        obj_eff_created = NGUITools.AddChild(Master.UIMenu.uiRoot, obj_eff);
        //        obj_eff_created.layer = LayerMask.NameToLayer("UI");
        //    }

        //    if (Master.UIGameplay != null)
        //    {
        //        obj_eff_created = NGUITools.AddChild(Master.UIGameplay.uiRoot, obj_eff);
        //        obj_eff_created.layer = LayerMask.NameToLayer("UI");
        //    }
        //}
        //else
        //{
        //    obj_eff_created = NGUITools.AddChild(Master.Gameplay.gameplayRoot, obj_eff);
        //    obj_eff_created.layer = LayerMask.NameToLayer("Gameplay");
        //}

        if (obj_eff_created.GetComponent<ParticleSystem>() != null)
        {
            obj_eff_created.GetComponent<Renderer>().sortingOrder = 1000;
        }

        obj_eff_created.transform.localPosition = position;

        Destroy(obj_eff_created, timeToDestroy);
    }


}
