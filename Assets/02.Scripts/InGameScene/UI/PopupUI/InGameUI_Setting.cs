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
                lastDateText.text = "����";
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
            StaticManager.UI.AlertUI.OpenAlertUI("�ȳ�", "(���� ����)\n�̱����� ����Դϴ�.");
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
            StaticManager.UI.SetLoadingIcon(true);
            //[�ڳ�] �����ֱ⸶�� �ڵ������ϴ±���� �������� ȣ��
            StaticManager.Backend.UpdateAllGameData(callback =>
            {
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
        }

        void OnCloseButtonClick()
        {
            Destroy(this.gameObject);
        }
    }
}
