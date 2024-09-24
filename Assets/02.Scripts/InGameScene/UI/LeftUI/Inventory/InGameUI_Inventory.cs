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

            // ���� ������ �ִ� ���⸦ ������ ���������� �����.
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

        // ���⸦ �߰��ϴ� �Լ�
        public void AddWeaponObjectInInventoryUI(string myWeaponId)
        {
            // myWeaponId�� ������ �˻��Ͽ� �����͸� �޾ƿ´�.
            BackendData.GameData.WeaponInventory.Item weaponInventoryItem =
                StaticManager.Backend.GameData.WeaponInventory.Dictionary[myWeaponId];

            // ������Ʈ ����
            var obj = Instantiate(_weaponEquipItemPrefab, _weaponEquipParentObject.transform, true);
            obj.transform.localPosition = new Vector3(0, 0, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);

            // ������ ���� ������ �ʱ�ȭ
            var inventoryItem = obj.GetComponent<InGameUI_InventoryItem>();
            inventoryItem.Init(weaponInventoryItem, () => OnInventoryItemClick(inventoryItem, weaponInventoryItem));

            // ����Ʈ�� �߰��Ͽ� Focus ������ Ȱ��
            _inventoryItems.Add(inventoryItem);
        }

        // �κ��丮 ������ Ŭ�� �� ó��
        private void OnInventoryItemClick(InGameUI_InventoryItem clickedItem, BackendData.GameData.WeaponInventory.Item weaponInventoryItem)
        {
            // ��� �������� Focus�� ��Ȱ��ȭ
            foreach (var item in _inventoryItems)
            {
                item.SetFocus(false);
            }

            // Ŭ���� �����۸� Focus Ȱ��ȭ
            clickedItem.SetFocus(true);

            _weaponInventoryPopup.gameObject.SetActive(true);
            // �˾�â�� ���õ� ���� ���� ����
            _weaponInventoryPopup.SetData(weaponInventoryItem, this);
        }
    }
}