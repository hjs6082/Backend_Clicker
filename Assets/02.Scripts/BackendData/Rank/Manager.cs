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
    //===============================================================
    //  �������� �ҷ��� ��ŷ �����͸� �����ϴ� Ŭ����
    //===============================================================
    public class Manager : Base.Normal
    {

        List<RankTableItem> rankTableItemList = new();
        public IReadOnlyList<RankTableItem> List => (IReadOnlyList<RankTableItem>)rankTableItemList.AsReadOnlyList();


        public override void BackendLoad(AfterBackendLoadFunc afterBackendLoadFunc)
        {

            bool isSuccess = false;
            string className = GetType().Name;
            string funcName = MethodBase.GetCurrentMethod()?.Name;
            string errorInfo = string.Empty;

            //[�ڳ�] ��� ���� ��ŷ ������ ���� ��ȸ �Լ� ȣ��
            SendQueue.Enqueue(Backend.URank.User.GetRankTableList, callback => {
                try
                {
                    Debug.Log($"Backend.URank.User.GetRankTableList : {callback}");

                    if (callback.IsSuccess())
                    {

                        JsonData rankTableListJson = callback.FlattenRows();

                        // ���� �� ���ϵ� ���� �̿��Ͽ� �Ľ�.
                        for (int i = 0; i < rankTableListJson.Count; i++)
                        {
                            RankTableItem rankTableItem = new RankTableItem(rankTableListJson[i]);
                            rankTableItemList.Add(rankTableItem);
                        }
                    }
                    else
                    {
                        if (callback.GetMessage().Contains("rank not found"))
                        {
                            StaticManager.UI.AlertUI.OpenWarningUI("��ŷ�� �������� �ʽ��ϴ�.", "��ŷ�� ã��  �� �����ϴ�.\n���� �����Ͱ� ������ ���� �ش� �����͸� �̿��Ͽ� ��ŷ�� �������ֽñ� �ٶ��ϴ�.");

                        }
                        else
                        {
                            throw new Exception(callback.ToString());
                        }
                    }
                    isSuccess = true;
                }
                catch (Exception e)
                {
                    errorInfo = e.ToString();
                }
                finally
                {
                    afterBackendLoadFunc(isSuccess, className, funcName, errorInfo);
                }
            });


        }
    }
}