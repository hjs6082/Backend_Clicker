// Copyright 2013-2022 AFI, INC. All rights reserved.

using UnityEngine;

namespace InGameScene.UI.PopupUI
{
    //===========================================================
    // �˾� UI�� ���̽� Ŭ����
    //===========================================================
    public abstract class InGamePopupUI : MonoBehaviour
    {
        public abstract void Init();
        public abstract void Open();
    }
}