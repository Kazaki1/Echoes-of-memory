using UnityEngine;
using UnityEngine.UI;

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
        DontDestroyOnLoad(gameObject);          // Giữ qua scene khác (tuỳ chọn)
    }

    void Start()
    {
        // Tạo slot theo số lượng inventorySize
        slots = new GameObject[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject slot = Instantiate(slotPrefab, itemsParent);
            slot.transform.localScale = Vector3.one;  // quan trọng để slot không bị scale sai
            slot.name = $"Slot_{i}";
            slots[i] = slot;

            // Ẩn icon ban đầu
            Image icon = slot.transform.Find("ItemIcon").GetComponent<Image>();
            icon.color = new Color(1, 1, 1, 0);
            icon.sprite = null;
        }

        inventoryPanel.SetActive(false);  // ẩn UI khi khởi chạy
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
    /// Gọi hàm này bất cứ khi nào danh sách Item thay đổi để cập nhật UI.
    /// </summary>
    public void RefreshUI()
    {
        var items = Inventory.Instance.items;   // danh sách item hiện có

        for (int i = 0; i < slots.Length; i++)
        {
            Image icon = slots[i].transform.Find("ItemIcon").GetComponent<Image>();

            if (i < items.Count && items[i].icon != null)
            {
                icon.sprite = items[i].icon;
                icon.color = new Color(1, 1, 1, 1);   // hiện icon
            }
            else
            {
                icon.sprite = null;
                icon.color = new Color(1, 1, 1, 0);   // ẩn icon
            }
        }
    }


    public void Toggle()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
    #endregion
}
