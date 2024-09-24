// Copyright 2013-2022 AFI, INC. All rights reserved.

using UnityEngine;

namespace InGameScene.UI
{
    //===========================================================
    // ���׷��̵� UI
    //===========================================================
    public class InGameUI_Upgrade : InGameUI_LeftUIBase
    {
        [SerializeField] private GameObject _upgradeParentObject;
        [SerializeField] private GameObject _upgradeItemPrefab;

        // ���� ���� ������ ����
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
            }
        }

        public override void Open()
        {
            base.Open();
        }
    }
}