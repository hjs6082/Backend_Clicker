// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;

namespace BackendData.GameData.WeaponEquip
{

    //===============================================================
    // WeaponEquip ���̺��� �����͸� �����ϴ� Ŭ����
    //===============================================================
    public class Manager : Base.GameData
    {

        // WeaponEquip �� �������� ��� Dictionary
        private Dictionary<string, int> _dictionary = new();

        // �ٸ� Ŭ�������� Add, Delete�� ������ �Ұ����ϵ��� �б� ���� Dictionary
        public IReadOnlyDictionary<string, int> Dictionary => (IReadOnlyDictionary<string, int>)_dictionary.AsReadOnlyCollection();

        // ���̺� �̸� ���� �Լ�
        public override string GetTableName()
        {
            return "WeaponEquip";
        }

        // �÷� �̸� ���� �Լ�
        public override string GetColumnName()
        {
            return "WeaponEquip";
        }

        // �����Ͱ� �������� ���� ���, �ʱⰪ ����
        protected override void InitializeData()
        {
            _dictionary.Clear();
            foreach (var weaponDic in StaticManager.Backend.GameData.WeaponInventory.Dictionary)
            {
                _dictionary.Add(weaponDic.Key, 0);
            }
        }

        // ������ ���� �� ������ �����͸� �ڳ��� �°� �Ľ��ϴ� �Լ�
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

            var keys = gameDataJson.Keys;

            foreach (var key in keys)
            {
                int position = int.Parse(gameDataJson[key].ToString());
                string myWeaponId = key;

                _dictionary.Add(myWeaponId, position);
            }
        }

        // ������ ���⿡ ���� ������ �����ϴ� �Լ�
        public void ChangeEquip(string prevWeaponId, string myWeaponId)
        {
            IsChangedData = true;
            if (string.IsNullOrEmpty(prevWeaponId) == false)
            {
                _dictionary.Remove(prevWeaponId);
            }
            if (_dictionary.ContainsKey(myWeaponId))
            {
                _dictionary.Remove(myWeaponId);
            }
            _dictionary.Add(myWeaponId, 0);
        }
    }
}
