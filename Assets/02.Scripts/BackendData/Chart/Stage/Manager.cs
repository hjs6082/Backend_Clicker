// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using LitJson;
using Unity.VisualScripting;

namespace BackendData.Chart.Stage
{
    //===============================================================
    // Weapon ��Ʈ�� �����͸� �����ϴ� Ŭ����
    //===============================================================
    public class Manager : Base.Chart
    {

        readonly List<Item> _list = new();
        public IReadOnlyList<Item> List => (IReadOnlyList<Item>)_list.AsReadOnlyList();

        // ��Ʈ ���� �̸� ���� �Լ�
        // ��Ʈ �ҷ����⸦ ���������� ó���ϴ� BackendChartDataLoad() �Լ����� �ش� �Լ��� ���� ��Ʈ ���� �̸��� ��´�.
        public override string GetChartFileName()
        {
            return "stageChart";
        }

        // Backend.Chart.GetChartContents���� �� ��Ʈ ���¿� �°� �Ľ��ϴ� Ŭ����
        // ��Ʈ ���� �ҷ����� �Լ��� BackendData.Base.Chart�� BackendChartDataLoad�� �������ּ���
        protected override void LoadChartDataTemplate(JsonData json)
        {
            foreach (JsonData eachItem in json)
            {
                Item info = new Item(eachItem);
                _list.Add(info);
            }
        }
    }
}