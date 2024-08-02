// Copyright 2013-2022 AFI, INC. All rights reserved.

using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI
{
    //===========================================================
    // ������ ����, ����ġ, �̸��� �˷��ִ� UI
    //===========================================================
    public class InGameUI_User : MonoBehaviour
    {
        [SerializeField] private TMP_Text _NickNameText;
        [SerializeField] private Slider _ExpSlider;

        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _moneyText;

        public void Init()
        {
            UpdateUI();
        }

        // ���� UserData�� UI�� ������Ʈ
        public void UpdateUI()
        {
            _NickNameText.text = Backend.UserNickName;

            _ExpSlider.maxValue = StaticManager.Backend.GameData.UserData.MaxExp;
            _ExpSlider.value = StaticManager.Backend.GameData.UserData.Exp;

            _levelText.text = StaticManager.Backend.GameData.UserData.Level.ToString();
            _moneyText.text = StaticManager.Backend.GameData.UserData.Money.ToString();
        }
    }

}