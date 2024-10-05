// Copyright 2013-2022 AFI, INC. All rights reserved.

using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI
{
    //===========================================================
    // 유저의 레벨, 경험치, 이름을 알려주는 UI
    //===========================================================
    public class InGameUI_User : MonoBehaviour
    {
        [SerializeField] private TMP_Text _NickNameText;
        [SerializeField] private Slider _ExpSlider;
        [SerializeField] private TMP_Text _ExpText;

        [SerializeField] private Image _playerIcon;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _moneyText;
        [SerializeField] private TMP_Text _gemText;

        public void Init()
        {
            UpdateUI();
        }

        // 현재 UserData로 UI를 업데이트
        public void UpdateUI()
        {
            _NickNameText.text = Backend.UserNickName;

            _ExpSlider.maxValue = StaticManager.Backend.GameData.UserData.MaxExp;
            _ExpSlider.value = StaticManager.Backend.GameData.UserData.Exp;

            _ExpText.text = Mathf.FloorToInt(StaticManager.Backend.GameData.UserData.Exp).ToString() + "/" + StaticManager.Backend.GameData.UserData.MaxExp;

            _levelText.text = "Lv." + StaticManager.Backend.GameData.UserData.Level.ToString();
            _moneyText.text = StaticManager.Backend.GameData.UserData.Money.ToString();
            _gemText.text = StaticManager.Backend.GameData.UserData.Gem.ToString();

            int playerIcon = StaticManager.Backend.GameData.UserData.PlayerIcon;

            if (playerIcon <= 9)
            {
                _playerIcon.sprite = Resources.Load<Sprite>("Images/PlayerIcon/Icon_0" + playerIcon);
            }
            else
            {
                _playerIcon.sprite = Resources.Load<Sprite>("Images/PlayerIcon/Icon_" + playerIcon);
            }

            //_playerIcon.sprite = Resources.Load<Sprite>("Images/PlayerIcon/Icon_" + StaticManager.Backend.GameData.UserData.PlayerIcon);
        }
    }

}