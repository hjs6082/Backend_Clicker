// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace BackendData.Chart.Enemy
{
    //===============================================================
    // Enemy 차트의 각 row 데이터를 표현하는 클래스
    //===============================================================
    public class Item
    {
        //===============================================================
        // 적이 드롭하는 아이템 정보를 저장하는 클래스
        //===============================================================
        public class DropItem
        {
            // 드롭 아이템의 ID
            public int ItemID { get; private set; }

            // 드롭 확률 (퍼센트)
            public float DropPercent { get; private set; }

            // DropItem 생성자
            // - itemID: 드롭 아이템의 ID
            // - dropPercent: 아이템의 드롭 확률
            public DropItem(int itemID, float dropPercent)
            {
                ItemID = itemID;
                DropPercent = dropPercent;
            }
        }

        // 적의 고유 ID
        public int EnemyID { get; private set; }
        // 적의 이름
        public string EnemyName { get; private set; }
        // 적의 체력 (HP)
        public float Hp { get; private set; }
        // 적의 레벨
        public int Lv { get; private set; }
        // 적 처치 시 획득 경험치
        public float Exp { get; private set; }
        // 적 처치 시 획득 금액
        public float Money { get; private set; }
        // 적이 드롭하는 아이템 목록
        public List<DropItem> DropItemList { get; private set; }
        // 적의 이미지 파일 경로 또는 이름
        public string Image { get; private set; }

        // Item 생성자
        // - json: 적 정보를 담고 있는 JSON 데이터
        public Item(JsonData json)
        {
            // JSON 데이터에서 각 속성을 파싱하여 클래스 필드에 저장
            EnemyID = int.Parse(json["EnemyID"].ToString());
            EnemyName = json["EnemyName"].ToString();
            Hp = float.Parse(json["Hp"].ToString());
            Lv = int.Parse(json["Lv"].ToString());
            Exp = float.Parse(json["Exp"].ToString());
            Money = float.Parse(json["Money"].ToString());
            Image = json["Image"].ToString();

            // 드롭 아이템 리스트 초기화
            DropItemList = new List<DropItem>();

            // JSON 데이터에서 "DropItemList" 값을 문자열로 가져옴
            string dropItemListString = json["DropItemList"].ToString();

            // 드롭 아이템 리스트가 비어 있거나 "null"인 경우 처리 종료
            if (string.IsNullOrEmpty(dropItemListString) || dropItemListString == "null")
            {
                return;
            }

            // 문자열 형태의 DropItemList를 JSON 객체로 변환
            JsonData dropItemListJson = JsonMapper.ToObject(dropItemListString);

            // DropItemList의 각 요소를 순회하며 DropItem 객체를 생성하고 리스트에 추가
            foreach (JsonData item in dropItemListJson)
            {
                // 드롭 아이템 데이터에서 ID 값을 파싱
                int itemID = int.Parse(item["id"].ToString());

                // 드롭 아이템 데이터에서 드롭 확률 값을 파싱
                float percent = float.Parse(item["percent"].ToString());

                // 파싱한 데이터를 기반으로 DropItem 객체를 생성하고 리스트에 추가
                DropItemList.Add(new DropItem(itemID, percent));
            }
        }
    }
}
