using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class InGameUI_BossItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _unLockLevelText;
    [SerializeField]
    private Image _unLockImage;

    [SerializeField]
    private Button _bossSpawnButton;
    [SerializeField]
    private TMP_Text _bossInfoText;

    private BackendData.Chart.Boss.Item _bossInfo;

    public void Init(BackendData.Chart.Boss.Item bossInfo, UnityAction onClose)
    {
        _bossInfo = bossInfo;

        _bossSpawnButton.onClick.RemoveAllListeners();
        _bossSpawnButton.onClick.AddListener(onClose);
        _bossSpawnButton.onClick.AddListener(OnClickBossSpawnButton);

        UnLockCheck();

        _bossInfoText.text = _bossInfo.BossName + "\n" + "권장 전투력 : " + (_bossInfo.Hp / 100).ToString() + "K" + "\n" + "보상 : " + _bossInfo.Money + "G";
    }

    public void UnLockCheck()
    {
        if(_bossInfo.Lv <= StaticManager.Backend.GameData.UserData.Level)
        {
            _unLockImage.gameObject.SetActive(false);
            _unLockLevelText.gameObject.SetActive(false);
            _bossSpawnButton.interactable = true;
        }
        else
        {
            _unLockImage.gameObject.SetActive(true);
            _unLockLevelText.gameObject.SetActive(true);
            _unLockLevelText.text = "필요 레벨\n" + _bossInfo.Lv.ToString();
            _bossSpawnButton.interactable = false;
        }
    }

    private void OnClickBossSpawnButton()
    {
        InGameScene.Managers.Process.SpawnBossEnemy(_bossInfo);
    }
}
