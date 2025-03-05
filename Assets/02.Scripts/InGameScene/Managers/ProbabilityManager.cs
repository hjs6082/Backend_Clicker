using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System;

public enum ProbabilityItemType
{
    Exp,
    Gold,
    Item
}

public class ProbabilityCardV2
{
    public string probabilityName; // 차트이름
    public string probabilityExplain; // 차트 설명
    public int selectedProbabilityFileId; // 차트 파일 아이디

    public override string ToString()
    {
        return $"probabilityName: {probabilityName}\n" +
        $"probabilityExplain: {probabilityExplain}\n" +
        $"selectedProbabilityFileId: {selectedProbabilityFileId}\n";
    }
}

public class ProbabilityItem
{
    public string itemID;
    public string itemName;
    public ProbabilityItemType itemType;
    public string percent;
    public int num;

    public override string ToString()
    {
        return $"itemID : {itemID}\n" +
        $"itemName : {itemName}\n" +
        $"ProbabilityItemType : {itemType}\n" +
        $"percent : {percent}\n" +
        $"num : {num}\n";
    }
}


namespace InGameScene
{
    public class ProbabilityManager : MonoBehaviour
    {
        [SerializeField]
        private List<ProbabilityCardV2> probabilityCardList = new List<ProbabilityCardV2>();
        [SerializeField]
        private GameObject gachaPrefab;
        [SerializeField]
        private Transform gachaSpawnTransform;

        // Start is called before the first frame update
        public void Init()
        {
            GetProbabilityCardListTestV2();
        }

        // Update is called once per frame
        void Update()
        {
/*            if (Input.GetKeyDown(KeyCode.K))
            {
                GetProbabilityTest();
            }*/
        }

        public void GetProbabilityCardListTestV2()
        {
            var bro = Backend.Probability.GetProbabilityCardListV2();

            if (!bro.IsSuccess())
            {
                Debug.LogError("에러가 발생했습니다 : " + bro.ToString());
                return;
            }


            LitJson.JsonData json = bro.FlattenRows();

            for (int i = 0; i < json.Count; i++)
            {
                ProbabilityCardV2 probabilityCard = new ProbabilityCardV2();

                probabilityCard.probabilityName = json[i]["probabilityName"].ToString();
                probabilityCard.probabilityExplain = json[i]["probabilityExplain"].ToString();
                probabilityCard.selectedProbabilityFileId = int.Parse(json[i]["selectedProbabilityFileId"].ToString());

                probabilityCardList.Add(probabilityCard);
            }
        }

        public void GetProbability(string itemName, string chestName)
        {
            foreach (var probabilityCard in probabilityCardList)
            {
                if (probabilityCard.probabilityName == itemName)
                {
                    var bro = Backend.Probability.GetProbability(probabilityCard.selectedProbabilityFileId.ToString());

                    if (!bro.IsSuccess())
                    {
                        Debug.LogError(bro.ToString());
                        return;
                    }

                    LitJson.JsonData json = bro.GetFlattenJSON();

                    ProbabilityItem item = new ProbabilityItem();

                    item.itemID = json["elements"]["itemID"].ToString();
                    item.itemName = json["elements"]["itemName"].ToString();

                    if (!Enum.TryParse<ProbabilityItemType>(json["elements"]["itemType"].ToString(), out var itemType))
                    {
                        throw new Exception($"{item.itemID} - 지정되지 않은 ItemType 입니다.");
                    }

                    item.itemType = itemType;

                    item.num = int.Parse(json["elements"]["num"].ToString());
                    item.percent = json["elements"]["percent"].ToString();

                    GameObject gachaObj = Instantiate(gachaPrefab, gachaSpawnTransform);
                    gachaObj.GetComponent<InGameUI_Gacha>().Init(item, chestName);
                }
            }
        }

/*        public void GetProbabilityTest()
        {
            string selectedProbabilityFileId = "12956";

            var bro = Backend.Probability.GetProbability(selectedProbabilityFileId);

            if (!bro.IsSuccess())
            {
                Debug.LogError(bro.ToString());
                return;
            }

            LitJson.JsonData json = bro.GetFlattenJSON();

            ProbabilityItem item = new ProbabilityItem();

            item.itemID = json["elements"]["itemID"].ToString();
            item.itemName = json["elements"]["itemName"].ToString();

            if (!Enum.TryParse<ProbabilityItemType>(json["elements"]["itemType"].ToString(), out var itemType))
            {
                throw new Exception($"{item.itemID} - 지정되지 않은 ItemType 입니다.");
            }

            item.itemType = itemType;

            item.num = int.Parse(json["elements"]["num"].ToString());
            item.percent = json["elements"]["percent"].ToString();

            GameObject gachaObj = Instantiate(gachaPrefab, gachaSpawnTransform);
            gachaObj.GetComponent<InGameUI_Gacha>().Init(item);
        }*/

       /* private void GetItem(ProbabilityItem item)
        {
            switch (item.itemType)
            {
                case ProbabilityItemType.Exp:
                    Managers.Game.UpdateUserData(0, item.num);
                    Debug.Log("얻은 아이템 : " + item.itemName + item.num); 
                    break;
                case ProbabilityItemType.Gold:
                    Managers.Game.UpdateUserData(item.num, 0);
                    Debug.Log("얻은 아이템 : " + item.itemName + item.num);
                    break;
                case ProbabilityItemType.Item:
                    foreach (var chartItem in StaticManager.Backend.Chart.Item.Dictionary)
                    {
                        if(item.itemName == chartItem.Value.ItemName)
                        {
                            Managers.Game.UpdateItemInventory(chartItem.Value.ItemID, item.num);
                            Debug.Log("얻은 아이템 : " + item.itemName + item.num);
                        }
                    }
                    break;
                default:
                    break;
            }
        }*/
    }
}
