using System.Collections.Generic;
using UnityEngine;

namespace InGameScene.UI
{
    public class InGameUI_ItemInventory : MonoBehaviour
    {
        [SerializeField] private List<InGameUI_UseItem> _useItemButtons;

        private Dictionary<int, InGameUI_UseItem> _useItemDic = new Dictionary<int, InGameUI_UseItem>();

        public void Init()
        {
            foreach (var useItemButton in _useItemButtons)
            {
                int itemID = useItemButton.ItemID;
                if (StaticManager.Backend.GameData.ItemInventory.Dictionary.ContainsKey(itemID))
                {
                    int itemCount = StaticManager.Backend.GameData.ItemInventory.Dictionary[itemID];
                    useItemButton.Init(this, itemID, itemCount);
                    _useItemDic[itemID] = useItemButton;
                }
                else
                {
                    useItemButton.gameObject.SetActive(false);
                }
            }
        }

        public void UpdateUI(int itemID, int count)
        {
            if (_useItemDic.ContainsKey(itemID))
            {
                _useItemDic[itemID].UpdateCount(count);
            }
        }

        public void DeleteUseItem(int itemID)
        {
            if (_useItemDic.ContainsKey(itemID))
            {
                _useItemDic[itemID].gameObject.SetActive(false);
                _useItemDic.Remove(itemID);
            }
        }
    }
}
