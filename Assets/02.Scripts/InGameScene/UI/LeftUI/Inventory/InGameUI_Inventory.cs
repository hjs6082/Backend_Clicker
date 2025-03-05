using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InGameScene.UI
{
    public class InGameUI_Inventory : InGameUI_LeftUIBase
    {
        [SerializeField] private GameObject _equipParentObject;  // 무기 및 상자 아이템을 담을 부모 객체
        [SerializeField] private GameObject _weaponEquipItemPrefab;
        [SerializeField] private GameObject _boxEquipItemPrefab;
        [SerializeField] private InGameUI_InventoryPopup _weaponInventoryPopup;  // 무기 팝업
        [SerializeField] private InGameUI_InventoryPopup_Box _boxInventoryPopup;  // 상자 팝업

        [SerializeField] private Button _weaponInventoryButton;  // 무기 인벤토리 버튼
        [SerializeField] private Button _boxInventoryButton;     // 상자 인벤토리 버튼
        [SerializeField] private GameObject[] _inventoryFocusObject; // 버튼 선택 상태를 나타내는 오브젝트 배열

        private List<InGameUI_InventoryItem> _inventoryItems = new List<InGameUI_InventoryItem>();  // 무기 아이템 리스트
        private List<InGameUI_InventoryItem_Box> _boxItems = new List<InGameUI_InventoryItem_Box>(); // 상자 아이템 리스트
        private bool _isWeaponInventory = true;  // 무기 인벤토리 활성화 상태 플래그

        public override void Init()
        {
            base.Init();

            // 무기 인벤토리 초기화
            foreach (var weapon in StaticManager.Backend.GameData.WeaponInventory.Dictionary)
            {
                AddWeaponObjectInInventoryUI(weapon.Value.MyWeaponId);
            }

            // 상자 아이템 초기화
            foreach (var item in StaticManager.Backend.GameData.ItemInventory.Dictionary)
            {
                foreach (var item2 in StaticManager.Backend.Chart.Item.Dictionary.Values)
                {
                    if (item.Key == item2.ItemID && (item.Value != 0) && (item2.ItemType == "Box" || item2.ItemType == "Key"))
                    {
                        AddBoxObjectInInventoryUI(item.Key);
                    }
                }
            }

            // 버튼 클릭 이벤트 설정
            _weaponInventoryButton.onClick.AddListener(() => SwitchInventory(true));
            _boxInventoryButton.onClick.AddListener(() => SwitchInventory(false));
        }

        public override void Open()
        {
            base.Open();

            // 무기/상자 아이템 업데이트
            UpdateWeaponInventory();
            UpdateBoxInventory();

            // 기본적으로 무기 인벤토리를 표시
            SwitchInventory(true);
        }

        // 무기 아이템 UI 추가 함수
        public void AddWeaponObjectInInventoryUI(string myWeaponId)
        {
            var weaponInventoryItem = StaticManager.Backend.GameData.WeaponInventory.Dictionary[myWeaponId];
            var obj = Instantiate(_weaponEquipItemPrefab, _equipParentObject.transform, true);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            var inventoryItem = obj.GetComponent<InGameUI_InventoryItem>();
            inventoryItem.Init(weaponInventoryItem, () => OnInventoryItemClick(inventoryItem, weaponInventoryItem));

            _inventoryItems.Add(inventoryItem);
        }

        // 상자 아이템 UI 추가 함수
        public void AddBoxObjectInInventoryUI(int boxItemId)
        {
            BackendData.Chart.Item.Item boxInventoryItem = null;
            foreach (var item2 in StaticManager.Backend.Chart.Item.Dictionary.Values)
            {
                if (boxItemId == item2.ItemID && (item2.ItemType == "Box" || item2.ItemType == "Key"))
                {
                    boxInventoryItem = item2;
                }
            }
            var obj = Instantiate(_boxEquipItemPrefab, _equipParentObject.transform, true);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            var inventoryItem = obj.GetComponent<InGameUI_InventoryItem_Box>();
            inventoryItem.Init(boxInventoryItem, () => OnBoxInventoryItemClick(inventoryItem, boxInventoryItem));

            _boxItems.Add(inventoryItem);
        }

        // 무기 아이템 클릭 시 처리
        private void OnInventoryItemClick(InGameUI_InventoryItem clickedItem, BackendData.GameData.WeaponInventory.Item weaponInventoryItem)
        {
            // 다른 팝업이 열려 있으면 닫기
            CloseAllPopups();

            foreach (var item in _inventoryItems)
            {
                item.SetFocus(false);
            }
            foreach (var item in _boxItems)
            {
                item.SetFocus(false);
            }
            clickedItem.SetFocus(true);

            _weaponInventoryPopup.gameObject.SetActive(true);
            _weaponInventoryPopup.SetData(weaponInventoryItem, this);
        }

        // 상자 아이템 클릭 시 처리
        private void OnBoxInventoryItemClick(InGameUI_InventoryItem_Box clickedItem, BackendData.Chart.Item.Item boxInventoryItem)
        {
            // 다른 팝업이 열려 있으면 닫기
            CloseAllPopups();

            foreach (var item in _inventoryItems)
            {
                item.SetFocus(false);
            }
            foreach (var item in _boxItems)
            {
                item.SetFocus(false);
            }
            clickedItem.SetFocus(true);

            _boxInventoryPopup.gameObject.SetActive(true);
            _boxInventoryPopup.SetData(boxInventoryItem, this);
        }

        // 무기/상자 인벤토리 전환 함수
        public void SwitchInventory(bool showWeaponInventory)
        {
            _isWeaponInventory = showWeaponInventory;

            // 무기 아이템 활성화/비활성화
            SetInventoryItemsActive(_inventoryItems, showWeaponInventory);
            // 상자 아이템 활성화/비활성화
            SetInventoryItemsActive(_boxItems, !showWeaponInventory);

            // 버튼 선택 상태 업데이트
            _inventoryFocusObject[0].SetActive(showWeaponInventory);
            _inventoryFocusObject[1].SetActive(!showWeaponInventory);
        }

        // 무기 팝업과 상자 팝업 모두 닫는 함수
        private void CloseAllPopups()
        {
            _weaponInventoryPopup.gameObject.SetActive(false);
            _boxInventoryPopup.gameObject.SetActive(false);
        }

        // 기존 무기 아이템들 업데이트
        private void UpdateWeaponInventory()
        {
            List<GameObject> itemsToDelete = new List<GameObject>();
            foreach (var item in _inventoryItems)
            {
                itemsToDelete.Add(item.gameObject);
            }
            foreach (var item in itemsToDelete)
            {
                Destroy(item);
            }
            _inventoryItems.Clear();

            foreach (var weapon in StaticManager.Backend.GameData.WeaponInventory.Dictionary)
            {
                AddWeaponObjectInInventoryUI(weapon.Value.MyWeaponId);
            }

            _weaponInventoryPopup.gameObject.SetActive(false);
        }

        // 기존 상자 아이템들 업데이트
        private void UpdateBoxInventory()
        {
            List<GameObject> itemsToDelete = new List<GameObject>();
            foreach (var item in _boxItems)
            {
                itemsToDelete.Add(item.gameObject);
            }
            foreach (var item in itemsToDelete)
            {
                Destroy(item);
            }
            _boxItems.Clear();

            foreach (var item in StaticManager.Backend.GameData.ItemInventory.Dictionary)
            {
                foreach (var item2 in StaticManager.Backend.Chart.Item.Dictionary.Values)
                {
                    if (item.Key == item2.ItemID && (item.Value != 0) && (item2.ItemType == "Box" || item2.ItemType == "Key"))
                    {
                        AddBoxObjectInInventoryUI(item.Key);
                    }
                }
            }

            _boxInventoryPopup.gameObject.SetActive(false);
        }

        // 아이템 활성화/비활성화 설정
        private void SetInventoryItemsActive<T>(List<T> items, bool isActive) where T : MonoBehaviour
        {
            foreach (var item in items)
            {
                item.gameObject.SetActive(isActive);
            }
        }

        public void SetItemUI()
        {
            foreach (var item in _inventoryItems)
            {
                item.SetLevel();
            }
        }
    }
}
