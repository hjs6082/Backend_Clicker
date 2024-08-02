// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BackendData.Chart.Quest;

namespace InGameScene.UI
{
    //===========================================================
    // ����Ʈ ������ UI
    //===========================================================
    public class InGameUI_QuestItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _questReqeatTypeText;
        [SerializeField] private TMP_Text _questContentText;
        [SerializeField] private TMP_Text _questRewardText;
        [SerializeField] private TMP_Text _questRequestText;
        [SerializeField] private TMP_Text _myRequestAchieveText;

        [SerializeField] private Button _requestAchieveButton;
        [SerializeField] private TMP_Text _isAchieveText;

        private Item _questItemInfo;

        // ����Ʈ Ÿ�� �����ϴ� �Լ�
        public QuestRepeatType GetRepeatType()
        {
            return _questItemInfo.QuestRepeatType;
        }

        // ����Ʈ ���� ����Ʈ
        private List<string> _rewardList = new();

        public void Init(Item questItemInfo)
        {
            _questItemInfo = questItemInfo;

            switch (_questItemInfo.QuestRepeatType)
            {
                case QuestRepeatType.Day:
                    _questReqeatTypeText.text = "����";
                    break;
                case QuestRepeatType.Week:
                    _questReqeatTypeText.text = "�ְ�";
                    break;
                case QuestRepeatType.Month:
                    _questReqeatTypeText.text = "����";
                    break;
                case QuestRepeatType.Once:
                    _questReqeatTypeText.text = "����";
                    break;
            }

            _questContentText.text = _questItemInfo.QuestContent;

            // Exp, Money�� �ִ� RewardStat�� ������ ���, ������ �˷��ִ� text�� ����
            if (_questItemInfo.RewardStat != null)
            {
                foreach (var item in _questItemInfo.RewardStat)
                {
                    // exp�� ������ ���
                    if (item.Exp > 0)
                    {
                        _rewardList.Add($"{item.Exp} Exp");
                    }
                    // money�� ������ ���
                    if (item.Money > 0)
                    {
                        _rewardList.Add($"{item.Money} Gold");
                    }
                }
            }

            // ������, ���⸦ �ִ� RewardItem�� ������ ���, ������ �˷��ִ� text�� ����
            if (_questItemInfo.RewardItem != null)
            {
                foreach (var item in _questItemInfo.RewardItem)
                {
                    switch (item.RewardItemType)
                    {
                        case RewardItemType.Item: // ������ �������� ��� ������ �̸�
                            _rewardList.Add(StaticManager.Backend.Chart.Item.Dictionary[item.Id].ItemName);
                            break;
                        case RewardItemType.Weapon:// ������ ������ ��� ���� �̸�
                            _rewardList.Add(StaticManager.Backend.Chart.Weapon.Dictionary[item.Id].WeaponName);
                            break;
                    }
                }
            }


            // ������ ���� list ���� ���ٷ� ǥ��
            StringBuilder rewardString = new StringBuilder();
            for (int i = 0; i < _rewardList.Count; i++)
            {
                if (i > 0)
                {
                    rewardString.Append(" | ");
                }

                rewardString.Append(_rewardList[i]);
            }

            _questRewardText.text = rewardString.ToString();
            _questRequestText.text = _questItemInfo.RequestCount.ToString();
            _myRequestAchieveText.text = 0.ToString();

            //���� �޼� �� �����ϴ� Achieve �Լ� ����
            _requestAchieveButton.onClick.AddListener(Achieve);
        }


