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

    // Thêm item với stack support
    public bool Add(Item item)
    {
        if (item == null || item.quantity <= 0) return false;

        int remainingQuantity = item.quantity;

        // Nếu item có thể stack, tìm stack hiện có
        if (item.isStackable)
        {
            foreach (Item existingItem in items)
            {
                if (existingItem.IsSameItem(item))
                {
                    int addedQuantity = existingItem.AddQuantity(remainingQuantity);
                    remainingQuantity -= addedQuantity;

                    if (remainingQuantity <= 0)
                    {
                        InventoryUI.Instance.RefreshUI();
                        return true;
                    }
                }
            }
        }

        // Tạo stack mới cho số lượng còn lại
        while (remainingQuantity > 0)
        {
            if (items.Count >= space)
            {
                Debug.Log($"Inventory đầy! Không thể thêm {remainingQuantity} {item.itemName}");
                return false;
            }

            Item newStack = item.Clone();
            newStack.quantity = 0;

            int quantityToAdd = Mathf.Min(remainingQuantity, newStack.maxStack);
            newStack.AddQuantity(quantityToAdd);

            items.Add(newStack);
            remainingQuantity -= quantityToAdd;
        }

        InventoryUI.Instance.RefreshUI();
        return true;
    }

    // Thêm item theo ID và quantity
    public bool Add(int itemId, int quantity)
    {
        // Tìm item template từ ItemDatabase hoặc tạo mới
        Item itemToAdd = CreateItemById(itemId);
        if (itemToAdd == null)
        {
            Debug.LogError($"Không tìm thấy item với ID: {itemId}");
            return false;
        }

        itemToAdd.quantity = quantity;
        return Add(itemToAdd);
    }

    // Xoá item (xoá toàn bộ stack)
    public void Remove(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            InventoryUI.Instance.RefreshUI();
        }
    }

    // Xoá một số lượng nhất định của item
    public bool RemoveQuantity(int itemId, int quantity)
    {
        int remainingToRemove = quantity;

        for (int i = items.Count - 1; i >= 0; i--)
        {
            Item item = items[i];
            if (item.id == itemId)
            {
                int removed = item.RemoveQuantity(remainingToRemove);
                remainingToRemove -= removed;

                // Nếu stack hết, xoá khỏi inventory
                if (item.quantity <= 0)
                {
                    items.RemoveAt(i);
                }

                if (remainingToRemove <= 0)
                {
                    InventoryUI.Instance.RefreshUI();
                    return true;
                }
            }
        }

        Debug.Log($"Không đủ {GetItemName(itemId)} để xoá. Còn thiếu: {remainingToRemove}");
        InventoryUI.Instance.RefreshUI();
        return false;
    }

    // Lấy tổng số lượng của một item
    public int GetItemQuantity(int itemId)
    {
        int total = 0;
        foreach (Item item in items)
        {
            if (item.id == itemId)
            {
                total += item.quantity;
            }
        }
        return total;
    }

    // Kiểm tra có đủ số lượng item không
    public bool HasEnoughQuantity(int itemId, int requiredQuantity)
    {
        return GetItemQuantity(itemId) >= requiredQuantity;
    }

    // Lấy tên item theo ID
    private string GetItemName(int itemId)
    {
        foreach (Item item in items)
        {
            if (item.id == itemId)
            {
                return item.itemName;
            }
        }
        return "Unknown Item";
    }

    // Tạo item theo ID (bạn có thể thay thế bằng ItemDatabase)
    private Item CreateItemById(int itemId)
    {
        // Đây là ví dụ đơn giản, bạn nên tạo ItemDatabase để quản lý
        switch (itemId)
        {
            case 1:
                return new Item(1, "Health Potion", null, 1, 99, true);
            case 2:
                return new Item(2, "Sword", null, 1, 1, false);
            case 3:
                return new Item(3, "Gold Coin", null, 1, 999, true);
            default:
                return null;
        }
    }
}