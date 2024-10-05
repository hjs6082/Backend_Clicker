// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace BackendData.Chart.Boss
{
    //===============================================================
    // Boss 차트의 각 row 데이터 클래스
    //===============================================================
    public class Item
    {
        public class DropItem
        {
            public int ItemID { get; private set; }

            public float DropPercent { get; private set; }

            public DropItem(int itemID, float dropPercent)
            {
                ItemID = itemID;
                DropPercent = dropPercent;
            }
        }

        public int BossID { get; private set; }
        public string BossName { get; private set; }

        public float Hp { get; private set; }
        public int Lv { get; private set; }
        public float Exp { get; private set; }
        public float Money { get; private set; }
        public List<DropItem> DropItemList { get; private set; }
        public string Image { get; private set; }

        public Item(JsonData json)
        {
            BossID = int.Parse(json["BossID"].ToString());
            BossName = json["BossName"].ToString();
            Hp = float.Parse(json["Hp"].ToString());
            Lv = int.Parse(json["Lv"].ToString());
            Exp = float.Parse(json["Exp"].ToString());
            Money = float.Parse(json["Money"].ToString());
            Image = json["Image"].ToString();

            DropItemList = new List<DropItem>();

            string dropItemListString = json["DropItemList"].ToString();

            if (string.IsNullOrEmpty(dropItemListString) || dropItemListString == "null")
            {
                return;
            }

            JsonData dropItemListJson = JsonMapper.ToObject(dropItemListString);

            foreach (JsonData item in dropItemListJson)
            {
                int itemID = int.Parse(item["id"].ToString());
                float percent = float.Parse(item["percent"].ToString());
                DropItemList.Add(new DropItem(itemID, percent));
            }
        }
    }
}