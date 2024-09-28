using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace InGameScene.UI
{
    public class InGameUI_Shop : InGameUI_LeftUIBase
    {
        [SerializeField]
        private InGameUI_ShopPopupItem buyPopup; // InGameUI_ShopPopupItem을 사용

        [SerializeField]
        private List<InGameUI_ShopItem> shopItemList;

        public override void Init()
        {
            base.Init();

            shopItemList.AddRange(GetComponentsInChildren<InGameUI_ShopItem>());

            foreach (var item in shopItemList)
            {
                item.SetShop(this); // 각 아이템에 Shop 연결
                item.Init();
            }

            // 팝업의 버튼 이벤트 초기화
            buyPopup.Init(OnConfirmPurchase, OnCancelPurchase);
        }

        public override void Open()
        {
            base.Open();
        }

        // 팝업 띄우기
        public void ShowBuyPopup(int price, int reward, RewardType rewardType)
        {
            buyPopup.SetItem(price, reward, rewardType); // 팝업의 아이템 설정
            buyPopup.Show(); // 팝업을 화면에 표시
        }

        // 구매 확정 처리
        private void OnConfirmPurchase(int price, int reward, RewardType rewardType)
        {
            if (StaticManager.Backend.GameData.UserData.Gem < price)
            {
                StaticManager.UI.AlertUI.OpenWarningUI("구매 불가", "현재 젬이 부족하여 해당 아이템을 구매할 수 없습니다");
                OnCancelPurchase();
                return;
            }

            InGameScene.Managers.Game.UpdateGem(-price);
            AddReward(rewardType, reward);
            UpdateUI();

            // 팝업 비활성화
            buyPopup.Hide();
        }

        // 보상 처리 로직
        private void AddReward(RewardType rewardType, int reward)
        {
            // 보상 타입에 따라 처리
            switch (rewardType)
            {
                case RewardType.gold:
                    InGameScene.Managers.Game.UpdateUserData(reward, 0);
                    StaticManager.UI.AlertUI.OpenAlertUI("구매 완료", rewardType + reward + "이(가) 지급되었습니다.");
                    UpdateUI();
                    break;
                case RewardType.gem:
                    // 다른 보상 처리 (예: 보석 추가)
                    //Debug.Log($"플레이어에게 {reward} 보석 지급.");
                    break;
            }
        }

        // 구매 취소 처리
        private void OnCancelPurchase()
        {
            // 팝업 비활성화
            buyPopup.Hide();
        }

        public void OnFailedPurchase()
        {
            StaticManager.UI.AlertUI.OpenAlertUI("구매 취소", "구매가 취소되었습니다.");
        }

        public void OnCompletePurchase(int gem)
        {
            InGameScene.Managers.Game.UpdateGem(gem);
            StaticManager.UI.AlertUI.OpenAlertUI("구매 완료", "Gem " + gem + "이(가) 지급되었습니다.");
            UpdateUI();
        }
    }
}
