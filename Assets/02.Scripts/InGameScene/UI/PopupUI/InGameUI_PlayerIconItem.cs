using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI_PlayerIconItem : MonoBehaviour
{
    [SerializeField]
    private Button _button;
    [SerializeField]
    private Image _iconImage;

    private int _iconIndex;
    
    void Start()
    {
        _button = this.gameObject.GetComponent<Button>();
        //_iconImage = this.gameObject.GetComponent<Image>();
        _button.onClick.AddListener(OnClickPlayerIcon);
    }

    public void SetItem(Sprite sprite, int index)
    {
        _iconImage.sprite = sprite;
        _iconIndex = index;
    }

    void OnClickPlayerIcon()
    {
         InGameScene.Managers.Game.UpdatePlayerIcon(_iconIndex);
    }
}
