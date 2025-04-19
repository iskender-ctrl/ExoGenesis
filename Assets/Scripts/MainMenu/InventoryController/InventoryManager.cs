using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    private List<string> ownedRockets = new List<string>();
    private List<string> ownedCollectionItems = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddRocket(string rocketName)
    {
        if (!ownedRockets.Contains(rocketName))
        {
            ownedRockets.Add(rocketName);
            SaveInventory();
        }
    }

    public void AddCollectionItem(string itemName)
    {
        if (!ownedCollectionItems.Contains(itemName))
        {
            ownedCollectionItems.Add(itemName);
            SaveInventory();
        }
    }

    public bool HasRocket(string rocketName)
    {
        return ownedRockets.Contains(rocketName);
    }

    public List<string> GetRockets()
    {
        return ownedRockets;
    }

    public List<string> GetCollectionItems()
    {
        return ownedCollectionItems;
    }

    private void SaveInventory()
    {
        PlayerPrefs.SetString("Rockets", string.Join(",", ownedRockets));
        PlayerPrefs.SetString("CollectionItems", string.Join(",", ownedCollectionItems));
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        string rockets = PlayerPrefs.GetString("Rockets", "");
        string collections = PlayerPrefs.GetString("CollectionItems", "");

        if (!string.IsNullOrEmpty(rockets))
            ownedRockets = new List<string>(rockets.Split(','));

        if (!string.IsNullOrEmpty(collections))
            ownedCollectionItems = new List<string>(collections.Split(','));
    }
}
