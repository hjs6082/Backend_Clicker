// Copyright 2013-2022 AFI, INC. All rights reserved.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI
{
    //===========================================================
    // ���׷��̵� UI�� ������ Ŭ����
    //===========================================================
    public class InGameUI_UpgradeItem : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _weaponName;
        [SerializeField] private TMP_Text _weaponExplane;
        [SerializeField] private TMP_Text _weaponAtk;
        [SerializeField] private Button _weaponBuyButton;
        [SerializeField] private Button _weaponEquipButton;
        [SerializeField] private TMP_Text _weaponPrice;

        private BackendData.Chart.Weapon.Item _weaponInfo;

        // ���� ��Ʈ�� ������ ���� �ʱ�ȭ
        public void Init(Sprite sprite, BackendData.Chart.Weapon.Item weaponInfo)
        {
            _weaponInfo = weaponInfo;

            _image.sprite = sprite;
            _weaponName.text = weaponInfo.WeaponName;
            _weaponExplane.text = weaponInfo.WeaponExplain;

            string stat = string.Empty;
            stat += $"���ݷ� : +{weaponInfo.Atk}\n";
            _weaponAtk.text = stat;

            _weaponPrice.text = weaponInfo.Price.ToString();

            _weaponBuyButton.onClick.AddListener(BuyButton);
            
            //TODO: Weapon�� Inventory�� ������� EquipButton�� Ȱ��ȭ.
        }

        // ���� ��ư Ŭ���� ���� �ڱ� ���⸦ ���Ͽ� ���� ���� �Ǵ�
        // ���� ���� ��, UserData�� �����ϰ� ���� ����.
        void BuyButton()
        {
            if (StaticManager.Backend.GameData.UserData.Money < _weaponInfo.Price)
            {
                StaticManager.UI.AlertUI.OpenWarningUI("���� �Ұ�", "���� �ڱ��� �����Ͽ� �ش� �������� ������ �� �����ϴ�");
                return;
            }

            InGameScene.Managers.Game.UpdateUserData(-_weaponInfo.Price, 0);
            StaticManager.UI.AlertUI.OpenAlertUI("���� �Ϸ�", _weaponInfo.WeaponName + "��(��) ���� �Ϸ�Ǿ����ϴ�.");
            //TODO : �ٷ� �����ų��
            InGameScene.Managers.Game.UpdateWeaponInventory(_weaponInfo.WeaponID);
            _weaponEquipButton.gameObject.SetActive(true);
        }
    }
}