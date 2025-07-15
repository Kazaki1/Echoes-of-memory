using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ItemButtonOpener : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            if (InventoryUI.Instance != null)
                InventoryUI.Instance.Toggle();
            else
                Debug.LogWarning("InventoryUI.Instance is null!");
        });
    }
}
