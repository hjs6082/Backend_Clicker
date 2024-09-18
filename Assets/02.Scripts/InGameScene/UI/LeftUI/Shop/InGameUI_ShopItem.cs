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

        private InGameUI_Shop shop; // Shop에 접근할 변수

        public void Init()
        {
            // 구매 버튼 클릭 이벤트 설정
            buyButton.onClick.AddListener(OnBuyButtonClicked);
        }

        // Shop 설정
        public void SetShop(InGameUI_Shop shop)
        {
            this.shop = shop;
        }

        // 구매 버튼 클릭 시 호출
        private void OnBuyButtonClicked()
        {
            // Shop에 정보를 전달하고 팝업을 띄움
            shop.ShowBuyPopup(price, reward, rewardType);
        }
    }
}
