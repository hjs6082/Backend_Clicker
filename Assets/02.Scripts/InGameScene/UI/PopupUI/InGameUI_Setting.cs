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
        //[SerializeField]
        //private TMP_Text nowStageText;
        //[SerializeField]
        //private TMP_Text nowAtkText;
        [SerializeField]
        private TMP_Text defeatEnemyCountText;
        [SerializeField]
        private TMP_Text allUsingGoldText;
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
            //nowStageText.text = FindObjectOfType<InGameUI_Stage>().gameObject.GetComponent<TMP_Text>().text;

            defeatEnemyCountText.text = StaticManager.Backend.GameData.UserData.AllDefeatEnemyCount.ToString();
            allUsingGoldText.text = StaticManager.Backend.GameData.UserData.AllUsingGold.ToString() + " G"; 
 /*
            foreach (var item in StaticManager.Backend.GameData.WeaponEquip.Dictionary.Keys)
            {
                if (item != null)
                {
                    foreach (var item2 in StaticManager.Backend.GameData.WeaponInventory.Dictionary.Values)
                    {
                        if(item == item2.MyWeaponId.ToString())
                        {
                            nowAtkText.text = item2.GetWeaponChartData().Atk.ToString();
                        }
                    }
                }
            }
 */

            if (StaticManager.Backend.GameData.UserData.LastLoginTime != null)
            {
                lastDateText.text = StaticManager.Backend.GameData.UserData.LastLoginTime;
            }
            else
            {
                lastDateText.text = "없음";
            }

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
            StaticManager.UI.AlertUI.OpenAlertUI("안내", "(구현 예정)\n미구현된 기능입니다.");
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
            StaticManager.UI.SetLoadingIcon(true);
            //[뒤끝] 일정주기마다 자동저장하는기능을 수동으로 호출
            StaticManager.Backend.UpdateAllGameData(callback =>
            {
                StaticManager.UI.SetLoadingIcon(false);

                if (callback == null)
                {
                    StaticManager.UI.AlertUI.OpenAlertUI("저장 데이터 미존재", "저장할 데이터가 존재하지 않습니다.");
                    return;
                }

                if (callback.IsSuccess())
                {
                    StaticManager.UI.AlertUI.OpenAlertUI("저장 성공", "저장에 성공했습니다.");
                }
                else
                {
                    StaticManager.UI.AlertUI.OpenErrorUIWithText("수동 저장 실패",
                        $"수동 저장에 실패했습니다.\n{callback.ToString()}");
                }

            });
        }

        void OnCloseButtonClick()
        {
            Destroy(this.gameObject);
        }
    }
}
