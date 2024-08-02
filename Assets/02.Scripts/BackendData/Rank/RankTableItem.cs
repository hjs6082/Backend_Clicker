// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.Rank
{
    public enum RankType
    {
        user,
        guild
    }

    //===============================================================
    //  �������� �ҷ��� ��ŷ ����Ʈ�� ���̺� ������
    //===============================================================
    public class RankTableItem
    {
        public RankType rankType { get; private set; }
        public string date { get; private set; }
        public string uuid { get; private set; }
        public string order { get; private set; }
        public bool isReset { get; private set; }
        public string title { get; private set; }
        public string table { get; private set; }
        public string column { get; private set; }

        //��ȸ�� ��ŷ���� ����
        public DateTime rankStartDateAndTime { get; private set; }
        public DateTime rankEndDateAndTime { get; private set; }

        //�߰� �׸��� ���� ��쿡�� ����
        public string extraDataColumn { get; private set; }
        public string extraDataType { get; private set; }

        public int totalUserCount { get; private set; }

        public DateTime UpdateTime { get; private set; }
        public DateTime MyRankUpdateTime { get; private set; }
        public RankUserItem MyRankItem { get; private set; }

        private List<RankUserItem> _userList = new();
        private IReadOnlyList<RankUserItem> UserList => (IReadOnlyList<RankUserItem>)_userList.AsReadOnlyList();

        // ��ŷ�� �ҷ��� �Ŀ� �ٲ� List���� �����ϴ� �븮�� �Լ�
        public delegate void GetListFunc(bool isSuccess, IReadOnlyList<RankUserItem> rankList);

        // ��ŷ ����Ʈ�� �����ϴ� �Լ�
        public void GetRankList(GetListFunc getListFunc)
        {
            // �������� 5���� ������ �ʾ����� ĳ�̵� ���� ����
            if ((DateTime.UtcNow - UpdateTime).Minutes < 5)
            {
                getListFunc(true, UserList);
                return;
            }
            string className = GetType().Name;
            string funcName = MethodBase.GetCurrentMethod()?.Name;
            // 5���� ������ ��쿡�� ��ŷ �Լ� ȣ��
            // [�ڳ�] ��ŷ ����Ʈ �ҷ����� �Լ�
            SendQueue.Enqueue(Backend.URank.User.GetRankList, uuid, 10, callback => {
                try
                {
                    Debug.Log($"Backend.URank.User.GetRankList({uuid}) : {callback}");
                    if (callback.IsSuccess())
                    {
                        UpdateTime = DateTime.UtcNow;

                        JsonData rankJson = callback.GetFlattenJSON();

                        totalUserCount = int.Parse(rankJson["totalCount"].ToString());
                        _userList.Clear();
                        foreach (JsonData tempJson in rankJson["rows"])
                        {
                            _userList.Add(new RankUserItem(tempJson, table, extraDataColumn));
                        }

                        getListFunc(true, UserList);
                    }
                    else
                    {
                        getListFunc(false, UserList);
                    }
                }
                catch (Exception e)
                {
                    StaticManager.UI.AlertUI.OpenErrorUI(className, funcName, e);
                    getListFunc(false, UserList);
                }
            });
        }

        public delegate void GetMyRankFunc(bool isSuccess, RankUserItem rankItem);

        // �� ��ŷ�� �ҷ����� ���� ������ �ѹ� �ߴ��� ����
        private bool _isTwiceRepeat = false;

        // �� ��ŷ�� ���ŵ��� �ʾ��� ��쿡�� Update�� �ѹ� ȣ��
        public void GetMyRank(GetMyRankFunc getMyRankFunc)
        {
            // 5���� ������ �ʾ��� ��쿡�� ĳ�̵� �� ����
            if ((DateTime.UtcNow - MyRankUpdateTime).Minutes < 5)
            {
                getMyRankFunc(true, MyRankItem);
                return;
            }

            string className = GetType().Name;
            string funcName = MethodBase.GetCurrentMethod()?.Name;

            SendQueue.Enqueue(Backend.URank.User.GetMyRank, uuid, callback => {
                try
                {
                    Debug.Log($"Backend.URank.User.GetMyRank({uuid}) : {callback}");
                    if (callback.IsSuccess())
                    {
                        // ���� �ֱ� ����
                        MyRankUpdateTime = DateTime.UtcNow;

                        // �� ������ ����
                        MyRankItem = new RankUserItem(callback.FlattenRows()[0], table, extraDataColumn);

                        // ������ �����ͷ� ����
                        getMyRankFunc(true, MyRankItem);
                    }
                    else
                    {
                        // ������ �߻��Ͽ��� ���

                        // ���� �� ��ŷ�� ���ŵǾ����� ���� ���
                        if (callback.GetMessage().Contains("userRank not found"))
                        {
                            // ������ �Ͽ��´뵵 �ٽ� ���⸦ ȣ���� ���
                            if (_isTwiceRepeat)
                            {
                                // ���� ������ ������ �ֱ� ������ bool������ �ѹ��� ȣ���ϰ� �����Ѵ�
                                _isTwiceRepeat = false;
                                getMyRankFunc(false, MyRankItem);
                                return;
                            }

                            _isTwiceRepeat = false;

                            StaticManager.Backend.UpdateUserRankScore(uuid, (afterUpdateCallback) => {
                                if (afterUpdateCallback == null)
                                {
                                    throw new Exception("afterUpdateCallback�� null�Դϴ�.");
                                }
                                if (afterUpdateCallback.IsSuccess())
                                {
                                    // �����Ͽ��� ��� �ٽ��ѹ� �� ��ŷ �ҷ����� ȣ��
                                    _isTwiceRepeat = true;
                                    GetMyRank(getMyRankFunc);
                                }
                                else
                                {
                                    StaticManager.UI.AlertUI.OpenWarningUI("��ŷ �ҷ����� �Ұ� �ȳ�",
                                        "��ŷ�� �ҷ��� �� �����ϴ�.\n5�� �ڿ� �ٽ� �õ����ּ���");
                                    getMyRankFunc(false, MyRankItem);
                                }
                            });
                        }
                        else
                        {
                            // "userRank not found" ���� �� �ٸ� ������ ���, �׳� ����
                            // ������ StaticManager.Backend.UpdateUserRankScore���� ����Ѵ�.
                            getMyRankFunc(false, MyRankItem);
                        }
                    }
                }
                catch (Exception e)
                {
                    StaticManager.UI.AlertUI.OpenErrorUI(className, funcName, e);
                    getMyRankFunc(false, MyRankItem);
                }
            });
        }

        // Backend.URank.User.GetRankTableList()�� ���ϵ����� �Ľ�
        public RankTableItem(JsonData gameDataJson)
        {
            date = gameDataJson["date"].ToString();
            uuid = gameDataJson["uuid"].ToString();
            order = gameDataJson["order"].ToString();
            isReset = gameDataJson["isReset"].ToString() == "true" ? true : false;
            title = gameDataJson["title"].ToString();
            table = gameDataJson["table"].ToString();
            column = gameDataJson["column"].ToString();

            if (gameDataJson.ContainsKey("rankStartDateAndTime"))
            {
                rankStartDateAndTime = DateTime.Parse(gameDataJson["rankStartDateAndTime"].ToString());
                rankEndDateAndTime = DateTime.Parse(gameDataJson["rankEndDateAndTime"].ToString());
            }

            if (gameDataJson.ContainsKey("extraDataColumn"))
            {
                extraDataColumn = gameDataJson["extraDataColumn"].ToString();
                extraDataType = gameDataJson["extraDataType"].ToString();
            }

            if (!Enum.TryParse<RankType>(gameDataJson["rankType"].ToString(), out var tempRankType))
            {
                throw new Exception($"[{uuid}] - �������� ���� RankType �Դϴ�.");
            }

            rankType = tempRankType;
            MyRankUpdateTime = DateTime.MinValue;
            UpdateTime = DateTime.MinValue;
        }
    }
}