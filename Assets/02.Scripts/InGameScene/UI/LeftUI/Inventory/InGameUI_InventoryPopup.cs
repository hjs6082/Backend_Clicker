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

            _weaponEquipText.text = "착용";

            WeaponEquipCheck();

            // 기존에 등록된 모든 이벤트를 제거한 후 다시 추가
            _weaponUpgradeButton.onClick.RemoveAllListeners();
            _weaponEquipButton.onClick.RemoveAllListeners();

            // 새롭게 이벤트 추가
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
                            _weaponEquipText.text = "착용 중";
                            _weaponEquipButton.enabled = false;
                        }
                    }
                }
            }
        }

        void OnClickUpgradeButton()
        {
            // 장비 업그레이드 비용
            float upgradePrice = _weaponItem.GetCurrentWeaponStat().UpgradePrice;

            // 업그레이드 비용이 더 높을 경우
            if (StaticManager.Backend.GameData.UserData.Money < upgradePrice)
            {
                StaticManager.UI.AlertUI.OpenWarningUI("업그레이드 불가", "보유중인 재화가 부족합니다.");
                return;
            }

            // 돈이 충분할 경우, 레벨업
            StaticManager.Backend.GameData.WeaponInventory.Dictionary[_weaponItem.MyWeaponId].LevelUp();
            // 레벨 갱신
            _weaponLevelText.text = "+" + _weaponItem.WeaponLevel.ToString();

            // 이름 갱신
            _weaponNameText.text = _weaponItem.GetWeaponChartData().WeaponName + " +" + _weaponItem.WeaponLevel;

            // 스텟 갱신
            _weaponAtkText.text = _weaponItem.GetCurrentWeaponStat().Atk.ToString();
            _weaponSpeedText.text = _weaponItem.GetCurrentWeaponStat().Spd.ToString();
            _weaponDelayText.text = _weaponItem.GetCurrentWeaponStat().Delay.ToString();

            // money 데이터 감소
            InGameScene.Managers.Game.UpdateUserData(-upgradePrice, 0);

            _inventory.UpdateUI();
            _inventory.SetItemUI();

            SoundManager.Instance.PlaySFX("Upgrade");

            if(_weaponItem.GetCurrentWeaponStat().Delay < 0.2)
            {
                _weaponUpgradeButton.enabled = false;
                _weaponUpgradePriceText.text = "최대 강화 달성";
                _weaponUpgradeIcon.gameObject.SetActive(false);
            }

        }

        void OnClickEquipButton()
        {
            //무기 변경
            var weaponEquipKeys = new List<string>(StaticManager.Backend.GameData.WeaponEquip.Dictionary.Keys);

            foreach (var item in weaponEquipKeys)
            {
                foreach (var item2 in StaticManager.Backend.GameData.WeaponInventory.Dictionary.Values)
                {
                    if (item2.WeaponChartId == _weaponItem.GetWeaponChartData().WeaponID)
                    {
                        Debug.Log("기존 아이디 : " + item);
                        Debug.Log("바꿀 아이디 : " + _weaponItem.MyWeaponId);
                        InGameScene.Managers.Game.UpdateWeaponEquip(item, _weaponItem.MyWeaponId);
                        StaticManager.UI.AlertUI.OpenAlertUI("착용 완료", _weaponItem.GetWeaponChartData().WeaponName + "이(가) 착용되었습니다.");
                        SoundManager.Instance.PlaySFX("Equip");
                        _weaponEquipText.text ="착용 중";

                        _weaponEquipButton.enabled = false;
                    }
                }
            }
        }
    }
}
