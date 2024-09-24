// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

namespace InGameScene.UI
{
    //===========================================================
    // 좌측 버튼에 따라 바뀌는 왼쪽 UI의 기본 베이스 클래스
    //===========================================================
    public class InGameUI_LeftUIBase : MonoBehaviour
    {
        [SerializeField] private InGameUI_Status _status;
        [SerializeField] private List<UnityEngine.UI.Button> closeButtons;
        
        public virtual void Init()
        {
            UpdateUI();

            foreach (var closeButton in closeButtons)
            {
                closeButton.onClick.AddListener(OnCloseButton);
            }
        }

        public virtual void Open()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            _status.UpdateUI();
        }

        private void OnCloseButton()
        {
            this.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
}