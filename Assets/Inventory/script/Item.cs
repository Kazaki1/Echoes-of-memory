using UnityEngine;

[System.Serializable]
public class Item
{
    public int id;
    public string itemName;
    public Sprite icon;

    public Item(int id, string name, Sprite icon)
    {
        this.id = id;
        this.itemName = name;
        this.icon = icon;
    }
}
    