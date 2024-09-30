using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BackEnd;

namespace InGameScene.UI.PopupUI
{
    //===========================================================
    // 설정 팝업 UI
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
                StaticManager.UI.AlertUI.OpenWarningUI("로그아웃", "로그아웃 되었습니다\n 시작화면으로 돌아갑니다.");
                StaticManager.Instance.ChangeScene("LoginScene");
            });
        }

        void OnResignButtonClick()
        {
            //즉시 탈퇴
            SendQueue.Enqueue(Backend.BMember.WithdrawAccount, callback => {
                // 이후 처리
            });

            //2시간 뒤에 탈퇴 예약
            SendQueue.Enqueue(Backend.BMember.WithdrawAccount, 2, callback => {
                // 이후 처리
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
