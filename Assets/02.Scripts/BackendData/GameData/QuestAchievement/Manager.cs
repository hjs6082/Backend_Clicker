// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;


namespace BackendData.GameData.QuestAchievement
{

    //===============================================================
    // WeaponInventory ���̺��� �����͸� �����ϴ� Ŭ����
    //===============================================================
    public class Manager : Base.GameData
    {

        // QuestAchievement�� �� �������� ��� Dictionary
        private Dictionary<int, Item> _dictionary = new();

        // �ٸ� Ŭ�������� Add, Delete�� ������ �Ұ����ϵ��� �б� ���� Dictionary
        public IReadOnlyDictionary<int, Item> Dictionary =>
            (IReadOnlyDictionary<int, Item>)_dictionary.AsReadOnlyCollection();

        // ���̺� �̸� ���� �Լ�
        public override string GetTableName()
        {
            return "QuestAchievement";
        }

        // �÷� �̸� ���� �Լ�
        public override string GetColumnName()
        {
            return "QuestAchievement";
        }

        // �����Ͱ� �������� ���� ���, �ʱⰪ ����
        protected override void InitializeData()
        {
            foreach (var chartData in StaticManager.Backend.Chart.Quest.Dictionary)
            {
                int questId = chartData.Value.QuestID;
                _dictionary.Add(questId, new Item(questId, false));
            }
        }

        // ������ ���� �� ������ �����͸� �ڳ��� �°� �Ľ��ϴ� �Լ�
        // Dictionary �ϳ��� ����
        public override Param GetParam()
        {
            Param param = new Param();
            param.Add(GetColumnName(), _dictionary);

            return param;
        }

        // Backend.GameData.GetMyData ȣ�� ���� ���ϵ� ���� �Ľ��Ͽ� ĳ���ϴ� �Լ�
        // �������� �����͸� �ҷ����e �Լ��� BackendData.Base.GameData�� BackendGameDataLoad() �Լ��� �������ּ���
        protected override void SetServerDataToLocal(JsonData gameDataJson)
        {
            for (int i = 0; i < gameDataJson.Count; i++)
            {
                int questId = int.Parse(gameDataJson[i]["QuestId"].ToString());
                bool isQuestAchieve = Boolean.Parse(gameDataJson[i]["IsAchieve"].ToString());

                _dictionary.Add(questId, new Item(questId, isQuestAchieve));
            }
        }

        // Ư�� ����Ʈ�� �޼�ó���ϴ� �Լ�
        public void SetAchieve(int questId)
        {
            IsChangedData = true;
            _dictionary[questId].SetAchieve();
        }
    }
}