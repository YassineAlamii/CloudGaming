using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public string shopName = "";
    public GameObject shopEntryPrefab = null;
    
    private List<ShopEntry> shopEntries = new List<ShopEntry>();

    // Start is called before the first frame update
    void OnEnable()
    {
        
        if (this.shopEntryPrefab != null)
            this.shopEntryPrefab.gameObject.SetActive(false);

        // Refresh prefab
        this.RefreshLeaderboard();
    }

    // Refresh
    public void RefreshLeaderboard()
    {
        
        if (this.shopEntryPrefab == null)
            return;

       
        if (PlayfabAuth.IsLoggedIn == true)
        {
          
            PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogItemsSuccess, OnGetCatalogItemsError);
        }
    }

    private void OnGetCatalogItemsSuccess(GetCatalogItemsResult result)
{
    // Clear existing entries
    this.ClearExistingEntries();

    // Retrieve data
    List<ShopItem> items = new List<ShopItem>();
    foreach (CatalogItem catalogItem in result.Catalog)
    {
        // Create ShopItem from CatalogItem
        ShopItem shopItem = new ShopItem();
        shopItem.itemID = catalogItem.ItemId;
        shopItem.itemDisplayName = catalogItem.DisplayName;
        shopItem.itemImageURL = catalogItem.ItemImageUrl;
        shopItem.isUnique = catalogItem.IsStackable == false;
        shopItem.itemPrice = (uint)catalogItem.VirtualCurrencyPrices["CR"];
        items.Add(shopItem);
    }

    // Instantiate objects and set values
    if (items != null)
    {
        for (int i = 0; i < items.Count; i++)
        {
            ShopItem shopItem = items[i];
            if (shopItem != null)
            {
                // Instantiate object copy
                GameObject shopEntryGameobjectCopy = GameObject.Instantiate(this.shopEntryPrefab, this.shopEntryPrefab.transform.parent);
                if (shopEntryGameobjectCopy != null)
                {
                    // Activate as our prefab is deactivated
                    shopEntryGameobjectCopy.gameObject.SetActive(true);

                    // set name
                    shopEntryGameobjectCopy.name = ("ShopItemEntry (" + shopItem.itemDisplayName + ")");

                    
                    ShopEntry shopEntry = shopEntryGameobjectCopy.GetComponent<ShopEntry>();
                    if (shopEntry != null)
                    {
                        
                        shopEntry.SetValue(shopItem);

                        
                        if (this.shopEntries == null)
                            this.shopEntries = new List<ShopEntry>();
                        this.shopEntries.Add(shopEntry);
                    }
                    
                    else
                    {
                        GameObject.Destroy(shopEntryGameobjectCopy);
                    }
                }
            }
        }
    }
}

    private void OnGetCatalogItemsError(PlayFabError error)
    {

        Debug.LogError("Shop.OnGetCatalogItemsError() - Error: " + error.GenerateErrorReport());
    }

    public void ClearExistingEntries()
    {
        if (this.shopEntries != null)
        {
            while (this.shopEntries.Count > 0)
            {
                if (this.shopEntries[0] != null)
                {
                    GameObject.Destroy(this.shopEntries[0].gameObject);
                }

                this.shopEntries.RemoveAt(0);
            }
        }
    }
}
