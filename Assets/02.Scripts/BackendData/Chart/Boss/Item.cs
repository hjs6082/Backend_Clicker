// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace BackendData.Chart.Boss
{
    //===============================================================
    // Boss 차트의 각 row 데이터를 저장하는 클래스
    //===============================================================
    public class Item
    {
        // 드롭 아이템 정보 클래스
        public class DropItem
        {
            // 드롭 아이템의 ID
            public int ItemID { get; private set; }

            // 드롭 확률 (퍼센트)
            public float DropPercent { get; private set; }

            // 생성자: 드롭 아이템 ID와 드롭 확률을 설정
            public DropItem(int itemID, float dropPercent)
            {
                ItemID = itemID;
                DropPercent = dropPercent;
            }
        }

        // 보스의 고유 ID
        public int BossID { get; private set; }
        // 보스의 이름
        public string BossName { get; private set; }
        // 보스의 체력
        public float Hp { get; private set; }
        // 보스의 레벨
        public int Lv { get; private set; }
        // 보스를 처치했을 때 획득 가능한 경험치
        public float Exp { get; private set; }
        // 보스를 처치했을 때 획득 가능한 돈
        public float Money { get; private set; }
        // 보스를 처치했을 때 드롭 가능한 아이템 리스트
        public List<DropItem> DropItemList { get; private set; }
        // 보스의 이미지 파일 경로
        public string Image { get; private set; }
        // 생성자: JSON 데이터를 기반으로 보스 데이터를 초기화
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

            // 드롭 아이템 정보가 없는 경우 함수 종료
            if (string.IsNullOrEmpty(dropItemListString) || dropItemListString == "null")
            {
                return;
            }

            // 드롭 아이템 리스트 JSON 데이터를 파싱
            JsonData dropItemListJson = JsonMapper.ToObject(dropItemListString);

            // 각 드롭 아이템 정보를 반복하여 리스트에 추가
            foreach (JsonData item in dropItemListJson)
            {
                // 드롭 아이템 ID
                int itemID = int.Parse(item["id"].ToString());

                // 드롭 확률
                float percent = float.Parse(item["percent"].ToString());

                // 드롭 아이템 객체를 생성하여 리스트에 추가
                DropItemList.Add(new DropItem(itemID, percent));
            }
        }
    }
}
