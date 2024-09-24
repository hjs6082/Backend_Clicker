// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using UnityEngine;

namespace InGameScene.UI
{
    public class InGameUI_Post : InGameUI_LeftUIBase
    {
        [SerializeField] private GameObject _postItemParentGroup;
        [SerializeField] private GameObject _postItemPrefab;
        [SerializeField] private GameObject _noPostAlertText;
        [SerializeField] private GameObject _postIconAlert;

        Dictionary<string, GameObject> _postItemDictionary = new();

        public override void Init()
        {
            base.Init();
        }

        public override void Open()
        {
            base.Open();

            StaticManager.UI.SetLoadingIcon(true);

            // �����Կ� ������ ���� ��� �ؽ�Ʈ ���
            if (StaticManager.Backend.Post.Dictionary.Count <= 0)
            {
                _noPostAlertText.gameObject.SetActive(true);
            }
            else
            {
                _noPostAlertText.gameObject.SetActive(false);
            }

            //���� ��� ��ȸ �� ������� �̿��Ͽ� ����Ʈ ������ ����
            StaticManager.Backend.Post.GetPostList(PostType.Rank, (success, info) => {
                StaticManager.UI.SetLoadingIcon(false);

                if (success)
                {
                    foreach (var list in StaticManager.Backend.Post.Dictionary)
                    {
                        // indate�� �ߺ��� ��쿡�� �н�
                        if (_postItemDictionary.ContainsKey(list.Value.inDate))
                        {
                            continue;
                        }

                        // ������ ����, �������� ���� ��ư�� RemovePost �Լ� ����
                        var obj = Instantiate(_postItemPrefab, _postItemParentGroup.transform, true);
                        obj.transform.localScale = new Vector3(1, 1, 1);
                        obj.GetComponent<InGameUI_PostItem>().Init(list.Value, RemovePost);
                        _postItemDictionary.Add(list.Value.inDate, obj);
                    }
                }
                else
                {
                    StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(),
                        info);
                }
            });
        }

        // ���� �������� ���� ��ư Ŭ���� ȣ��
        // UI ����Ʈ���� �ش� ���� ����
        private void RemovePost(string inDate)
        {
            if (_postItemDictionary.ContainsKey(inDate))
            {
                Destroy(_postItemDictionary[inDate]);
                _postItemDictionary.Remove(inDate);
            }
        }

        public void SetPostIconAlert(bool isActive)
        {
           _postIconAlert.SetActive(isActive);
        }
    }
}