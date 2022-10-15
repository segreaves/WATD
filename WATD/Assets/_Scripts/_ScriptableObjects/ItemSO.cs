using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemSO : ScriptableObject, ISerializationCallbackReceiver
{
    public string id;
    public string itemName;
    public Sprite imageSprite;
    public GameObject model;
    public ItemType itemType;
    public bool isStackable;
    [Range (1, 100)] public int stackLimit = 100;

    public void OnAfterDeserialize() {}

    public void OnBeforeSerialize()
    {
        // Generate ID
        if (string.IsNullOrEmpty(this.id))
        {
            this.id = Guid.NewGuid().ToString("N");
        }
        // Generate name from model if no name given
        if (string.IsNullOrEmpty(itemName) && model != null)
        {
            itemName = model.name;
        }
    }

    public Sprite GetImage()
    {
        return imageSprite;
    }

    public ItemType GetItemType()
    {
        return itemType;
    }

    public virtual bool IsUsable()
    {
        return false;
    }
}

public enum ItemType
{
    Useful,
    Weapon
}