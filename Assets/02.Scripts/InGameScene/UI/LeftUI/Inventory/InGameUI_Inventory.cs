using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InGameScene.UI
{
    public class InGameUI_Inventory : InGameUI_LeftUIBase
    {
        [SerializeField] private GameObject _weaponEquipParentObject;
        [SerializeField] private GameObject _weaponEquipItemPrefab;
        [SerializeField] private InGameUI_InventoryPopup _weaponInventoryPopup;

        private List<InGameUI_InventoryItem> _inventoryItems = new List<InGameUI_InventoryItem>();

        public override void Init()
        {
            base.Init();

            // 현재 가지고 있는 무기를 가져와 아이템으로 만든다.
            foreach (var weapon in StaticManager.Backend.GameData.WeaponInventory.Dictionary)
            {
                AddWeaponObjectInInventoryUI(weapon.Value.MyWeaponId);
            }
        }

        public override void Open()
        {
            base.Open();

            foreach (var item in _inventoryItems)
            {
                item.SetFocus(false);
            }

            _weaponInventoryPopup.gameObject.SetActive(false);
        }

        public void SetItemUI()
        {
            foreach (var item in _inventoryItems)
            {
                item.SetLevel();
            }
        }

        // 무기를 추가하는 함수
        public void AddWeaponObjectInInventoryUI(string myWeaponId)
        {
            // myWeaponId의 정보로 검색하여 데이터를 받아온다.
            BackendData.GameData.WeaponInventory.Item weaponInventoryItem =
                StaticManager.Backend.GameData.WeaponInventory.Dictionary[myWeaponId];

            // 오브젝트 생성
            var obj = Instantiate(_weaponEquipItemPrefab, _weaponEquipParentObject.transform, true);
            obj.transform.localPosition = new Vector3(0, 0, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);

            // 아이템 내부 데이터 초기화
            var inventoryItem = obj.GetComponent<InGameUI_InventoryItem>();
            inventoryItem.Init(weaponInventoryItem, () => OnInventoryItemClick(inventoryItem, weaponInventoryItem));

            // 리스트에 추가하여 Focus 관리에 활용
            _inventoryItems.Add(inventoryItem);
        }

        // 인벤토리 아이템 클릭 시 처리
        private void OnInventoryItemClick(InGameUI_InventoryItem clickedItem, BackendData.GameData.WeaponInventory.Item weaponInventoryItem)
        {
            // 모든 아이템의 Focus를 비활성화
            foreach (var item in _inventoryItems)
            {
                item.SetFocus(false);
            }

            // 클릭된 아이템만 Focus 활성화
            clickedItem.SetFocus(true);

            _weaponInventoryPopup.gameObject.SetActive(true);
            // 팝업창에 선택된 무기 정보 설정
            _weaponInventoryPopup.SetData(weaponInventoryItem, this);
        }
    }
}