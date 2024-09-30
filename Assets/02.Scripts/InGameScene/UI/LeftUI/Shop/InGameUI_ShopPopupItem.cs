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
        private TMP_Text popupMessage; // �˾� �� �޽����� ǥ���� �ؽ�Ʈ

        [SerializeField]
        private Button confirmButton; // �˾��� Ȯ�� ��ư
        [SerializeField]
        private Button cancelButton; // �˾��� ��� ��ư

        private int currentPrice;
        private int currentReward;
        private RewardType currentRewardType;

        public void SetItem(int price, int reward, RewardType rewardType)
        {
            currentPrice = price;
            currentReward = reward;
            currentRewardType = rewardType;

            // �˾� �޽��� ����
            popupMessage.text = $"{rewardType} {reward}��(��) �����Ͻðڽ��ϱ�? \n����: {price} Gem";
        }

        // �˾� ��ư �̺�Ʈ �ʱ�ȭ
        public void Init(System.Action<int, int, RewardType> onConfirm, System.Action onCancel)
        {
            // Ȯ�� ��ư�� ���� Ȯ�� ���� �߰�
            confirmButton.onClick.AddListener(() => onConfirm?.Invoke(currentPrice, currentReward, currentRewardType));
            cancelButton.onClick.AddListener(() => onCancel?.Invoke());
        }

        // �˾��� Ȱ��ȭ
        public void Show()
        {
            gameObject.SetActive(true);
        }

        // �˾��� ��Ȱ��ȭ
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
