using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InGameScene.UI
{
    public class InGameUI_InventoryPopup : MonoBehaviour
    {
        private InGameUI_Inventory _inventory;

        [SerializeField]
        private Image _weaponImage;
        [SerializeField]
        private TMP_Text _weaponLevelText;
        [SerializeField]
        private TMP_Text _weaponNameText;
        [SerializeField]
        private TMP_Text _weaponExplaneText;

        [SerializeField]
        private TMP_Text _weaponAtkText;
        [SerializeField]
        private TMP_Text _weaponSpeedText;
        [SerializeField]
        private TMP_Text _weaponDelayText;

        [SerializeField]
        private Button _weaponUpgradeButton;
        [SerializeField]
        private Button _weaponEquipButton;
        [SerializeField]
        private TMP_Text _weaponUpgradePriceText;
        [SerializeField]
        private TMP_Text _weaponEquipText;

        [SerializeField]
        private Image _weaponUpgradeIcon;

        private BackendData.GameData.WeaponInventory.Item _weaponItem;

        public void SetData(BackendData.GameData.WeaponInventory.Item weaponItem, InGameUI_Inventory inventory)
        {
            _weaponItem = weaponItem;
            _inventory = inventory;

            _weaponImage.sprite = _weaponItem.GetWeaponChartData().WeaponSprite;
            _weaponLevelText.text = "+" + _weaponItem.WeaponLevel.ToString();
            _weaponNameText.text = _weaponItem.GetWeaponChartData().WeaponName + " +" + _weaponItem.WeaponLevel;
            _weaponExplaneText.text = _weaponItem.GetWeaponChartData().WeaponExplain;

            _weaponAtkText.text = _weaponItem.GetCurrentWeaponStat().Atk.ToString();
            _weaponSpeedText.text = _weaponItem.GetCurrentWeaponStat().Spd.ToString();
            _weaponDelayText.text = _weaponItem.GetCurrentWeaponStat().Delay.ToString();

            _weaponUpgradeIcon.gameObject.SetActive(true);
            _weaponUpgradePriceText.text = _weaponItem.GetCurrentWeaponStat().UpgradePrice.ToString();

            _weaponEquipButton.enabled = true;

            _weaponEquipText.text = "����";

            WeaponEquipCheck();

            // ������ ��ϵ� ��� �̺�Ʈ�� ������ �� �ٽ� �߰�
            _weaponUpgradeButton.onClick.RemoveAllListeners();
            _weaponEquipButton.onClick.RemoveAllListeners();

            // ���Ӱ� �̺�Ʈ �߰�
            _weaponUpgradeButton.onClick.AddListener(OnClickUpgradeButton);
            _weaponEquipButton.onClick.AddListener(OnClickEquipButton);
        }


        void WeaponEquipCheck()
        {
            foreach (var item in StaticManager.Backend.GameData.WeaponEquip.Dictionary.Keys)
            {
                foreach (var item2 in StaticManager.Backend.GameData.WeaponInventory.Dictionary.Values)
                {
                    if (item2.WeaponChartId == _weaponItem.GetWeaponChartData().WeaponID)
                    {
                        if (item == item2.MyWeaponId.ToString())
                        {
                            _weaponEquipText.text = "���� ��";
                            _weaponEquipButton.enabled = false;
                        }
                    }
                }
            }
        }

        void OnClickUpgradeButton()
        {
            // ��� ���׷��̵� ���
            float upgradePrice = _weaponItem.GetCurrentWeaponStat().UpgradePrice;

            // ���׷��̵� ����� �� ���� ���
            if (StaticManager.Backend.GameData.UserData.Money < upgradePrice)
            {
                StaticManager.UI.AlertUI.OpenWarningUI("���׷��̵� �Ұ�", "�������� ��ȭ�� �����մϴ�.");
                return;
            }

            // ���� ����� ���, ������
            StaticManager.Backend.GameData.WeaponInventory.Dictionary[_weaponItem.MyWeaponId].LevelUp();
            // ���� ����
            _weaponLevelText.text = "+" + _weaponItem.WeaponLevel.ToString();

            // �̸� ����
            _weaponNameText.text = _weaponItem.GetWeaponChartData().WeaponName + " +" + _weaponItem.WeaponLevel;

            // ���� ����
            _weaponAtkText.text = _weaponItem.GetCurrentWeaponStat().Atk.ToString();
            _weaponSpeedText.text = _weaponItem.GetCurrentWeaponStat().Spd.ToString();
            _weaponDelayText.text = _weaponItem.GetCurrentWeaponStat().Delay.ToString();

            // money ������ ����
            InGameScene.Managers.Game.UpdateUserData(-upgradePrice, 0);

            _inventory.UpdateUI();
            _inventory.SetItemUI();

            SoundManager.Instance.PlaySFX("Upgrade");

            if(_weaponItem.GetCurrentWeaponStat().Delay < 0.2)
            {
                _weaponUpgradeButton.enabled = false;
                _weaponUpgradePriceText.text = "�ִ� ��ȭ �޼�";
                _weaponUpgradeIcon.gameObject.SetActive(false);
            }

        }

        void OnClickEquipButton()
        {
            //���� ����
            var weaponEquipKeys = new List<string>(StaticManager.Backend.GameData.WeaponEquip.Dictionary.Keys);

            foreach (var item in weaponEquipKeys)
            {
                foreach (var item2 in StaticManager.Backend.GameData.WeaponInventory.Dictionary.Values)
                {
                    if (item2.WeaponChartId == _weaponItem.GetWeaponChartData().WeaponID)
                    {
                        Debug.Log("���� ���̵� : " + item);
                        Debug.Log("�ٲ� ���̵� : " + _weaponItem.MyWeaponId);
                        InGameScene.Managers.Game.UpdateWeaponEquip(item, _weaponItem.MyWeaponId);
                        StaticManager.UI.AlertUI.OpenAlertUI("���� �Ϸ�", _weaponItem.GetWeaponChartData().WeaponName + "��(��) ����Ǿ����ϴ�.");
                        SoundManager.Instance.PlaySFX("Equip");
                        _weaponEquipText.text ="���� ��";

                        _weaponEquipButton.enabled = false;
                    }
                }
            }
        }
    }
}
