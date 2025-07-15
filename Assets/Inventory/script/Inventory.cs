using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public int space = 20;  // Số slot tối đa

    public List<Item> items = new List<Item>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Thêm item, trả về true nếu thành công
    public bool Add(Item item)
    {
        if (items.Count >= space)
        {
            Debug.Log("Inventory đầy!");
            return false;
        }
        items.Add(item);
        InventoryUI.Instance.RefreshUI();  // Cập nhật UI sau khi thêm
        return true;
    }

    // Xoá item
    public void Remove(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            InventoryUI.Instance.RefreshUI();  // Cập nhật UI sau khi xoá
        }
    }
}
