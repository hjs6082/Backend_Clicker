// Copyright 2013-2022 AFI, INC. All rights reserved.

using UnityEngine;

namespace InGameScene.UI
{
    //===========================================================
    // ���� ��ư�� ���� �ٲ�� ���� UI�� �⺻ ���̽� Ŭ����
    //===========================================================
    public class InGameUI_LeftUIBase : MonoBehaviour
    {
        [SerializeField] private InGameUI_Status _status;
        public virtual void Init()
        {
        }

        public virtual void Open()
        {
        }

        public void UpdateUI()
        {
            _status.UpdateUI();
        }
    }
}