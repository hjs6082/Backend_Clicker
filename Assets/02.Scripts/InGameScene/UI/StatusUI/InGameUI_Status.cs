using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace InGameScene.UI
{
    public class InGameUI_Status : MonoBehaviour
    {
        [SerializeField] private TMP_Text _moneyText;
        //[SerializeField] private TMP_Text _gemText;

        public void Init()
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            _moneyText.text = StaticManager.Backend.GameData.UserData.Money.ToString();
        }
    }
}
