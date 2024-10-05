// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InGameScene.UI
{
    //===========================================================
    // ���������� �Ѿ������ ���� ���������� �˷��ִ� UI
    //===========================================================
    public class InGameUI_Stage : MonoBehaviour
    {
        [SerializeField] private GameObject _titleStageText;
        [SerializeField] private Slider _stageSlider;
        [SerializeField] private Image[] _stageIcons;

        //private float _titleStateTextMoveSpeed = 400.0f;

        // ���� �������� �����ִ� �ڷ�ƾ �Լ� �����ϴ� �Լ�
        public void ShowTitleStage(string stageName)
        {
            _titleStageText.GetComponentInChildren<TMP_Text>().text = $"{stageName}";
            //StartCoroutine(UpdownTitleInGameUI_Stage());
        }

/*        // ���� �������� UI�� �Ʒ��� �������鼭 �����ְ� �ö���� �ڷ�ƾ
        IEnumerator UpdownTitleInGameUI_Stage()
        {
            _titleStageText.gameObject.SetActive(true);

            Vector3 upPosition = new Vector3(0, 0, 0);
            Vector3 downPosition = new Vector3(0, -400, 0);

            _titleStageText.transform.localPosition = upPosition;
            var tempPosition = _titleStageText.transform.localPosition;

            while (_titleStageText.transform.localPosition.y > downPosition.y)
            {
                tempPosition.y -= _titleStateTextMoveSpeed * Time.deltaTime;
                _titleStageText.transform.localPosition = tempPosition;

                yield return null;
            }

            yield return new WaitForSeconds(2.0f);

            while (_titleStageText.transform.localPosition.y < upPosition.y)
            {
                tempPosition.y += _titleStateTextMoveSpeed * Time.deltaTime;
                _titleStageText.transform.localPosition = tempPosition;

                yield return null;
            }

            _titleStageText.gameObject.SetActive(false);
        }*/
    }
}