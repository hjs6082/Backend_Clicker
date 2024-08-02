// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


namespace InGameScene.UI.PopupUI
{

    //===========================================================
    // ģ��,���, �̺�Ʈ�� ���� �˾� UI�� �� �� �ִ� UI�� ���� Ŭ����
    //===========================================================
    public class InGameUI_Etc : InGamePopupUI
    {
        [SerializeField] private Button _friendButton;
        [SerializeField] private Button _guildButton;
        [SerializeField] private Button _eventButton;
        [SerializeField] private Button _couponButton;
        [SerializeField] private Button _gachaUIButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _logoutUIButton;

        public override void Init()
        {
            _friendButton.onClick.AddListener(FriendButton);
            _guildButton.onClick.AddListener(GuildButton);
            _eventButton.onClick.AddListener(EventButton);
            _couponButton.onClick.AddListener(CouponButton);
            _gachaUIButton.onClick.AddListener(GachaButton);
            _saveButton.onClick.AddListener(SaveButton);
            _logoutUIButton.onClick.AddListener(LogOutButton);
        }

        public override void Open()
        {
            // ���� ȣ���� �ʿ�����Ƿ� ���� �۾� ����
        }

        private void FriendButton()
        {
            StaticManager.UI.AlertUI.OpenWarningUI("�̱��� �ȳ�", "ģ�� ����� �غ����Դϴ�.");
        }

        private void GuildButton()
        {
            StaticManager.UI.AlertUI.OpenWarningUI("�̱��� �ȳ�", "��� ����� �غ����Դϴ�.");

        }
        private void EventButton()
        {
            StaticManager.UI.AlertUI.OpenWarningUI("�̱��� �ȳ�", "�̺�Ʈ/���������� �غ����Դϴ�.");

        }
        private void CouponButton()
        {
            StaticManager.UI.AlertUI.OpenWarningUI("�̱��� �ȳ�", "������ �غ����Դϴ�.");

        }
        private void GachaButton()
        {
            StaticManager.UI.AlertUI.OpenWarningUI("�̱��� �ȳ�", "Ȯ�� �̱��� �غ����Դϴ�.");

        }

        private void SaveButton()
        {
            StaticManager.UI.AlertUI.OpenWarningUI("���� �ȳ�", "Ȯ���� ������ ���������� ����˴ϴ�.", () => {
                StaticManager.UI.SetLoadingIcon(true);
                //[�ڳ�] �����ֱ⸶�� �ڵ������ϴ±���� �������� ȣ��
                StaticManager.Backend.UpdateAllGameData(callback => {
                    StaticManager.UI.SetLoadingIcon(false);

                    if (callback == null)
                    {
                        StaticManager.UI.AlertUI.OpenAlertUI("���� ������ ������", "������ �����Ͱ� �������� �ʽ��ϴ�.");
                        return;
                    }

                    if (callback.IsSuccess())
                    {
                        StaticManager.UI.AlertUI.OpenAlertUI("���� ����", "���忡 �����߽��ϴ�.");
                    }
                    else
                    {
                        StaticManager.UI.AlertUI.OpenErrorUIWithText("���� ���� ����",
                            $"���� ���忡 �����߽��ϴ�.\n{callback.ToString()}");
                    }

                });
            });

        }

        private void LogOutButton()
        {
            StaticManager.UI.AlertUI.OpenWarningUI("�α׾ƿ� �ȳ�", "Ȯ���� ������ �α׾ƿ��� ����˴ϴ�.", () => {
                StaticManager.UI.SetLoadingIcon(true);
                //[�ڳ�] �α׾ƿ� �Լ�
                SendQueue.Enqueue(Backend.BMember.Logout, callback => {
                    Debug.Log($"Backend.BMember.Logout : {callback}");

                    if (callback.IsSuccess())
                    {
                        StaticManager.Instance.ChangeScene("LoginScene");
                    }
                    else
                    {
                        StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), callback.ToString());
                    }
                    StaticManager.UI.SetLoadingIcon(false);
                });
            });
        }


    }
}