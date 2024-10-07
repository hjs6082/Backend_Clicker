using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI.PopupUI
{
    //===========================================================
    // 닫기 UI
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
                    StaticManager.UI.AlertUI.OpenErrorUIWithText("수동 저장 실패",
                        $"수동 저장에 실패했습니다.\n{callback.ToString()}");
                }
            });
        }

        void OnExitButton()
        {
            Application.Quit();
        }
    }
}
