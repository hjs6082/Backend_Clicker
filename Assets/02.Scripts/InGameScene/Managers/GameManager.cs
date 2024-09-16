// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using BackendData.Chart.Quest;
using InGameScene.UI;

namespace InGameScene
{
    //===========================================================
    // ������ ���, ���� �������� UI�� ������ ������ �ѹ��� �����ϴ� Ŭ����
    //===========================================================
    public class GameManager
    {
        private UIManager _uiManager;
        private Player _player;

        public void Init(Player player, UIManager uiManager)
        {
            _uiManager = uiManager;
            _player = player;
        }

        // UserData�� ���õ� UI�� ������ ����� �����͸� �����ϴ� �Լ�
        // �ش� �Լ��� ���ؼ��� UserData ���� ����
        public void UpdateUserData(float money, float exp)
        {
            try
            {
                // ������ ������ ���, ȹ�淮 ����
                if (money > 0)
                {
                    money = Managers.Buff.GetBuffedStat(Buff.BuffStatType.Gold, money);
                }
                if (exp > 0)
                {
                    exp = Managers.Buff.GetBuffedStat(Buff.BuffStatType.Exp, exp);
                }

                // ������ ȹ�淮��ŭ GameData�� UserData ������Ʈ
                StaticManager.Backend.GameData.UserData.UpdateUserData(money, exp);

                // ����� �����Ϳ� �°� UserUI ����(���� ���)
                _uiManager.UserUI.UpdateUI();

                // ����Ʈ�� ������ ��ĥ �����Ͱ� �����Ѵٸ� QuestUI�� ������Ʈ
                if (exp > 0)
                {
                    _uiManager.LeftUI.GetUI<InGameUI_Quest>().UpdateUI(QuestType.LevelUp);
                }
                if (money < 0)
                {
                    _uiManager.LeftUI.GetUI<InGameUI_Quest>().UpdateUI(QuestType.UseGold);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"UpdateUserData({money}, {exp}) �� ������ �߻��Ͽ����ϴ�\n{e}");
            }
        }

        // Gem�� ���õ� UserData�� �����ϴ� �Լ�
        public void UpdateGem(int gem)
        {
            try
            {
/*                // ������ ������ ���, ȹ�淮 ����
                if (money > 0)
                {
                    money = Managers.Buff.GetBuffedStat(Buff.BuffStatType.Gold, money);
                }
                if (exp > 0)
                {
                    exp = Managers.Buff.GetBuffedStat(Buff.BuffStatType.Exp, exp);
                }*/

                // ������ ȹ�淮��ŭ GameData�� UserData ������Ʈ
                StaticManager.Backend.GameData.UserData.UpdateGem(gem);

                // ����� �����Ϳ� �°� UserUI ����(���� ���)
                _uiManager.UserUI.UpdateUI();
            }
            catch (Exception e)
            {
                throw new Exception($"UpdateGem({gem}}}) �� ������ �߻��Ͽ����ϴ�\n{e}");
            }
        }

        // PlayerIcon�� ���õ� UserData�� �����ϴ� �Լ�
        public void UpdatePlayerIcon(int playerIconNum)
        {
            try
            {
                StaticManager.Backend.GameData.UserData.UpdatePlayerIcon(playerIconNum);

                // ����� �����Ϳ� �°� UserUI ����(���� ���)
                _uiManager.UserUI.UpdateUI();
            }
            catch (Exception e)
            {
                throw new Exception($"UpdatePlayerIcon({playerIconNum}}}) �� ������ �߻��Ͽ����ϴ�\n{e}");
            }
        }

        // ������ �κ��丮 ���� �����Ϳ� UI�� ������Ʈ�ϴ� �Լ�
        public void UpdateItemInventory(int itemID, int count)
        {
            try
            {
                StaticManager.Backend.GameData.ItemInventory.AddItem(itemID, count);
                _uiManager.ItemUI.UpdateUI(itemID, count);
                _uiManager.UserUI.UpdateUI();

                if (count > 0)
                {
                    _uiManager.LeftUI.GetUI<InGameUI_Quest>()
                        .UpdateUIForGetItem(RequestItemType.Item, itemID);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"UpdateItemInventory({itemID}, {count}) �� ������ �߻��Ͽ����ϴ�\n{e}");
            }
        }
        
        // ���� �κ��丮 ���� �����Ϳ� UI�� ������Ʈ�ϴ� �Լ�
        public void UpdateWeaponInventory(int weaponId)
        {
            try
            {
                string myWeaponId = StaticManager.Backend.GameData.WeaponInventory.AddWeapon(weaponId);
                //_uiManager.BottomUI.GetUI<InGameUI_Equip>().AddWeaponObjectInInventoryUI(myWeaponId);
                _uiManager.LeftUI.GetUI<InGameUI_Quest>()
                    .UpdateUIForGetItem(RequestItemType.Weapon, weaponId);
            }
            catch (Exception e)
            {
                throw new Exception($"UpdateWeaponInventory({weaponId}) �� ������ �߻��Ͽ����ϴ�\n{e}");
            }
        }

        
        // ���� ���� ���� �����Ϳ� UI�� ������Ʈ�ϴ� �Լ�
        public void UpdateWeaponEquip(string prevWeaponId, string myWeaponId)
        {
            try
            {
                // �⺻ ����� 
                StaticManager.Backend.GameData.WeaponEquip.ChangeEquip(prevWeaponId, myWeaponId);
                _player.SetWeapon();
                //_uiManager.BottomUI.GetUI<InGameUI_Equip>().UpdateUI(prevWeaponId, myWeaponId);
            }
            catch (Exception e)
            {
                StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name, "UpdateWeaponEquip", $"UpdateWeaponEquip({prevWeaponId}, {myWeaponId}) �� ������ �߻��Ͽ����ϴ�\n" + e.ToString());
            }

        }
    }

}