using UnityEngine;
using System.Collections;

public class IAPController : MonoBehaviour
{

    // Use this for initialization

    //for remove ad item

    //[Header("Platform Setting")]
    //public string androidPublicKey = "";

    private System.Action onPurchaseComplete;
    private System.Action onPurchaseFail;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        if (Master.IAP != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Master.IAP = this;
        }
    }

    void Start()
    {
        if (!Master.Ad.isRemoveAd)
        {
            UM_InAppPurchaseManager.OnPurchaseFlowFinishedAction += OnPurchaseFlowFinishedAction;
            UM_InAppPurchaseManager.OnBillingConnectFinishedAction += OnConnectFinished;
            UM_InAppPurchaseManager.OnBillingConnectFinishedAction += OnBillingConnectFinishedAction;
            UM_InAppPurchaseManager.Instance.Init();
        }
    }

    void Update()
    {

    }

    public void PurchaseProduct(string idProduct, System.Action onComplete = null, System.Action onFail = null)
    {
        this.onPurchaseComplete = onComplete;
        this.onPurchaseFail = onFail;
        UM_InAppPurchaseManager.Instance.Purchase(idProduct);
    }

    private void OnConnectFinished(UM_BillingConnectionResult result)
    {

        if (result.isSuccess)
        {
        }
        else
        {
        }
    }

    private void OnPurchaseFlowFinishedAction(UM_PurchaseResult result)
    {
        if (result.isSuccess)
        {
            Debug.Log("Purchase Success!");
            if (onPurchaseComplete != null)
            {
                onPurchaseComplete();
                onPurchaseComplete = null;
            }
        }
        else
        {
            Debug.Log("Purchase Fail!");
            if (onPurchaseFail != null)
            {
                onPurchaseFail();
                onPurchaseFail = null;
            }
        }
    }

    private void OnBillingConnectFinishedAction(UM_BillingConnectionResult result)
    {
        if (result.isSuccess)
        {
            Debug.Log("Billing Connected");
        }
        else
        {
            Debug.Log("Billing Failed to connect");
        }
    }


    public void AddProduct(string productID, string productSKU, bool isConsumable)
    {
        UM_InAppProduct checkProduct = UltimateMobileSettings.Instance.GetProductById(productID);
        if (checkProduct == null)
        {
            UM_InAppProduct product = new UM_InAppProduct();
            product.id = productID;
            product.AndroidId = productSKU;
            product.IOSId = productSKU;
            product.WP8Id = productSKU;
            product.IsConsumable = isConsumable;
            UltimateMobileSettings.Instance.AddProduct(product);
        }
    }

}
