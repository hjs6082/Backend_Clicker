using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InGameScene.UI
{
    public class InGameUI_ShopPopupItem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text popupMessage; // 팝업 내 메시지를 표시할 텍스트

        [SerializeField]
        private Button confirmButton; // 팝업의 확인 버튼
        [SerializeField]
        private Button cancelButton; // 팝업의 취소 버튼

        private int currentPrice;
        private int currentReward;
        private RewardType currentRewardType;

        public void SetItem(int price, int reward, RewardType rewardType)
        {
            currentPrice = price;
            currentReward = reward;
            currentRewardType = rewardType;

            // 팝업 메시지 설정
            popupMessage.text = $"{rewardType} {reward}을(를) 구매하시겠습니까? \n가격: {price} Gem";
        }

        // 팝업 버튼 이벤트 초기화
        public void Init(System.Action<int, int, RewardType> onConfirm, System.Action onCancel)
        {
            // 확인 버튼에 구매 확정 로직 추가
            confirmButton.onClick.AddListener(() => onConfirm?.Invoke(currentPrice, currentReward, currentRewardType));
            cancelButton.onClick.AddListener(() => onCancel?.Invoke());
        }

        // 팝업을 활성화
        public void Show()
        {
            gameObject.SetActive(true);
        }

        // 팝업을 비활성화
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
