using UnityEngine;

public class InventoryBootstrap : MonoBehaviour
{
    public GameObject inventorySystemPrefab;

    void Awake()
    {
        if (Inventory.Instance == null)
        {
            Instantiate(inventorySystemPrefab);
        }
    }
}
