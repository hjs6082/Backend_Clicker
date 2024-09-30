// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

namespace InGameScene.UI
{
    //===========================================================
    // 업그레이드 UI
    //===========================================================
    public class InGameUI_Upgrade : InGameUI_LeftUIBase
    {
        [SerializeField] private GameObject _upgradeParentObject;
        [SerializeField] private GameObject _upgradeItemPrefab;

        [SerializeField] private List<InGameUI_UpgradeItem> _upgradeItemList; 

        // 무기 상점 아이템 생성
        public override void Init()
        {
            base.Init();

            foreach (var weapon in StaticManager.Backend.Chart.Weapon.Dictionary)
            {
                Sprite sprite = weapon.Value.WeaponSprite;

                var newWeapon = Instantiate(_upgradeItemPrefab, _upgradeParentObject.transform, true);
                newWeapon.transform.localPosition = new Vector3(0, 0, 0);
                newWeapon.transform.localScale = new Vector3(1, 1, 1);

                newWeapon.GetComponent<InGameUI_UpgradeItem>().Init(sprite, weapon.Value, UpdateUI);
                _upgradeItemList.Add(newWeapon.GetComponent<InGameUI_UpgradeItem>());
            }
        }

        public override void Open()
        {
            base.Open();
            foreach (var upgradeItem in _upgradeItemList)
            {
                upgradeItem.EquipCheck();
            }
        }
    }
}