using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace InGameScene.UI
{
    public class InGameUI_InventoryItem_Box : MonoBehaviour
    {
        [SerializeField]
        private Image _boxImage;
        [SerializeField]
        private TMP_Text _boxCount;
        [SerializeField]
        private Image _focusImage;
        [SerializeField]
        private Button _clickButton;

        private BackendData.Chart.Item.Item _boxItem;

        public void Init(BackendData.Chart.Item.Item boxItem, UnityAction onClickButton)
        {
            _boxItem = boxItem;
            _boxImage.sprite = _boxItem.ImageSprite;
            foreach (var item in StaticManager.Backend.GameData.ItemInventory.Dictionary)
            {
                if(item.Key == _boxItem.ItemID)
                {
                    _boxCount.text = item.Value.ToString();
                }
            }
            _clickButton.onClick.AddListener(onClickButton);
        }

        public void SetFocus(bool isFocus)
        {
            _focusImage.gameObject.SetActive(isFocus);
        }

    }
}