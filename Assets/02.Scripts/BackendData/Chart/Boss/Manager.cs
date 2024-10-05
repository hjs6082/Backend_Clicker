// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.Chart.Boss
{
    //===============================================================
    // Boss ��Ʈ�� �����͸� �����ϴ� Ŭ����
    //===============================================================
    public class Manager : Base.Chart
    {

        // �� ��Ʈ�� row ������ ��� Dictionary
        private readonly Dictionary<int, Item> _dictionary = new();

        // �ٸ� Ŭ�������� Add, Delete�� ������ �Ұ����ϵ��� �б� ���� Dictionary
        public IReadOnlyDictionary<int, Item> Dictionary => (IReadOnlyDictionary<int, Item>)_dictionary.AsReadOnlyCollection();

        // ���� �̹��� ĳ���� �����ϴ� Dictionary
        private readonly Dictionary<string, Sprite> _enemyImages = new();

        // ��Ʈ ���� �̸� ���� �Լ�
        // ��Ʈ �ҷ����⸦ ���������� ó���ϴ� BackendChartDataLoad() �Լ����� �ش� �Լ��� ���� ��Ʈ ���� �̸��� ��´�.
        public override string GetChartFileName()
        {
            return "bossChart";
        }

        // Backend.Chart.GetChartContents���� �� ��Ʈ ���¿� �°� �Ľ��ϴ� Ŭ����
        // ��Ʈ ���� �ҷ����� �Լ��� BackendData.Base.Chart�� BackendChartDataLoad�� �������ּ���
        protected override void LoadChartDataTemplate(JsonData json)
        {
            foreach (JsonData eachItem in json)
            {
                Item info = new Item(eachItem);

                _dictionary.Add(info.BossID, info);
                string prefabPath = $"Prefabs/Monster/Boss/{info.Image}";
                GameObject enemyPrefab = Resources.Load<GameObject>(prefabPath);

                if (enemyPrefab == null)
                {
                    Debug.LogError($"Prefab not found for enemy: {info.BossName} at path {prefabPath}");
                }
            }
        }
    }
}