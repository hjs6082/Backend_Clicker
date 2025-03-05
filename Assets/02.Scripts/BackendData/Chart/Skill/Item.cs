// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace BackendData.Chart.Skill
{
    //===============================================================
    // Skill 차트의 각 row 데이터 클래스
    //===============================================================
    public class Item
    {
        public int SkillID { get; private set; }
        public string SkillName { get; private set; }
        public string SkillContent { get; private set; }

        public string SkillImageName { get; private set; }

        public int SkillCooltime { get; private set; }
        public int Price { get; private set; }

        public Sprite SkillSprite { get; set; }

        public Item(JsonData json)
        {
            SkillID = int.Parse(json["SkillID"].ToString());
            SkillName = json["SkillName"].ToString();
            SkillContent = json["SkillContent"].ToString();
            SkillImageName = json["SkillImageName"].ToString();
            SkillCooltime = int.Parse(json["SkillCooltime"].ToString());
            Price = int.Parse(json["Price"].ToString());
        }
    }
}