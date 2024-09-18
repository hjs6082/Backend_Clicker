using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI
{
    public class InGameUI_UseItem : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _itemCountText;
        [SerializeField] private TMP_Text _itemNameText;
        //[SerializeField] private TMP_Text _itemInfoText;
        [SerializeField] private Button _itemUseButton;

        private BackendData.Chart.Item.Item _useItemInfo;
        private InGameUI_ItemInventory _inventory;
        [SerializeField] private int _itemID;

        public int ItemID => _itemID;

        public void Init(InGameUI_ItemInventory inventory, int itemID, int count)
        {
            _inventory = inventory;
            _itemID = itemID;
            _itemCountText.text = count.ToString();

            _useItemInfo = StaticManager.Backend.Chart.Item.Dictionary[itemID];

            _image.sprite = _useItemInfo.ImageSprite;
            _itemNameText.text = _useItemInfo.ItemName;
            //_itemInfoText.text = _useItemInfo.ItemContent;

            _itemUseButton.onClick.AddListener(UseItem);
        }

        public void UpdateCount(int count)
        {
            int.TryParse(_itemCountText.text, out int itemCount);
            if (itemCount > 0)
            {
                itemCount += count;
                _itemCountText.text = itemCount.ToString();

                //_inventory.DeleteUseItem(_itemID);
            }
            else
            {
                _itemCountText.text = count.ToString();
            }
        }

        private void UseItem()
        {
            int.TryParse(_itemCountText.text, out int itemCount);
            if (itemCount == 0)
            {
                StaticManager.UI.AlertUI.OpenAlertUI("아이템 부족", "사용할 아이템의 수량이 부족합니다.");
                return;
            }
            InGameScene.Managers.Item.Use(_itemID);
            _itemCountText.text = StaticManager.Backend.GameData.ItemInventory.Dictionary[_itemID].ToString();
        }
    }
}
