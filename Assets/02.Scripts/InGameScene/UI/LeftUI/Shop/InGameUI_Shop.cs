using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace InGameScene.UI
{
    public class InGameUI_Shop : InGameUI_LeftUIBase
    {
        [SerializeField]
        private InGameUI_ShopPopupItem buyPopup; // InGameUI_ShopPopupItem�� ���

        [SerializeField]
        private List<InGameUI_ShopItem> shopItemList;

        public override void Init()
        {
            base.Init();

            shopItemList.AddRange(GetComponentsInChildren<InGameUI_ShopItem>());

            foreach (var item in shopItemList)
            {
                item.SetShop(this); // �� �����ۿ� Shop ����
                item.Init();
            }

            // �˾��� ��ư �̺�Ʈ �ʱ�ȭ
            buyPopup.Init(OnConfirmPurchase, OnCancelPurchase);
        }

        public override void Open()
        {
            base.Open();
        }

        // �˾� ����
        public void ShowBuyPopup(int price, int reward, RewardType rewardType)
        {
            buyPopup.SetItem(price, reward, rewardType); // �˾��� ������ ����
            buyPopup.Show(); // �˾��� ȭ�鿡 ǥ��
        }

        // ���� Ȯ�� ó��
        private void OnConfirmPurchase(int price, int reward, RewardType rewardType)
        {
            if (StaticManager.Backend.GameData.UserData.Gem < price)
            {
                StaticManager.UI.AlertUI.OpenWarningUI("���� �Ұ�", "���� ���� �����Ͽ� �ش� �������� ������ �� �����ϴ�");
                OnCancelPurchase();
                return;
            }

            InGameScene.Managers.Game.UpdateGem(-price);
            AddReward(rewardType, reward);
            UpdateUI();

            // �˾� ��Ȱ��ȭ
            buyPopup.Hide();
        }

        // ���� ó�� ����
        private void AddReward(RewardType rewardType, int reward)
        {
            // ���� Ÿ�Կ� ���� ó��
            switch (rewardType)
            {
                case RewardType.gold:
                    InGameScene.Managers.Game.UpdateUserData(reward, 0);
                    StaticManager.UI.AlertUI.OpenAlertUI("���� �Ϸ�", rewardType + reward + "��(��) ���޵Ǿ����ϴ�.");
                    UpdateUI();
                    break;
                case RewardType.gem:
                    // �ٸ� ���� ó�� (��: ���� �߰�)
                    //Debug.Log($"�÷��̾�� {reward} ���� ����.");
                    break;
            }
        }

        // ���� ��� ó��
        private void OnCancelPurchase()
        {
            // �˾� ��Ȱ��ȭ
            buyPopup.Hide();
        }

        public void OnFailedPurchase()
        {
            StaticManager.UI.AlertUI.OpenAlertUI("���� ���", "���Ű� ��ҵǾ����ϴ�.");
        }

        public void OnCompletePurchase(int gem)
        {
            InGameScene.Managers.Game.UpdateGem(gem);
            StaticManager.UI.AlertUI.OpenAlertUI("���� �Ϸ�", "Gem " + gem + "��(��) ���޵Ǿ����ϴ�.");
            UpdateUI();
        }
    }
}
