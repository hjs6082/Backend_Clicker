// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace InGameScene.UI
{
    //===========================================================
    // ���� �˾� UI�� �� ���� �������� ������ ������ Ŭ����
    //===========================================================
    public class InGameUI_PostItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text postTitleText;
        [SerializeField] private TMP_Text postContentText;

        [SerializeField] private TMP_Text postRewardText;
        [SerializeField] private Button postReceiveButton;
        [SerializeField] private Sprite postItemIcon;
        //[SerializeField] private TMP_Text expirationDateText;

        private BackendData.Post.PostItem _postItem;

        public delegate void ReceivePostFunc(string inDate);

        private ReceivePostFunc _receivePostFunc;

        // List�� �ִ� PostItem�� �����͸� �̿��Ͽ� ���� ������ ����, ���� ���� �� ������ ��� UI���� ���� �����ϴ� ��ư ����
        public void Init(BackendData.Post.PostItem postItem, ReceivePostFunc func)
        {
            try
            {
                _postItem = postItem;

                postTitleText.text = _postItem.title;
                postContentText.text = _postItem.content;

               /* if ((_postItem.expirationDate - DateTime.UtcNow).Days > 0)
                {
                    expirationDateText.text = (_postItem.expirationDate - DateTime.UtcNow).Days + "�� ����";
                }
                else
                {
                    expirationDateText.text = (_postItem.expirationDate - DateTime.UtcNow).Hours + "�ð� ����";

                }*/

                string itemString = string.Empty;
                foreach (var item in _postItem.items)
                {
                    itemString += $"{item.itemName}\n";
                }

                if (itemString.Length > 0)
                {
                    itemString.TrimEnd('|');
                }

                postRewardText.text = itemString;
                _receivePostFunc = func;

                postReceiveButton.onClick.AddListener(Receive);
            }
            catch (Exception e)
            {
                throw new Exception($"{GetType().Name} : {MethodBase.GetCurrentMethod()?.ToString()} : {e.ToString()}");
            }
        }

        // �������� �޴� �Լ�
        void Receive()
        {
            try
            {
                // PostItem ��ü���� ���� �ޱ� �Լ� ������ ����� ����
                _postItem.ReceiveItem((isSuccess) => {
                    if (isSuccess)
                    {
                        // ���� �� InGameUI_Post���� �ش� ���� ����
                        _receivePostFunc.Invoke(_postItem.inDate);
                    }
                });
            }
            catch (Exception e)
            {
                StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), e.ToString());
            }
        }
    }
}