        // ����Ʈ ��Ʈ�� �ִ� ���� Ƚ���� �Ѿ����� Ȯ��.
        public void UpdateUI(float count)
        {
            bool isAchieve = StaticManager.Backend.GameData.QuestAchievement.Dictionary[_questItemInfo.QuestID]
                .IsAchieve;
            if (isAchieve)
            { // �̹� �޼��� ���¶��
                _isAchieveText.text = "�Ϸ�";
                _requestAchieveButton.onClick.RemoveAllListeners();
                _requestAchieveButton.interactable = false;
                _requestAchieveButton.GetComponent<Image>().color = Color.gray;
            }
            else
            { // �޼��� �Ǿ��ٸ�
                if (_questItemInfo.RequestCount <= count)
                {
                    _isAchieveText.text = "�޼�";
                    _requestAchieveButton.interactable = true;
                    _requestAchieveButton.GetComponent<Image>().color = new Color32(255, 236, 144, 255);
                }
                else
                { // ���� count�� �����ϴٸ�
                    _isAchieveText.text = "�̴޼�";
                    _requestAchieveButton.interactable = false;
                    _requestAchieveButton.GetComponent<Image>().color = Color.gray;
                }
            }

            // ���� �������� Ƚ��
            _myRequestAchieveText.text = count.ToString();
        }


        // ����Ʈ �޼� ��ư Ŭ���� ȣ��Ǵ� �Լ�
        public void Achieve()
        {
            // ���� �����Ϳ��� ����Ʈ �޼� ���θ� ����
            StaticManager.Backend.GameData.QuestAchievement.SetAchieve(_questItemInfo.QuestID);

            // ���� ����
            Reward();

            // ����Ʈ UI�� �Ϸ�� �����ϰ� ��ư ����
            _isAchieveText.text = "�Ϸ�";
            _requestAchieveButton.onClick.RemoveAllListeners();
            _requestAchieveButton.interactable = false;
            _requestAchieveButton.GetComponent<Image>().color = Color.gray;
        }

        // �������� �ִ��� Ȯ��
        public void CheckItem(RequestItemType requestItemType, int itemId)
        {

            if (_questItemInfo.RequestItem == null)
            {
                throw new Exception("RequestItem�� ����ֽ��ϴ�.");
            }

            // itemID�� �������� ���� �ֱ� ������ weapon�� item�� �� �����ؾ��Ѵ�.
            // ������ ������ �ٸ��� �н�
            if (requestItemType != _questItemInfo.RequestItem.RequestItemType)
            {
                return;
            }

            // ������Ʈ�� �������� �����ϸ� ������Ʈ
            if (itemId == _questItemInfo.RequestItem.Id)
            {
                UpdateUI(1);
            }
        }

        // �� ���� ������ ���� ������ �����ϴ� �Լ�
        private void Reward()
        {
            if (_questItemInfo.RewardStat != null)
            {
                // ��Ʈ ������  money, exp�� �ִ� rewardStat�� �����Ѵٸ�
                foreach (var item in _questItemInfo.RewardStat)
                {
                    InGameScene.Managers.Game.UpdateUserData(item.Money, item.Exp);
                }
            }

            // ��Ʈ ������ ������, ���⸦ �ִ� RewardItem�� �����Ѵٸ�
            if (_questItemInfo.RewardItem != null)
            {
                foreach (var item in _questItemInfo.RewardItem)
                {
                    switch (item.RewardItemType)
                    {
                        case RewardItemType.Item: // �������� ��� �������� id�� ������ ������Ʈ
                            InGameScene.Managers.Game.UpdateItemInventory(item.Id, (int)item.Count);

                            break;
                        case RewardItemType.Weapon: // ������ ���, ������ id�� ������ ������Ʈ
                            InGameScene.Managers.Game.UpdateWeaponInventory(item.Id);
                            break;
                    }
                }
            }

            // ���� ������ UI�� ǥ��
            StringBuilder rewardString = new StringBuilder();
            rewardString.Append("���� ������ ȹ���߽��ϴ�.\n");
            for (int i = 0; i < _rewardList.Count; i++)
            {
                if (i > 0)
                {
                    rewardString.Append("\n");
                }

                rewardString.Append(_rewardList[i]);
            }

            StaticManager.UI.AlertUI.OpenAlertUI("����Ʈ �Ϸ�", rewardString.ToString());
        }
    }
}