using System;
using System.Reflection;
using InGameScene.UI.PopupUI;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene
{
    public class InGameUI_PopupManager : MonoBehaviour
    {
        [SerializeField] private GameObject _popupParentObject;

        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _playerIconSettingButton;

        private const string _path = "Prefabs/InGameScene/UI/";

        void Start()
        {
            _settingButton.onClick.AddListener(OpenSettingUI);
            _playerIconSettingButton.onClick.AddListener(OpenPlayerIconSettingUI);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OpenUI<InGameUI_Exit>("InGameUI_Exit");
            }
        }

        private void OpenSettingUI()
        {
            OpenUI<InGameUI_Setting>("InGameUI_Setting");
        }

        private void OpenPlayerIconSettingUI()
        {
            OpenUI<InGameUI_PlayerIcon>("InGameUI_PlayerIcon");
        }

        // InGamePopupUI이 부모인 UI를 생성하거나 활성화시키는 함수.
        private void OpenUI<T>(string prefabName) where T : InGamePopupUI
        {
            try
            {
                T ui = transform.GetComponentInChildren<T>();

                if (ui == null)
                {
                    var obj = Resources.Load<GameObject>(_path + prefabName);
                    ui = Instantiate(obj, _popupParentObject.transform, true).GetComponent<T>();
                    ui.transform.localScale = new Vector3(1, 1, 1);

                    // Init의 경우, 한번만 호출.
                    ui.Init();
                }
                ui.gameObject.SetActive(true);
                ui.Open();
            }
            catch (Exception e)
            {
                StaticManager.UI.AlertUI.OpenErrorUI(typeof(T).Name, MethodBase.GetCurrentMethod()?.ToString(), e);
            }
        }
    }
}
