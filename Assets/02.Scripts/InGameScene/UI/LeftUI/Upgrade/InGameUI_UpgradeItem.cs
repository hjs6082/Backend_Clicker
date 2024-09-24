// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
        [SerializeField] private TMP_Text _weaponStatusText;
        [SerializeField] private TMP_Text _weaponPrice;

        private BackendData.Chart.Weapon.Item _weaponInfo;

        private UnityAction _update;

        // ���� ��Ʈ�� ������ ���� �ʱ�ȭ
        public void Init(Sprite sprite, BackendData.Chart.Weapon.Item weaponInfo, UnityAction action)
        {
            _weaponInfo = weaponInfo;

            _image.sprite = sprite;
            _weaponName.text = weaponInfo.WeaponName;
            _weaponExplane.text = weaponInfo.WeaponExplain;

            string stat = string.Empty;
            stat += $"���ݷ� : +{weaponInfo.Atk}\n";
            _weaponAtk.text = stat;

            _weaponPrice.text = "���� : " + weaponInfo.Price.ToString();

            _weaponBuyButton.onClick.AddListener(BuyButton);

            _weaponEquipButton.onClick.AddListener(EquipButton);

            _update = action;
            _update.Invoke();

            //TODO: Weapon�� Inventory�� ������� EquipButton�� Ȱ��ȭ.
            foreach (var equipWeapon in StaticManager.Backend.GameData.WeaponInventory.Dictionary)
            {
                if (equipWeapon.Value.WeaponChartId == _weaponInfo.WeaponID)
                {
                    _weaponBuyButton.gameObject.SetActive(false);
                    _weaponEquipButton.gameObject.SetActive(true);
                    _weaponStatusText.text = "����";
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
                            _weaponStatusText.text = "���� ��";
                        }
                    }
                }
            }

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
            //this.gameObject.SetActive(false);
            InGameScene.Managers.Game.UpdateWeaponInventory(_weaponInfo.WeaponID);
            _weaponEquipButton.gameObject.SetActive(true);
            _weaponBuyButton.gameObject.SetActive(false);
            _weaponStatusText.text = "����";
            _update.Invoke();
        }

        void EquipButton()
        {
            //���� ����
            var weaponEquipKeys = new List<string>(StaticManager.Backend.GameData.WeaponEquip.Dictionary.Keys);

            foreach (var item in weaponEquipKeys)
            {
                foreach (var item2 in StaticManager.Backend.GameData.WeaponInventory.Dictionary.Values)
                {
                    if (item2.WeaponChartId == _weaponInfo.WeaponID)
                    {
                        Debug.Log("���� ���̵� : " + item);
                        Debug.Log("�ٲ� ���̵� : " + item2.MyWeaponId);
                        InGameScene.Managers.Game.UpdateWeaponEquip(item, item2.MyWeaponId);
                        StaticManager.UI.AlertUI.OpenAlertUI("���� �Ϸ�", _weaponInfo.WeaponName + "��(��) ����Ǿ����ϴ�.");
                        Equip();
                    }
                }
            }
        }

        void Equip()
        {
            Transform parentTransform = this.transform.parent;

            // �θ� ��ü�� �ִ��� Ȯ���մϴ�.
            if (parentTransform != null)
            {
                // �θ� ��ü�� �ڽ� ��ü ���� �����ɴϴ�.
                int childCount = parentTransform.childCount;

                // Dictionary�� ���� ��ȸ�մϴ�.
                int index = 0;
                foreach (var weaponEntry in StaticManager.Backend.Chart.Weapon.Dictionary)
                {
                    // �ڽ� ��ü�� ������ Dictionary�� ������ ��
                    if (index >= childCount)
                    {
                        Debug.LogWarning("�θ� ��ü�� �ڽ� ������ Dictionary �׸� ���� �����ϴ�.");
                        break;
                    }

                    // �ڽ� ��ü�� �����ɴϴ�.
                    Transform sibling = parentTransform.GetChild(index);

                    // �ڽ� ��ü�� InGameUI_UpgradeItem ��ũ��Ʈ�� �ִ��� Ȯ���մϴ�.
                    InGameUI_UpgradeItem upgradeItem = sibling.GetComponent<InGameUI_UpgradeItem>();

                    // ��ũ��Ʈ�� ������ Init �Լ��� ȣ���մϴ�.
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