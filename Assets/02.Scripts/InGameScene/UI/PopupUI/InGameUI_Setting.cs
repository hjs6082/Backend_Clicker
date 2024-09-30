using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BackEnd;

namespace InGameScene.UI.PopupUI
{
    //===========================================================
    // ���� �˾� UI
    //===========================================================
    public class InGameUI_Setting : InGamePopupUI
    {
        [SerializeField]
        private TMP_Text nowStageText;
        [SerializeField]
        private TMP_Text nowAtkText;
        [SerializeField]
        private TMP_Text lastDateText;

        [SerializeField]
        private Button couponButton;
        [SerializeField]
        private Button logoutButton;
        [SerializeField]
        private Button resignButton;
        [SerializeField]
        private Button saveButton;
        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private GameObject couponPanel;

        public override void Init()
        {
            couponButton.onClick.AddListener(OnCouponButtonClick);
            logoutButton.onClick.AddListener(OnLogoutButtonClick);
            resignButton.onClick.AddListener(OnResignButtonClick);
            saveButton.onClick.AddListener(OnSaveButtonClick);
            closeButton.onClick.AddListener(OnCloseButtonClick);
        }

        public override void Open()
        {
            //throw new System.NotImplementedException();
        }


        void OnCouponButtonClick()
        {

        }

        void OnLogoutButtonClick()
        {
            SendQueue.Enqueue(Backend.BMember.Logout, (callback) => {
                StaticManager.UI.AlertUI.OpenWarningUI("�α׾ƿ�", "�α׾ƿ� �Ǿ����ϴ�\n ����ȭ������ ���ư��ϴ�.");
                StaticManager.Instance.ChangeScene("LoginScene");
            });
        }

        void OnResignButtonClick()
        {
            //��� Ż��
            SendQueue.Enqueue(Backend.BMember.WithdrawAccount, callback => {
                // ���� ó��
            });

            //2�ð� �ڿ� Ż�� ����
            SendQueue.Enqueue(Backend.BMember.WithdrawAccount, 2, callback => {
                // ���� ó��
            });
        }

        void OnSaveButtonClick()
        {

        }

        void OnCloseButtonClick()
        {
            Destroy(this.gameObject);
        }
    }
}
