// Copyright 2013-2022 AFI, INC. All rights reserved.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI
{
    //===========================================================
    // ������ �κ��丮���� ����ϴ� �������� ������ Ŭ����
    //===========================================================
    public class InGameUI_UseItem : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _itemCountText;

        [SerializeField] private TMP_Text _itemNameText;
        [SerializeField] private TMP_Text _itemInfoText;

        [SerializeField] private Button _itemUseButton;

        private BackendData.Chart.Item.Item _useItemInfo;
        private InGameUI_ItemInventory _inventory;
        private int _itemID = 0;

        // �ڽ��� �������ִ� ������ �κ��丮 UI�� �����ͼ� �ش� UI�� �ִ� ���(�����۸���Ʈ���� ����) ���
        public void Init(InGameUI_ItemInventory inventory, int itemID, int count)
        {
            _inventory = inventory;

            _itemID = itemID;
            _itemCountText.text = count.ToString();

            _useItemInfo = StaticManager.Backend.Chart.Item.Dictionary[itemID];

            _image.sprite = _useItemInfo.ImageSprite;
            _itemNameText.text = _useItemInfo.ItemName;
            _itemInfoText.text = _useItemInfo.ItemContent;

            _itemUseButton.onClick.AddListener(UseItemButton);
        }

        // 
        public void Update()
        {
            // Dictionary[_itemID] = _item�� ����
            int count = StaticManager.Backend.GameData.ItemInventory.Dictionary[_itemID];

            // count�� 0�� �Ǹ� ����Ʈ���� ����
            if (count <= 0)
            {
                _inventory.DeleteUseItem(_itemID);
            }
            else
            {
                // ���ο� count�� ����
                _itemCountText.text = count.ToString();
            }
        }

        void UseItemButton()
        {
            InGameScene.Managers.Item.Use(_itemID);
        }
    }
}