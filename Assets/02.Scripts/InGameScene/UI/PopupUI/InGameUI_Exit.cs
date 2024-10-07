using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI.PopupUI
{
    //===========================================================
    // �ݱ� UI
    //===========================================================
    public class InGameUI_Exit : InGamePopupUI
    {
        [SerializeField]
        private Button closeButton;
        [SerializeField]
        private Button saveAndExitButton;
        [SerializeField]
        private Button exitButton;

        public override void Init()
        {
            closeButton.onClick.AddListener(OnCloseButtonClick);
            saveAndExitButton.onClick.AddListener(OnSaveAndExitButtonClick);
            exitButton.onClick.AddListener(OnExitButton);
        }

        public override void Open()
        {
            //throw new System.NotImplementedException();
        }

        void OnCloseButtonClick()
        {
            Destroy(this.gameObject);
        }

        void OnSaveAndExitButtonClick()
        {
            StaticManager.Backend.UpdateAllGameData(callback => {
                StaticManager.UI.SetLoadingIcon(false);
                if (callback == null)
                {
                    Application.Quit();
                }
                if (callback.IsSuccess())
                {
                    Application.Quit();
                }
                else
                {
                    StaticManager.UI.AlertUI.OpenErrorUIWithText("���� ���� ����",
                        $"���� ���忡 �����߽��ϴ�.\n{callback.ToString()}");
                }
            });
        }

        void OnExitButton()
        {
            Application.Quit();
        }
    }
}
