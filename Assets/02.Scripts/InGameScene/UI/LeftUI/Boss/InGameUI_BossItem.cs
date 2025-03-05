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
        // 보스 정보를 초기화하고 버튼 동작을 설정하는 함수
        _bossInfo = bossInfo;

        // 보스 생성 버튼의 기존 리스너를 모두 제거
        _bossSpawnButton.onClick.RemoveAllListeners();

        // 닫기 동작을 버튼 클릭에 추가
        _bossSpawnButton.onClick.AddListener(onClose);

        // 보스 스폰 버튼 클릭 시 실행할 동작 추가
        _bossSpawnButton.onClick.AddListener(OnClickBossSpawnButton);

        // 보스 스폰 잠금 해제 여부를 확인
        UnLockCheck();

        // 보스 정보 UI 텍스트를 설정
        // 보스 이름, 권장 전투력, 보상 정보 표시
        _bossInfoText.text = _bossInfo.BossName + "\n" +
                             "권장 전투력 : " + (_bossInfo.Hp / 100).ToString() + "K" + "\n" +
                             "보상 : " + _bossInfo.Money + "G";
    }
    public void UnLockCheck()
    {
        // 현재 유저의 레벨과 보스의 요구 레벨을 비교하여 잠금 상태를 설정하는 함수
        if (_bossInfo.Lv <= StaticManager.Backend.GameData.UserData.Level)
        {
            // 유저 레벨이 요구 레벨 이상인 경우
            // 잠금 이미지를 비활성화하고 보스 생성 버튼을 활성화
            _unLockImage.gameObject.SetActive(false);
            _unLockLevelText.gameObject.SetActive(false);
            _bossSpawnButton.interactable = true;
        }
        else
        {
            // 유저 레벨이 요구 레벨 미만인 경우
            // 잠금 이미지를 활성화하고 필요 레벨을 표시
            _unLockImage.gameObject.SetActive(true);
            _unLockLevelText.gameObject.SetActive(true);
            _unLockLevelText.text = "필요 레벨\n" + _bossInfo.Lv.ToString();

            // 보스 생성 버튼을 비활성화
            _bossSpawnButton.interactable = false;
        }
    }
    private void OnClickBossSpawnButton()
    {
        // 보스 생성 버튼 클릭 시 호출되는 함수
        // 보스를 생성하는 작업을 처리
        InGameScene.Managers.Process.SpawnBossEnemy(_bossInfo);
    }

}
