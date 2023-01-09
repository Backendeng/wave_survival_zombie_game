using UnityEngine;
using System.Collections;

public class ShopPanelController : MonoBehaviour
{

    // Use this for initialization
    private GameObject productListPanel;
    private GameObject pf_product;
    public bool isAssign;

    void Awake()
    {
        AssignObject();
    }

    void Start()
    {
        
        SetProductList();
    }

    public void OnOpen()
    {
       // AssignObject();
    }

    void AssignObject()
    {
        if (isAssign) return;
        isAssign = true;

        Debug.Log("Assign Shop Panel");
        productListPanel = Master.GetChildByName(gameObject, "Products");
        pf_product = Master.GetGameObjectInPrefabs("UI/Product");
    }


    void SetProductList()
    {
        foreach (ProductDataController.ProductData productData in Master.ProductData.listProductData)
        {
            GameObject product = NGUITools.AddChild(productListPanel, pf_product);
            product.GetComponent<ProductController>().SetInfo(productData);
        }
        productListPanel.GetComponent<UIGrid>().Reposition();

    }


}
