// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using LitJson;
using Unity.VisualScripting;

namespace BackendData.Chart.Quest
{
    //===============================================================
    // Quest ��Ʈ�� �����͸� �����ϴ� Ŭ����
    //===============================================================
    public class Manager : Base.Chart
    {

        // �� ��Ʈ�� Item(row) ������ ��� Dictionary
        readonly Dictionary<int, Item> _dictionary = new();

        // �ٸ� Ŭ�������� Add, Delete�� ������ �Ұ����ϵ��� �б� ���� Dictionary
        public IReadOnlyDictionary<int, Item> Dictionary => (IReadOnlyDictionary<int, Item>)_dictionary.AsReadOnlyCollection();


        // ��Ʈ ���� �̸� ���� �Լ�
        // ��Ʈ �ҷ����⸦ ���������� ó���ϴ� BackendChartDataLoad() �Լ����� �ش� �Լ��� ���� ��Ʈ ���� �̸��� ��´�.
        public override string GetChartFileName()
        {
            return "questChart";
        }

        // Backend.Chart.GetChartContents���� �� ��Ʈ ���¿� �°� �Ľ��ϴ� Ŭ����
        // ��Ʈ ���� �ҷ����� �Լ��� BackendData.Base.Chart�� BackendChartDataLoad�� �������ּ���
        protected override void LoadChartDataTemplate(JsonData json)
        {
            foreach (JsonData eachItem in json)
            {
                Item info = new Item(eachItem);
                _dictionary.Add(info.QuestID, info);
            }
        }
    }
}