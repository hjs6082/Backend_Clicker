// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI
{
    //===========================================================
    // �Ʒ� ��ư �� �� ��ư�� UI�� ��ü�ϴ� �Ʒ��� UI�� �����ϴ� Ŭ����
    //===========================================================
    public class InGameUI_LeftUI : MonoBehaviour
    {
        private InGameUI_LeftUIBase[] _bottomUIs;
        private Button[] _leftUIButtons;

        [SerializeField] private GameObject _bottomUIPanel; //��ü ȭ���� ������ ��׶��� UI 
        [SerializeField] private GameObject _UIChangeButtonParentObject;

        //===========================================================
        // ������ �ٸ� ���� UI���� ���� Ȱ��ȭ�� �Ǿ��־���Ѵ�.
        //===========================================================


        public void Init()
        {
            _bottomUIs = transform.GetComponentsInChildren<InGameUI_LeftUIBase>();

            // BottomUI ���� �ҷ��� �ʱ�ȭ
            foreach (var ui in _bottomUIs)
            {
                ui.Init();
            }

            //����UI�� ��ư �迭 
            _leftUIButtons = _UIChangeButtonParentObject.GetComponentsInChildren<Button>();

            // �� ��ư�� Ŭ���� Ȱ��ȭ�Ǵ� UI ����
            for (int i = 0; i < _leftUIButtons.Length; i++)
            {
                int index = i;
                _leftUIButtons[index].onClick.AddListener(() => ChangeUI(index));
            }

            // 2�� UI ���� ���� �ʱ� ����
            //ChangeUI(2);

            _bottomUIPanel.SetActive(false);
            foreach (var ui in _bottomUIs)
            { 
                ui.gameObject.SetActive(false);
            }
        }

        // ���� �� �� BottomUIBase�� ������ �ִ� UI Ŭ������ ����
        public T GetUI<T>() where T : InGameUI_LeftUIBase
        {
            for (int i = 0; i < _bottomUIs.Length; i++)
            {
                if (typeof(T) == _bottomUIs[i].GetType())
                {
                    return (T)_bottomUIs[i];
                }
            }

            throw new Exception($"{typeof(T)}�� �������� �ʽ��ϴ�.");
        }

        // ��ư�� ������� �ش� UI�� ����
        void ChangeUI(int index)
        {
            try
            {
                if (!_bottomUIPanel.activeSelf)
                {
                    _bottomUIPanel.SetActive(true);
                    _UIChangeButtonParentObject.transform.SetParent(_bottomUIPanel.transform);
                }

                for (int i = 0; i < _leftUIButtons.Length; i++)
                {
                    _leftUIButtons[i].image.color = Color.white;
                }

                _leftUIButtons[index].image.color = Color.gray;

                Type type = _bottomUIs[index].GetType();

                // �迭�� ��ȸ�ϸ鼭 �ش� UI�� �´� Ŭ������ ������ ��� Ȱ��ȭ, �������� ��Ȱ��ȭ
                for (int i = 0; i < _bottomUIs.Length; i++)
                {

                    if (_bottomUIs[i].GetType() == type)
                    {
                        _bottomUIs[i].gameObject.SetActive(true);

                        _bottomUIs[i].Open();
                    }
                    else
                    {
                        _bottomUIs[i].gameObject.SetActive(false);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Ȱ������ ���� Bottom UI�� �����մϴ�.\n�õ��� UI : {index}��\n��ü Bottom UI ���� : {_bottomUIs.Length}\n\n{e}");
            }
        }
        public void CloseUI()
        {
            _bottomUIPanel.SetActive(false);
            _UIChangeButtonParentObject.transform.SetParent(this.gameObject.transform);
        }
    }
}