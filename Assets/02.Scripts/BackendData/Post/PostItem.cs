// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using InGameScene;
using LitJson;
using UnityEngine;

namespace BackendData.Post
{

    // ������ ����ϴ� ��Ʈ
    // ���ο� ��Ʈ�� ������ �������� �Ҷ����� �׿� �´� ������ �߰��ؾ��Ѵ�.
    public enum ChartType
    {
        forPost,
        weaponChart,
        itemChart
    }
    //===============================================================
    //  ������ ���� ���� Ŭ����
    //===============================================================
    public class PostChartItem
    {
        public ChartType chartType { get; private set; }
        public int itemID { get; private set; }
        public float itemCount { get; private set; }
        public string itemName { get; private set; }

        private delegate void ReceiveFunc();

        private ReceiveFunc _receiveFunc = null;

        // PostItem Ŭ������ ������ ����ϴ� ��Ʈ ������ �Ľ��ϴ� Ŭ����
        public PostChartItem(JsonData json)
        {
            itemCount = float.Parse(json["itemCount"].ToString());
            string chartName = json["chartName"].ToString();

            if (!Enum.TryParse<ChartType>(chartName, out var tempChartType))
            {
                throw new Exception("�������� ���� Post ChartType �Դϴ�.");
            }
            chartType = tempChartType;

            // ���� ��Ź�� �������� ���� ��Ʈ�� Ÿ�Կ� �°� ����
            try
            {
                switch (chartType)
                {
                    case ChartType.forPost:
                        itemID = int.Parse(json["item"]["ItemID"].ToString());
                        if (itemID == 1)
                        {
                            itemName = "gold";
                            //Receive �Լ� ȣ�� �� �ش� ��������Ʈ ȣ��
                            _receiveFunc = () => { Managers.Game.UpdateUserData(itemCount, 0); };
                        }
                        else if (itemID == 2)
                        {
                            itemName = "jewel";
                            // Jewel�� ���� �̱���
                        }
                        else if (itemID == 3)
                        {
                            itemName = "exp";
                            //Receive �Լ� ȣ�� �� �ش� ��������Ʈ ȣ��
                            _receiveFunc = () => { Managers.Game.UpdateUserData(0, itemCount); };
                        }
                        break;
                    case ChartType.weaponChart:
                        itemID = int.Parse(json["item"]["WeaponID"].ToString());
                        itemName = StaticManager.Backend.Chart.Weapon.Dictionary[itemID].WeaponName;
                        //Receive �Լ� ȣ�� �� itemID�� ���⸦ �򵵷� ������Ʈ
                        //_receiveFunc = () => { Managers.Game.UpdateWeaponInventory(itemID); };
                        break;
                    case ChartType.itemChart:
                        itemID = int.Parse(json["item"]["ItemID"].ToString());
                        itemName = StaticManager.Backend.Chart.Item.Dictionary[itemID].ItemName;
                        //Receive �Լ� ȣ�� �� itemID�� �������� �򵵷� ������Ʈ
                        _receiveFunc = () => { Managers.Game.UpdateItemInventory(itemID, (int)itemCount); };
                        break;
                }
            }
            catch (Exception e)
            {
                throw new Exception("PostChartItem : itemID�� �Ľ����� ���߽��ϴ�.\n" + e.ToString());
            }
        }

        public void Receive()
        {
            _receiveFunc.Invoke();
        }
    }

    //===============================================================
    //  Post.Manager Ŭ������ GetPostList�� ���ϰ��� ���� �Ľ��ϴ� Ŭ����
    //===============================================================
    public class PostItem
    {
        public readonly string title;
        public readonly string content;
        public readonly DateTime expirationDate;
        public readonly string inDate;
        public readonly PostType PostType;

        // public readonly string author;
        // public readonly DateTime reservationDate;
        // public readonly DateTime sentDate;
        // public readonly string nickname;

        public readonly List<PostChartItem> items = new List<PostChartItem>();

        public PostItem(PostType postType, JsonData postListJson)
        {
            PostType = postType;

            content = postListJson["content"].ToString();
            expirationDate = DateTime.Parse(postListJson["expirationDate"].ToString());
            inDate = postListJson["inDate"].ToString();
            title = postListJson["title"].ToString();

            // sentDate = DateTime.Parse(postListJson["sentDate"].ToString());
            // reservationDate = DateTime.Parse(postListJson["reservationDate"].ToString());
            // nickname = postListJson["nickname"].ToString();
            // if (postListJson.ContainsKey("author")) {
            //     author = postListJson["author"].ToString();
            // }

            // ���� ���� ������
            if (postListJson["items"].Count > 0)
            {
                for (int itemNum = 0; itemNum < postListJson["items"].Count; itemNum++)
                {
                    PostChartItem item = new PostChartItem(postListJson["items"][itemNum]);
                    items.Add(item);
                }
            }
        }

        // ���� ������ ���� �� ȣ��Ǵ� ��������Ʈ �Լ�
        public delegate void IsReceiveSuccessFunc(bool isSuccess);

        // [�ڳ�] ���� ���� �Լ�
        public void ReceiveItem(IsReceiveSuccessFunc isReceiveSuccessFunc)
        {
            SendQueue.Enqueue(Backend.UPost.ReceivePostItem, PostType, inDate, callback => {
                bool isSuccess = false;
                try
                {
                    Debug.Log($"Backend.UPost.ReceivePostItem({PostType}, {inDate}) : {callback}");

                    // ������ ���
                    if (callback.IsSuccess())
                    {
                        isSuccess = true;

                        string postItemString = String.Empty;

                        // �ش� ������ ������ �ִ� item�� Receive�Լ��� ȣ���Ͽ� ������ ȹ��
                        foreach (var item in items)
                        {
                            item.Receive();
                            postItemString += $"{item.itemName} x {item.itemCount}\n";
                        }

                        // ������ ���� ��쿡�� �׳� �н�
                        if (string.IsNullOrEmpty(postItemString))
                        {
                            postItemString = "�ش� ������ �������� �������� �ʽ��ϴ�.";
                            StaticManager.UI.AlertUI.OpenAlertUI("���� ���� �Ϸ�", postItemString);
                        }
                        else
                        {
                            // ������ �� ��� �� �������� ����
                            // ������ ������ ��� ������ ���ŵǱ� ������ ������Ʈ �ֱ⿡ ������ �����ϸ� ���� ���ɿ� ���� ����� ����� �� �ִ�.
                            StaticManager.Backend.UpdateAllGameData(callback => {
                                if (callback.IsSuccess())
                                {
                                    StaticManager.UI.AlertUI.OpenAlertUI("���� ���� �Ϸ�", "���� �������� �����Ͽ����ϴ�\n" + postItemString);
                                }
                                else
                                {
                                    StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), callback.ToString());
                                }
                            });
                        }
                    }
                    else
                    {
                        StaticManager.UI.AlertUI.OpenErrorUIWithText("���� ���� ���� ����", "���� ���ɿ� �����߽��ϴ�.\n" + callback.ToString());
                    }
                }
                catch (Exception e)
                {
                    StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), e.ToString());
                }
                finally
                {
                    if (isSuccess)
                    {
                        //������ �Ϸ�� ��� ���� ��Ͽ��� ����
                        StaticManager.Backend.Post.RemovePost(inDate);
                    }

                    isReceiveSuccessFunc(isSuccess);
                }
            });
        }
    }
}