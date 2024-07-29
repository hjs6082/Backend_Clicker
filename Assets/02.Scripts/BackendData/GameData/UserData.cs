// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using BackEnd;
using LitJson;
using UnityEngine;

namespace BackendData.GameData
{
    //===============================================================
    // UserData ���̺��� �����͸� ����ϴ� Ŭ����(����)
    //===============================================================
    public partial class UserData
    {
        public int Level { get; private set; }
        public float Money { get; private set; }
        public string LastLoginTime { get; private set; }

        public float Exp { get; private set; }

        public float MaxExp { get; private set; }
        public float Jewel { get; private set; }

        public float DayUsingGold { get; set; }
        public float WeekUsingGold { get; set; }
        public float MonthUsingGold { get; set; }

        public int DayDefeatEnemyCount { get; private set; }
        public int WeekDefeatEnemyCount { get; private set; }
        public int MonthDefeatEnemyCount { get; private set; }
    }

    //===============================================================
    // UserData ���̺��� �����͸� ����ϴ� Ŭ����(�Լ�)
    //===============================================================
    public partial class UserData : Base.GameData
    {

        // �����Ͱ� �������� ���� ���, �ʱⰪ ����
        protected override void InitializeData()
        {
            Level = 1;
            Money = 10000;
            MaxExp = 100;
        }

        // Backend.GameData.GetMyData ȣ�� ���� ���ϵ� ���� �Ľ��Ͽ� ĳ���ϴ� �Լ�
        // �������� �����͸� �ҷ����e �Լ��� BackendData.Base.GameData�� BackendGameDataLoad() �Լ��� �������ּ���
        protected override void SetServerDataToLocal(JsonData gameDataJson)
        {
            Level = int.Parse(gameDataJson["Level"].ToString());
            Exp = float.Parse(gameDataJson["Exp"].ToString());
            MaxExp = float.Parse(gameDataJson["MaxExp"].ToString());
            Money = float.Parse(gameDataJson["Money"].ToString());
            LastLoginTime = gameDataJson["LastLoginTime"].ToString();

            DayUsingGold = float.Parse(gameDataJson["DayUsingGold"].ToString());
            WeekUsingGold = float.Parse(gameDataJson["WeekUsingGold"].ToString());
            MonthUsingGold = float.Parse(gameDataJson["MonthUsingGold"].ToString());

            DayDefeatEnemyCount = int.Parse(gameDataJson["DayDefeatEnemyCount"].ToString());
            WeekDefeatEnemyCount = int.Parse(gameDataJson["WeekDefeatEnemyCount"].ToString());
            MonthDefeatEnemyCount = int.Parse(gameDataJson["MonthDefeatEnemyCount"].ToString());

            Jewel = gameDataJson.ContainsKey("Jewel") ? float.Parse(gameDataJson["Jewel"].ToString()) : 0;
        }

        // ���̺� �̸� ���� �Լ�
        public override string GetTableName()
        {
            return "UserData";
        }

        // �÷� �̸� ���� �Լ�
        public override string GetColumnName()
        {
            return null;
        }

        // ������ ���� �� ������ �����͸� �ڳ��� �°� �Ľ��ϴ� �Լ�
        public override Param GetParam()
        {
            Param param = new Param();

            param.Add("Level", Level);
            param.Add("Money", Money);
            param.Add("Exp", Exp);
            param.Add("MaxExp", MaxExp);
            param.Add("LastLoginTime", string.Format("{0:MM-DD:HH:mm:ss.fffZ}", DateTime.Now.ToString(CultureInfo.InvariantCulture)));

            param.Add("DayUsingGold", DayUsingGold);
            param.Add("WeekUsingGold", WeekUsingGold);
            param.Add("MonthUsingGold", MonthUsingGold);

            param.Add("DayDefeatEnemyCount", DayDefeatEnemyCount);
            param.Add("WeekDefeatEnemyCount", WeekDefeatEnemyCount);
            param.Add("MonthDefeatEnemyCount", MonthDefeatEnemyCount);

            return param;
        }

        // �� óġ Ƚ���� �����ϴ� �Լ�
        public void CountDefeatEnemy()
        {
            DayDefeatEnemyCount++;
            WeekDefeatEnemyCount++;
            MonthDefeatEnemyCount++;
        }

        // ������ ������ �����ϴ� �Լ�
        public void UpdateUserData(float money, float exp)
        {
            IsChangedData = true;

            Exp += exp;
            Money += money;

            if (money < 0)
            {
                float tempMoney = Math.Abs(money);
                DayUsingGold += tempMoney;
                WeekUsingGold += tempMoney;
                MonthUsingGold += tempMoney;
            }

            if (Exp > MaxExp)
            {
                while (Exp > MaxExp)
                {
                    LevelUp();
                }
            }
        }

        // �������ϴ� �Լ�
        private void LevelUp()
        {
            //Exp�� MaxExp�� �ʰ����� ��츦 ����Ͽ� ����
            Exp -= MaxExp;

            //���� ����ġ���� 1.1��
            MaxExp = (float)Math.Truncate(MaxExp * 1.1);

            Level++;
        }
    }
}