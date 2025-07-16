using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("References")]
    public GameObject slotPrefab;     // Prefab slot (80×80)
    public Transform itemsParent;     // ItemContainer (có Grid Layout Group)
    public GameObject inventoryPanel; // Panel Inventory (bật/tắt bằng phím I)

    [Header("Settings")]
    [SerializeField] private int inventorySize = 20;

    private GameObject[] slots;

    #region Unity lifecycle
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

    void Start()
    {
        // Tạo slot theo số lượng inventorySize
        slots = new GameObject[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject slot = Instantiate(slotPrefab, itemsParent);
            slot.transform.localScale = Vector3.one;
            slot.name = $"Slot_{i}";
            slots[i] = slot;

            // Ẩn icon và text ban đầu
            Image icon = slot.transform.Find("ItemIcon").GetComponent<Image>();
            icon.color = new Color(1, 1, 1, 0);
            icon.sprite = null;

            // Ẩn text quantity
            Transform quantityTextTransform = slot.transform.Find("QuantityText");
            if (quantityTextTransform != null)
            {
                TextMeshProUGUI quantityText = quantityTextTransform.GetComponent<TextMeshProUGUI>();
                quantityText.text = "";
            }
        }

        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        // Phím I mở/tắt Inventory
        if (Input.GetKeyDown(KeyCode.I))
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
    #endregion

    #region Public API
    /// <summary>
    /// Cập nhật UI hiển thị items và quantity
    /// </summary>
    public void RefreshUI()
    {
        var items = Inventory.Instance.items;

        for (int i = 0; i < slots.Length; i++)
        {
            Image icon = slots[i].transform.Find("ItemIcon").GetComponent<Image>();
            Transform quantityTextTransform = slots[i].transform.Find("QuantityText");
            TextMeshProUGUI quantityText = quantityTextTransform?.GetComponent<TextMeshProUGUI>();

            if (i < items.Count && items[i].icon != null)
            {
                // Hiển thị icon
                icon.sprite = items[i].icon;
                icon.color = new Color(1, 1, 1, 1);

                // Hiển thị quantity (chỉ hiện nếu > 1)
                if (quantityText != null)
                {
                    if (items[i].quantity > 1)
                    {
                        quantityText.text = items[i].quantity.ToString();
                        quantityText.color = Color.white;
                    }
                    else
                    {
                        quantityText.text = "";
                    }
                }
            }
            else
            {
                // Ẩn icon và text
                icon.sprite = null;
                icon.color = new Color(1, 1, 1, 0);

                if (quantityText != null)
                {
                    quantityText.text = "";
                }
            }
        }
    }

    public void Toggle()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    /// <summary>
    /// Lấy thông tin item tại slot (dùng cho tooltip hoặc drag & drop)
    /// </summary>
    public Item GetItemAtSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < Inventory.Instance.items.Count)
        {
            return Inventory.Instance.items[slotIndex];
        }
        return null;
    }
    #endregion
}