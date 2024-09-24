// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InGameScene.UI
{
    //===========================================================
    // 업그레이드 UI의 아이템 클래스
    //===========================================================
    public class InGameUI_UpgradeItem : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _weaponName;
        [SerializeField] private TMP_Text _weaponExplane;
        [SerializeField] private TMP_Text _weaponAtk;
        [SerializeField] private Button _weaponBuyButton;
        [SerializeField] private Button _weaponEquipButton;
        [SerializeField] private TMP_Text _weaponStatusText;
        [SerializeField] private TMP_Text _weaponPrice;

        private BackendData.Chart.Weapon.Item _weaponInfo;

        private UnityAction _update;

        // 무기 차트의 정보에 따라 초기화
        public void Init(Sprite sprite, BackendData.Chart.Weapon.Item weaponInfo, UnityAction action)
        {
            _weaponInfo = weaponInfo;

            _image.sprite = sprite;
            _weaponName.text = weaponInfo.WeaponName;
            _weaponExplane.text = weaponInfo.WeaponExplain;

            string stat = string.Empty;
            stat += $"공격력 : +{weaponInfo.Atk}\n";
            _weaponAtk.text = stat;

            _weaponPrice.text = "가격 : " + weaponInfo.Price.ToString();

            _weaponBuyButton.onClick.AddListener(BuyButton);

            _weaponEquipButton.onClick.AddListener(EquipButton);

            _update = action;
            _update.Invoke();

            //TODO: Weapon이 Inventory에 있을경우 EquipButton을 활성화.
            foreach (var equipWeapon in StaticManager.Backend.GameData.WeaponInventory.Dictionary)
            {
                if (equipWeapon.Value.WeaponChartId == _weaponInfo.WeaponID)
                {
                    _weaponBuyButton.gameObject.SetActive(false);
                    _weaponEquipButton.gameObject.SetActive(true);
                    _weaponStatusText.text = "착용";
                }
            }

            foreach (var item in StaticManager.Backend.GameData.WeaponEquip.Dictionary.Keys)
            {
                foreach (var item2 in StaticManager.Backend.GameData.WeaponInventory.Dictionary.Values)
                {
                    if (item2.WeaponChartId == _weaponInfo.WeaponID)
                    {
                        if (item == item2.MyWeaponId.ToString())
                        {
                            _weaponStatusText.text = "착용 중";
                        }
                    }
                }
            }

        }

        // 구매 버튼 클릭시 현재 자금 무기를 비교하여 구매 여부 판단
        // 구매 가능 시, UserData를 뺄셈하고 무기 지급.
        void BuyButton()
        {
            if (StaticManager.Backend.GameData.UserData.Money < _weaponInfo.Price)
            {
                StaticManager.UI.AlertUI.OpenWarningUI("구매 불가", "현재 자금이 부족하여 해당 아이템을 구매할 수 없습니다");
                return;
            }

            InGameScene.Managers.Game.UpdateUserData(-_weaponInfo.Price, 0);
            StaticManager.UI.AlertUI.OpenAlertUI("구매 완료", _weaponInfo.WeaponName + "이(가) 구매 완료되었습니다.");
            //TODO : 바로 착용시킬것
            //this.gameObject.SetActive(false);
            InGameScene.Managers.Game.UpdateWeaponInventory(_weaponInfo.WeaponID);
            _weaponEquipButton.gameObject.SetActive(true);
            _weaponBuyButton.gameObject.SetActive(false);
            _weaponStatusText.text = "착용";
            _update.Invoke();
        }

        void EquipButton()
        {
            //무기 변경
            var weaponEquipKeys = new List<string>(StaticManager.Backend.GameData.WeaponEquip.Dictionary.Keys);

            foreach (var item in weaponEquipKeys)
            {
                foreach (var item2 in StaticManager.Backend.GameData.WeaponInventory.Dictionary.Values)
                {
                    if (item2.WeaponChartId == _weaponInfo.WeaponID)
                    {
                        Debug.Log("기존 아이디 : " + item);
                        Debug.Log("바꿀 아이디 : " + item2.MyWeaponId);
                        InGameScene.Managers.Game.UpdateWeaponEquip(item, item2.MyWeaponId);
                        StaticManager.UI.AlertUI.OpenAlertUI("착용 완료", _weaponInfo.WeaponName + "이(가) 착용되었습니다.");
                        Equip();
                    }
                }
            }
        }

        void Equip()
        {
            Transform parentTransform = this.transform.parent;

            // 부모 객체가 있는지 확인합니다.
            if (parentTransform != null)
            {
                // 부모 객체의 자식 객체 수를 가져옵니다.
                int childCount = parentTransform.childCount;

                // Dictionary의 값을 순회합니다.
                int index = 0;
                foreach (var weaponEntry in StaticManager.Backend.Chart.Weapon.Dictionary)
                {
                    // 자식 개체의 개수와 Dictionary의 개수를 비교
                    if (index >= childCount)
                    {
                        Debug.LogWarning("부모 객체의 자식 수보다 Dictionary 항목 수가 많습니다.");
                        break;
                    }

                    // 자식 객체를 가져옵니다.
                    Transform sibling = parentTransform.GetChild(index);

                    // 자식 객체에 InGameUI_UpgradeItem 스크립트가 있는지 확인합니다.
                    InGameUI_UpgradeItem upgradeItem = sibling.GetComponent<InGameUI_UpgradeItem>();

                    // 스크립트가 있으면 Init 함수를 호출합니다.
                    if (upgradeItem != null)
                    {
                        upgradeItem.Init(weaponEntry.Value.WeaponSprite, weaponEntry.Value, _update);
                    }

                    index++;
                }
            }
        }
    }
}