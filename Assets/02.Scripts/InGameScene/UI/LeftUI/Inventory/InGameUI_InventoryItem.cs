using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace InGameScene.UI
{
    public class InGameUI_InventoryItem : MonoBehaviour
    {
        [SerializeField]
        private Image _weaponImage;
        [SerializeField]
        private TMP_Text _weaponLevelText;
        [SerializeField]
        private Image _focusImage;
        [SerializeField]
        private Button _clickButton;

        private BackendData.GameData.WeaponInventory.Item _weaponItem;

        public void Init(BackendData.GameData.WeaponInventory.Item weaponItem, UnityAction onClickButton)
        {
            _weaponItem = weaponItem;
            _weaponImage.sprite = _weaponItem.GetWeaponChartData().WeaponSprite;
            _weaponLevelText.text = "+" + weaponItem.WeaponLevel.ToString();
            _clickButton.onClick.AddListener(onClickButton);
        }

        public void SetFocus(bool isFocus)
        {
            _focusImage.gameObject.SetActive(isFocus);
        }

        public void SetLevel()
        {
            _weaponLevelText.text = "+" + _weaponItem.WeaponLevel;
        }
    }
}