// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI
{
    //===========================================================
    // 아래 버튼 및 각 버튼별 UI를 교체하는 아랫쪽 UI를 제어하는 클래스
    //===========================================================
    public class InGameUI_LeftUI : MonoBehaviour
    {
        private InGameUI_LeftUIBase[] _bottomUIs;
        private Button[] _leftUIButtons;

        [SerializeField] private GameObject _bottomUIPanel; //전체 화면을 가리는 백그라운드 UI 
        [SerializeField] private GameObject _UIChangeButtonParentObject;

        //===========================================================
        // 씬에서 다른 바텀 UI들은 전부 활성화가 되어있어야한다.
        //===========================================================


        public void Init()
        {
            _bottomUIs = transform.GetComponentsInChildren<InGameUI_LeftUIBase>();

            // BottomUI 정보 불러와 초기화
            foreach (var ui in _bottomUIs)
            {
                ui.Init();
            }

            //바텀UI의 버튼 배열 
            _leftUIButtons = _UIChangeButtonParentObject.GetComponentsInChildren<Button>();

            // 각 버튼별 클릭시 활성화되는 UI 배정
            for (int i = 0; i < _leftUIButtons.Length; i++)
            {
                int index = i;
                _leftUIButtons[index].onClick.AddListener(() => ChangeUI(index));
            }

            // 2번 UI 현재 장비로 초기 설정
            //ChangeUI(2);

            _bottomUIPanel.SetActive(false);
            foreach (var ui in _bottomUIs)
            { 
                ui.gameObject.SetActive(false);
            }
        }

        // 바텀 내 각 BottomUIBase를 가지고 있는 UI 클래스에 접근
        public T GetUI<T>() where T : InGameUI_LeftUIBase
        {
            for (int i = 0; i < _bottomUIs.Length; i++)
            {
                if (typeof(T) == _bottomUIs[i].GetType())
                {
                    return (T)_bottomUIs[i];
                }
            }

            throw new Exception($"{typeof(T)}가 존재하지 않습니다.");
        }

        // 버튼을 누를경우 해당 UI로 변경
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

                // 배열을 순회하면서 해당 UI에 맞는 클래스에 존재할 경우 활성화, 나머지는 비활성화
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
                    $"활성되지 않은 Bottom UI가 존재합니다.\n시도된 UI : {index}번\n전체 Bottom UI 개수 : {_bottomUIs.Length}\n\n{e}");
            }
        }
        public void CloseUI()
        {
            _bottomUIPanel.SetActive(false);
            _UIChangeButtonParentObject.transform.SetParent(this.gameObject.transform);
        }
    }
}