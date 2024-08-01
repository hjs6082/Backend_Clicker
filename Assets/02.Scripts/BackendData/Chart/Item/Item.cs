// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace BackendData.Chart.Item
{
    //===============================================================
    // Item ��Ʈ�� �� row ������ Ŭ����
    //===============================================================
    public class Item
    {
        public int ItemID { get; private set; }
        public string ItemName { get; private set; }
        public string ItemType { get; private set; }
        public string ItemContent { get; private set; }

        public string ImageName { get; private set; }

        public Sprite ImageSprite { get; set; }

        public Dictionary<string, float> ItemStat { get; private set; }

        public Item(JsonData json)
        {
            ItemID = int.Parse(json["ItemID"].ToString());
            ItemName = json["ItemName"].ToString();
            ItemType = json["ItemType"].ToString();
            ItemContent = json["ItemContent"].ToString();
            ImageName = json["ImageName"].ToString();

            ItemStat = new Dictionary<string, float>();
            string itemStatString = json["ItemStat"].ToString();

            if (string.IsNullOrEmpty(itemStatString) || itemStatString == "null")
            {
                return;
            }

            // string�� {"Delay":"0.5","Time":"60"} �� ���� ���
            JsonData dropItemListJson = JsonMapper.ToObject(itemStatString);

            var jsonKeys = dropItemListJson.Keys;

            foreach (var key in jsonKeys)
            {
                ItemStat.Add(key, float.Parse(dropItemListJson[key].ToString()));
            }
        }
    }
}