using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShopItem
{
    public string itemID = string.Empty;               
    public string itemDisplayName = string.Empty;       
    public string itemImageURL = string.Empty;          
    public bool isUnique = false;                       
    public uint itemPrice = 0;      
                        // Prix 
}

public class ShopEntry : MonoBehaviour
{
    [Header("Data")]
    public ShopItem shopItem = null;

    [Header("UI Elements")]
    public Image itemSprite = null;
    public Text itemNameText = null;
    public Text itemValueText = null;
    public Image itemValueSprite = null;

    
    void OnEnable()
    {
        
        if (PlayfabInventory.Instance != null)
        {
            
            PlayfabInventory.Instance.OnInventoryUpdateSuccess.AddListener(this.OnInventoryUpdateSuccess);
            PlayfabInventory.Instance.OnInventoryUpdateError.AddListener(this.OnInventoryUpdateError);
        }

        
        this.UpdateView();
    }

    void OnDisable()
    {
        
        if (PlayfabInventory.Instance != null)
        {
            
            PlayfabInventory.Instance.OnInventoryUpdateSuccess.RemoveListener(this.OnInventoryUpdateSuccess);
            PlayfabInventory.Instance.OnInventoryUpdateError.RemoveListener(this.OnInventoryUpdateError);
        }
    }

    // Update View
    public void SetValue(ShopItem _shopItem)
    {
        // Save catalog item
        this.shopItem = _shopItem;

        // Update view
        this.UpdateView();
    }

    public void UpdateView()
    {
        // Check item is set
        if (this.shopItem != null)
        {
            // Determine some data from the catalog item itself
            bool isUnique = (this.shopItem.isUnique == true);
            bool isPossessed = false;

            // If unique & already in inventoryk, specific view
            if (isUnique == true && PlayfabInventory.Instance.Possess(this.shopItem.itemID) == true)
            {
                
                isPossessed = true;
            }

            // Get data
            string itemImageURL = this.shopItem.itemImageURL;
            string itemName = this.shopItem.itemDisplayName;
            uint itemPrice = this.shopItem.itemPrice;

            
            if (this.itemSprite != null)
            {
                Sprite sprite = (string.IsNullOrWhiteSpace(itemImageURL) == false ? Resources.Load<Sprite>(itemImageURL) : null);
                if (sprite != null)
                    this.itemSprite.sprite = sprite;
                else
                    this.itemSprite.sprite = null;
            }

            
            if (this.itemNameText != null)
                this.itemNameText.text = itemName;

            
            if (PlayfabInventory.Instance != null && PlayfabInventory.Instance.Inventory != null)
            {
                
                if (isPossessed == true)
                {
                    
                    if (this.itemValueSprite != null)
                        this.itemValueSprite.gameObject.SetActive(false);

                    // Update value
                    if (this.itemValueText != null)
                    {
                        //this.itemValueText.alignment = TextAnchor.MiddleCenter;
                        this.itemValueText.text = "Owned";
                    }
                }
                else
                {
                    // Show item price image
                    if (this.itemValueSprite != null)
                        this.itemValueSprite.gameObject.SetActive(true);

                    // Update value
                    if (this.itemValueText != null)
                    {
                        //this.itemValueText.alignment = TextAnchor.MiddleLeft;
                        this.itemValueText.text = itemPrice.ToString();
                    }
                }
            }
        }
    }

    // UI Interactions
    public void OnBuyButtonClick()
    {
        this.TryBuyItem();
    }

    // Buy item
public void TryBuyItem()
{
    // Determine some data from the catalog item itself
    bool isUnique = (this.shopItem.isUnique == true);
    bool isPossessed = false;

    // If already in inventory
    if (PlayfabInventory.Instance.Possess(this.shopItem.itemID) == true)
    {
        // Mark as possessed
        isPossessed = true;
    }

    // If unique & already possessed, prevent buy
    if (isUnique == true && isPossessed == true)
    {
        Debug.LogWarning("ShopEntry.TryBuyItem() - " + this.gameObject.name + ": Prevent buy as it's unique & already possessed");
        return;
    }

    // Trigger item purchasing
    var request = new PurchaseItemRequest
    {
        ItemId = this.shopItem.itemID,
        VirtualCurrency = "CR",
        Price = (int)this.shopItem.itemPrice,
    };

    PlayFabClientAPI.PurchaseItem(request, result =>
    {
        // Purchase succeeded
        Debug.Log("ShopEntry.TryBuyItem() - " + this.gameObject.name + ": Purchase succeeded");
        this.OnPurchaseItemSuccess();
    }, error =>
    {
        // Purchase failed
        Debug.LogWarning("ShopEntry.TryBuyItem() - " + this.gameObject.name + ": Purchase failed: " + error.ErrorMessage);
    });
}

    private void OnPurchaseItemSuccess()
    {
        
        Debug.Log("PlayerStatsView.OnPurchaseItemSuccess()");

        
        if (PlayfabInventory.Instance != null)
            PlayfabInventory.Instance.UpdateInventory();
    }

    private void OnPurchaseItemError()
    {
        
        Debug.LogError("PlayerStatsView.OnUpdateUserAccountInfosError() - Error: TODO");
    }

   
    private void OnInventoryUpdateSuccess()
    {
        this.UpdateView();
    }

    private void OnInventoryUpdateError()
    {
        this.UpdateView();
    }
}