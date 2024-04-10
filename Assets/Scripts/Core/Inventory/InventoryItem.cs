using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLOTC.Core.Inventory
{
    public abstract class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] string _itemID;
        [SerializeField] Sprite _sprite;
        [SerializeField] string _displayName;
        [SerializeField, TextArea(5, 8)] string _description;
        [SerializeField] bool _stackable;

        private static Dictionary<string, InventoryItem> s_itemLookupCache;

        public string ItemID { get { return _itemID; } }
        public Sprite Sprite { get { return _sprite; } }
        public string DisplayName { get { return _displayName; } }
        public string Description { get { return _description; } }
        public bool Stackable { get { return _stackable; } }

        public static InventoryItem GetFromID(string itemID)
        {
            if (s_itemLookupCache  == null)
            {
                s_itemLookupCache = new Dictionary<string, InventoryItem>();
                InventoryItem[] itemList = Resources.LoadAll<InventoryItem>("");
                foreach (InventoryItem item in  itemList)
                {
                    if (s_itemLookupCache.ContainsKey(item._itemID))
                    {
                        Debug.LogError(string.Format("Id must be unique: {0} and {1}", s_itemLookupCache[item._itemID], item));
                        continue;
                    }

                    s_itemLookupCache[item._itemID] = item;
                }
            }

            if (string.IsNullOrWhiteSpace(itemID) || !s_itemLookupCache.ContainsKey(itemID)) 
                return null;

            return s_itemLookupCache[itemID];
        }

        public void OnBeforeSerialize()
        {
            if (string.IsNullOrWhiteSpace(_itemID))
            {
                _itemID = System.Guid.NewGuid().ToString();
            }
        }

        public void OnAfterDeserialize()
        {

        }
    }
}