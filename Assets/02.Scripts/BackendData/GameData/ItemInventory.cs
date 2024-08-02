// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;


namespace BackendData.GameData
{
    //===============================================================
    // ItemInventory ���̺��� �����͸� ����ϴ� Ŭ����
    //===============================================================
    public class ItemInventory : Base.GameData
    {

        // key�� int�� itemID, value�� int�� ������ ���� 
        private Dictionary<int, int> _dictionary = new();
        public IReadOnlyDictionary<int, int> Dictionary => (IReadOnlyDictionary<int, int>)_dictionary.AsReadOnlyCollection();

        // �����Ͱ� �������� ���� ���, �ʱⰪ ����
        protected override void InitializeData()
        {
            _dictionary.Clear();
        }

        // Backend.GameData.GetMyData ȣ�� ���� ���ϵ� ���� �Ľ��Ͽ� ĳ���ϴ� �Լ�
        // �������� �����͸� �ҷ����e �Լ��� BackendData.Base.GameData�� BackendGameDataLoad() �Լ��� �������ּ���
        protected override void SetServerDataToLocal(JsonData gameDataJson)
        {

            var keys = gameDataJson.Keys;
            foreach (var key in keys)
            {

                _dictionary.Add(int.Parse(key), int.Parse(gameDataJson[key].ToString()));
            }
        }

        // ���̺� �̸� ���� �Լ�
        public override string GetTableName()
        {
            return "ItemInventory";
        }

        // �÷� �̸� ���� �Լ�
        public override string GetColumnName()
        {
            return "ItemInventory";
        }

        // ������ ���� �� ������ �����͸� �ڳ��� �°� �Ľ��ϴ� �Լ�
        public override Param GetParam()
        {
            Param param = new Param();

            param.Add(GetColumnName(), _dictionary);
            return param;
        }

        // �������� �߰��ϴ� �Լ�
        public void AddItem(int itemID, int count)
        {
            IsChangedData = true;
            if (_dictionary.ContainsKey(itemID))
            {
                _dictionary[itemID] += count;
            }
            else
            {
                _dictionary.Add(itemID, count);
            }
        }

    }
}
