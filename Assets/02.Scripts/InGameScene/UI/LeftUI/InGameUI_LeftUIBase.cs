// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

namespace InGameScene.UI
{
    //===========================================================
    // ���� ��ư�� ���� �ٲ�� ���� UI�� �⺻ ���̽� Ŭ����
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