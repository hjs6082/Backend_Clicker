// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
using BackendData.Chart.Quest;

namespace InGameScene.UI
{
    //===========================================================
    // ����Ʈ UI
    //===========================================================
    public class InGameUI_Quest : InGameUI_LeftUIBase
    {
        [SerializeField] private GameObject _QuestParentObject;
        [SerializeField] private GameObject _questItemPrefab;

        // ����Ʈ ������ ����Ʈ(key���� questId)
        private Dictionary<int, InGameUI_QuestItem> _questItemDic = new();

        // ����Ʈ ������ ������ ����Ʈ(key���� questType)
        private Dictionary<int, List<InGameUI_QuestItem>> _questItemDicByQuestType = new();

        public override void Init()
        {

            // ����Ʈ ��Ʈ�� �ִ� ��� ���� �ҷ��� ����
            foreach (var questItem in StaticManager.Backend.Chart.Quest.Dictionary)
            {
                var newQuestItem = Instantiate(_questItemPrefab, _QuestParentObject.transform, true);
                newQuestItem.transform.localPosition = new Vector3(0, 0, 0);
                newQuestItem.transform.localScale = new Vector3(1, 1, 1);

                var useItem = newQuestItem.GetComponent<InGameUI_QuestItem>();
                useItem.Init(questItem.Value);
                _questItemDic.Add(questItem.Value.QuestID, useItem);

                // �ش� ����Ʈ Ÿ��
                int typeNum = (int)questItem.Value.QuestType;

                // ����Ʈ Ÿ���� ������ ����, ������ ���� ����
                // ����Ʈ Ÿ�Ժ��� List�� ����� ����
                if (!_questItemDicByQuestType.ContainsKey(typeNum))
                {
                    _questItemDicByQuestType.Add(typeNum, new List<InGameUI_QuestItem>());
                }

                _questItemDicByQuestType[typeNum].Add(useItem);
            }

            // ����Ʈ Ÿ�Ժ��� ������Ʈ(��� ���� ����Ʈ�� UserData, ���� ������ WeaponInventory��)
            for (int i = 0; i < Enum.GetValues(typeof(QuestType)).Length; i++)
            {
                UpdateUI((QuestType)i);
            }
        }

        // �� ����Ʈ Ÿ�Ժ� ������Ʈ�ϴ� �Լ�
        public void UpdateUI(QuestType questType)
        {
            switch (questType)
            {
                case QuestType.LevelUp: // ������ ���� ����Ʈ�� ��쿡�� ���� ������ �̿��Ͽ� ������Ʈ
                    foreach (var list in _questItemDicByQuestType[(int)questType])
                    {
                        list.UpdateUI(StaticManager.Backend.GameData.UserData.Level);
                    }
                    break;
                case QuestType.UseGold: // ��� ��� ����Ʈ�� ��쿡�� UsingGold �������� �̿��Ͽ� ������Ʈ
                    foreach (var list in _questItemDicByQuestType[(int)questType])
                    {
                        if (list.GetRepeatType() == QuestRepeatType.Day)
                        {
                            list.UpdateUI(StaticManager.Backend.GameData.UserData.DayUsingGold);
                        }
                        else if (list.GetRepeatType() == QuestRepeatType.Week)
                        {
                            list.UpdateUI(StaticManager.Backend.GameData.UserData.WeekUsingGold);
                        }
                        else if (list.GetRepeatType() == QuestRepeatType.Month)
                        {
                            list.UpdateUI(StaticManager.Backend.GameData.UserData.MonthUsingGold);
                        }
                        else
                        {
                            throw new Exception("Ȯ�ε��� ���� �����Դϴ�.");
                        }
                    }

                    break;
                case QuestType.DefeatEnemy:// �� ó�� ����Ʈ�� DefeatEenmyCount�� ���
                    foreach (var list in _questItemDicByQuestType[(int)questType])
                    {
                        if (list.GetRepeatType() == QuestRepeatType.Day)
                        {
                            list.UpdateUI(StaticManager.Backend.GameData.UserData.DayDefeatEnemyCount);
                        }
                        else if (list.GetRepeatType() == QuestRepeatType.Week)
                        {
                            list.UpdateUI(StaticManager.Backend.GameData.UserData.WeekDefeatEnemyCount);
                        }
                        else if (list.GetRepeatType() == QuestRepeatType.Month)
                        {
                            list.UpdateUI(StaticManager.Backend.GameData.UserData.MonthDefeatEnemyCount);
                        }
                        else
                        {
                            throw new Exception("Ȯ�ε��� ���� �����Դϴ�.");
                        }
                    }

                    break;
                case QuestType.GetItem: // ������ ���� �Լ��� ��쿡�� �������� �����ϴ��� Ȯ��
                    foreach (var list in _questItemDicByQuestType[(int)questType])
                    {
                        list.UpdateUI(0);
                    }

                    // UpdateUIForGetItem���� ��ü
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(questType), questType, null);
            }
        }

        // ������, ������ ��� �� �����ۿ��� �ش� �Լ� ȣ���Ͽ� �������� �����ϴ��� Ȯ���ϰ� �޼� üũ
        public void UpdateUIForGetItem(RequestItemType requestItemType, int itemId)
        {
            foreach (var list in _questItemDicByQuestType[(int)QuestType.GetItem])
            {
                list.CheckItem(requestItemType, itemId);
            }
        }
    }
}