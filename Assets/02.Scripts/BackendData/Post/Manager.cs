// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;

namespace BackendData.Post
{
    //===============================================================
    //  ���� �����͸� �����ϴ� Ŭ����
    //===============================================================
    public class Manager : Base.Normal
    {
        Dictionary<string, PostItem> _dictionary = new();
        public IReadOnlyDictionary<string, PostItem> Dictionary => (IReadOnlyDictionary<string, PostItem>)_dictionary.AsReadOnlyCollection();


        private DateTime _rankPostUpdateTime;

        // �ε������� ȣ��Ǵ� �Լ�
        public override void BackendLoad(AfterBackendLoadFunc afterBackendLoadFunc)
        {
            string className = GetType().Name;
            string funcName = MethodBase.GetCurrentMethod()?.Name;

            // ������ ���� �ҷ�����
            GetPostList(PostType.Admin, (isSuccess, errorInfo) => {
                afterBackendLoadFunc.Invoke(isSuccess, className, funcName, errorInfo);
            });
        }

        public void BackendLoadForRank(AfterBackendLoadFunc afterBackendLoadFunc)
        {
            _rankPostUpdateTime = DateTime.MinValue;

            string className = GetType().Name;
            string funcName = MethodBase.GetCurrentMethod()?.Name;

            // ������ ���� �ҷ�����
            GetPostList(PostType.Rank, (isSuccess, errorInfo) => {
                afterBackendLoadFunc.Invoke(isSuccess, className, funcName, errorInfo);
            });
        }

        public delegate void AfterGetPostFunc(bool isSuccess, string errorInfo);

        // ���� ����Ʈ �ҷ����� �Լ�
        public void GetPostList(PostType postType, AfterGetPostFunc afterPostLoadingFunc)
        {
            bool isSuccess = false;
            string errorInfo = string.Empty;

            // ��ŷ ������ ���, UI�� �������� ����.
            // UI ������ 10���� ������ �ʾ��� ��쿡�� ĳ�̵� ���� ����
            if (postType == PostType.Rank)
            {
                if ((DateTime.UtcNow - _rankPostUpdateTime).Minutes < 10)
                {
                    afterPostLoadingFunc(true, string.Empty);
                    return;
                }
            }

            //[�ڳ�] ���� ��� �ҷ����� �Լ�
            SendQueue.Enqueue(Backend.UPost.GetPostList, postType, callback => {
                try
                {
                    Debug.Log($"Backend.UPost.GetPostList({postType}) : {callback}");

                    if (callback.IsSuccess() == false)
                    {
                        throw new Exception(callback.ToString());
                    }

                    // ��ŷ ���� �ð� �ֱٽð����� ����
                    _rankPostUpdateTime = DateTime.UtcNow;

                    JsonData postListJson = callback.GetReturnValuetoJSON()["postList"];

                    for (int i = 0; i < postListJson.Count; i++)
                    {

                        if (_dictionary.ContainsKey(postListJson[i]["inDate"].ToString()))
                        {
                            //���� �ҷ��� ���� ���� �ȹ��� ������ �����Ͱ� ���� ��� �н�
                        }
                        else
                        {
                            // ���ο� ������ ���
                            PostItem postItem = new PostItem(postType, postListJson[i]);
                            _dictionary.Add(postItem.inDate, postItem);
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
                    afterPostLoadingFunc.Invoke(isSuccess, errorInfo);
                }
            });
        }

        public void RemovePost(string inDate)
        {
            _dictionary.Remove(inDate);
        }
    }
}