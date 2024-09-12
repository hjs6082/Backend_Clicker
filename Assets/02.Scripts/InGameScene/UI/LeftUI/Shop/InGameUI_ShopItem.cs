using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RewardType
{
    gold,
    gem
}

namespace InGameScene.UI
{
    public class InGameUI_ShopItem : MonoBehaviour
    {
        [SerializeField]
        private RewardType rewardType;

        [SerializeField]
        private int price;
        [SerializeField]
        private int reward;

        [SerializeField]
        private Button buyButton;

        private InGameUI_Shop shop; // Shop�� ������ ����

        public void Init()
        {
            // ���� ��ư Ŭ�� �̺�Ʈ ����
            buyButton.onClick.AddListener(OnBuyButtonClicked);
        }

        // Shop ����
        public void SetShop(InGameUI_Shop shop)
        {
            this.shop = shop;
        }

        // ���� ��ư Ŭ�� �� ȣ��
        private void OnBuyButtonClicked()
        {
            // Shop�� ������ �����ϰ� �˾��� ���
            shop.ShowBuyPopup(price, reward, rewardType);
        }
    }
}
