// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using BackEnd;
using LitJson;
using Unity.VisualScripting;

namespace BackendData.GameData.WeaponInventory
{

    //===============================================================
    // WeaponInventory ���̺��� �����͸� �����ϴ� Ŭ����
    //===============================================================
    public class Manager : Base.GameData
    {

        // WeaponInventory�� �� �������� ��� Dictionary
        private Dictionary<string, Item> _dictionary = new();

        // �ٸ� Ŭ�������� Add, Delete�� ������ �Ұ����ϵ��� �б� ���� Dictionary
        public IReadOnlyDictionary<string, Item> Dictionary =>
            (IReadOnlyDictionary<string, Item>)_dictionary.AsReadOnlyCollection();

        // ���̺� �̸� ���� �Լ�
        public override string GetTableName()
        {
            return "WeaponInventory";
        }

        // �÷� �̸� ���� �Լ�
        public override string GetColumnName()
        {
            return "WeaponInventory";
        }

        // �����Ͱ� �������� ���� ���, �ʱⰪ ����
        protected override void InitializeData()
        {
            _dictionary.Clear();
            AddWeapon(1);
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
                int weaponLevel = int.Parse(gameDataJson[i]["WeaponLevel"].ToString());
                string myWeaponId = gameDataJson[i]["MyWeaponId"].ToString();
                int weaponChartId = int.Parse(gameDataJson[i]["WeaponChartId"].ToString());

                var weaponInfo = StaticManager.Backend.Chart.Weapon.Dictionary[weaponChartId];

                _dictionary.Add(myWeaponId,
                    new Item(myWeaponId, weaponLevel, weaponInfo));
            }
        }

        // �κ��丮�� ���� �߰�
        public string AddWeapon(int weaponId)
        {
            IsChangedData = true;

            // ��Ʈ���� ���� ���� �˻�
            var weaponInfo = StaticManager.Backend.Chart.Weapon.Dictionary[weaponId];

            // ������ ���� ���̵� ����� ���� UnixTime�� ����(����ð��� ���ڷ� �ٲ� ������)
            // 0.00001���� �������� �����͸� �����ؾ߸� �ߺ��� �ǹǷ�, ���� �ϳ��� �����ͷδ� ����ũ id�� �����ϴ�.
            DateTime now = DateTime.Now;
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            string myWeaponID = Convert.ToString(Convert.ToInt64((now - epoch).TotalMilliseconds));

            _dictionary.Add(myWeaponID, new Item(myWeaponID, 1, weaponInfo));

            return myWeaponID;
        }

    }
}