using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InGameScene.UI
{
    public class InGameUI_InventoryPopup_Box : MonoBehaviour
    {
        private InGameUI_Inventory _inventory;

        [SerializeField]
        private Image _boxImage;
        [SerializeField]
        private TMP_Text _boxNameText;
        [SerializeField]
        private TMP_Text _boxExplaneText;

        [SerializeField]
        private TMP_Text _boxOpenText;
        [SerializeField]
        private Button _boxOpenButton;

        private BackendData.Chart.Item.Item _boxItem;
        private BackendData.Chart.Item.Item _keyItem;

        public void SetData(BackendData.Chart.Item.Item boxItem, InGameUI_Inventory inventory)
        {
            _boxItem = boxItem;
            foreach (var item in StaticManager.Backend.Chart.Item.Dictionary)
            {
                if(item.Value.ItemID == _boxItem.ItemID + 1)
                    _keyItem = item.Value;
            }
            _inventory = inventory;

            _boxImage.sprite = _boxItem.ImageSprite;
            _boxNameText.text = _boxItem.ItemName;
            _boxExplaneText.text = _boxItem.ItemContent;

            // 기존에 등록된 모든 이벤트를 제거한 후 다시 추가
            _boxOpenButton.onClick.RemoveAllListeners();

            // 새롭게 이벤트 추가
            _boxOpenButton.onClick.AddListener(OnClickOpenButton);

            if(_boxItem.ItemType == "Key")
            {
                _boxOpenText.text = "사용 불가";
                _boxOpenButton.enabled = false;
            }
            else
            {
                _boxOpenText.text = "열기";
                _boxOpenButton.enabled = true;
            }
        }

        void OnClickOpenButton()
        {
            List<int> itemsToUse = new List<int>();

            // Box 아이템 체크
            if (StaticManager.Backend.GameData.ItemInventory.Dictionary.ContainsKey(_boxItem.ItemID))
            {
                var boxItem = StaticManager.Backend.GameData.ItemInventory.Dictionary[_boxItem.ItemID];
                if (boxItem != 0)
                {
                    itemsToUse.Add(_boxItem.ItemID);  // Box 아이템을 사용 리스트에 추가
                }
                else
                {
                    StaticManager.UI.AlertUI.OpenAlertUI("사용 불가", $"{_boxItem.ItemName}의 수량이 부족합니다.");
                    return;
                }
            }

            // Key 아이템 체크
            if (StaticManager.Backend.GameData.ItemInventory.Dictionary.ContainsKey(_keyItem.ItemID))
            {
                var keyItem = StaticManager.Backend.GameData.ItemInventory.Dictionary[_keyItem.ItemID];
                if (keyItem != 0)
                {
                    itemsToUse.Add(_keyItem.ItemID);  // Key 아이템을 사용 리스트에 추가
                }
                else
                {
                    // Key 아이템이 없으면 경고 메시지 출력
                    StaticManager.UI.AlertUI.OpenAlertUI("사용 불가", $"해당 아이템을 열기 위해선\n{_keyItem.ItemName}이 필요합니다.");
                    return;
                }
            }
            else
            {
                // Key 아이템이 Dictionary에 존재하지 않는 경우 처리
                StaticManager.UI.AlertUI.OpenAlertUI("사용 불가", $"해당 아이템을 열기 위해선\n{_keyItem.ItemName}이 필요합니다.");
                return;
            }

            // 아이템 사용
            foreach (var itemId in itemsToUse)
            {
                InGameScene.Managers.Item.Use(itemId);
            }

            // 확률 및 인벤토리 처리
            Managers.Probability.GetProbability(_boxItem.ItemName, _boxItem.ImageName);
            _inventory.Open();
            _inventory.SwitchInventory(false);

            // 사용한 아이템 목록 비우기
            itemsToUse.Clear();
        }

    }
}
